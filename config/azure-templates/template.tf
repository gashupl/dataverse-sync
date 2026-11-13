terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.0, < 5.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = "~> 2.0"
    }
  }
}

provider "azurerm" {
  features {}
}

variable "location" {
  description = "The location/region for the resources"
  type        = string
}

variable "resourceGroupName" {
  description = "The existing resource group to deploy into (Bicep receives this through deployment scope)"
  type        = string
}

variable "functionAppName" {
  description = "The name of the Function App"
  type        = string
}

variable "serviceBusName" {
  description = "The name of the Service Bus"
  type        = string
}

variable "dotnetVersion" {
  description = "The .NET version for the runtime stack"
  type        = string

  validation {
    condition     = contains(["8", "9", "10"], var.dotnetVersion)
    error_message = "dotnetVersion must be 8, 9, or 10."
  }
}

variable "environmentSuffix" {
  description = "Environment suffix for resource naming"
  type        = string
}

locals {
  full_function_app_name = "${var.functionAppName}-${var.environmentSuffix}"
  full_service_bus_name  = "${var.serviceBusName}-${var.environmentSuffix}"
  storage_account_name   = "st${replace(local.full_function_app_name, "-", "")}${var.environmentSuffix}"
  app_insights_name      = "appi-${local.full_function_app_name}"
  app_service_plan_name  = "asp-${local.full_function_app_name}"

  storage_blob_data_contributor_role_id = "ba92f5b4-2d11-453d-a403-e96b0029c9fe"
}

resource "azurerm_storage_account" "function_app" {
  name                            = substr(local.storage_account_name, 0, 24)
  resource_group_name             = data.azurerm_resource_group.current.name
  location                        = var.location
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  account_kind                    = "StorageV2"
  access_tier                     = "Hot"
  min_tls_version                 = "TLS1_2"
  allow_nested_items_to_be_public = false
  https_traffic_only_enabled      = true
}

data "azurerm_resource_group" "current" {
  name = var.resourceGroupName
}

resource "azurerm_application_insights" "function_app" {
  name                = local.app_insights_name
  location            = var.location
  resource_group_name = data.azurerm_resource_group.current.name
  application_type    = "web"

  internet_ingestion_enabled = true
  internet_query_enabled     = true
}

resource "azurerm_service_plan" "function_app" {
  name                = local.app_service_plan_name
  resource_group_name = data.azurerm_resource_group.current.name
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_windows_function_app" "function_app" {
  name                = local.full_function_app_name
  resource_group_name = data.azurerm_resource_group.current.name
  location            = var.location
  service_plan_id     = azurerm_service_plan.function_app.id

  storage_account_name       = azurerm_storage_account.function_app.name
  storage_account_access_key = azurerm_storage_account.function_app.primary_access_key

  https_only                                     = true
  ftp_publish_basic_authentication_enabled       = true
  webdeploy_publish_basic_authentication_enabled = true
  functions_extension_version                    = "~4"

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    WEBSITE_CONTENTSHARE     = lower(var.functionAppName)
    FUNCTIONS_WORKER_RUNTIME = "dotnet-isolated"
    WEBSITE_RUN_FROM_PACKAGE = "1"
  }

  site_config {
    ftps_state                             = "Disabled"
    minimum_tls_version                    = "1.2"
    use_32_bit_worker                      = false
    application_insights_key               = azurerm_application_insights.function_app.instrumentation_key
    application_insights_connection_string = azurerm_application_insights.function_app.connection_string

    application_stack {
      dotnet_version              = "v${var.dotnetVersion}.0"
      use_dotnet_isolated_runtime = true
    }

    cors {
      allowed_origins = ["https://portal.azure.com"]
    }
  }
}

resource "azapi_update_resource" "function_app_client_affinity" {
  type        = "Microsoft.Web/sites@2023-12-01"
  resource_id = azurerm_windows_function_app.function_app.id

  body = {
    properties = {
      clientAffinityEnabled = false
    }
  }
}

resource "azurerm_role_assignment" "function_app_storage" {
  name                 = uuidv5("url", "${azurerm_storage_account.function_app.id}/${azurerm_windows_function_app.function_app.id}/${local.storage_blob_data_contributor_role_id}")
  scope                = azurerm_storage_account.function_app.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_windows_function_app.function_app.identity[0].principal_id
  principal_type       = "ServicePrincipal"
}

resource "azurerm_servicebus_namespace" "function_app" {
  name                = local.full_service_bus_name
  location            = var.location
  resource_group_name = data.azurerm_resource_group.current.name
  sku                 = "Basic"
}

resource "azurerm_servicebus_queue" "function_app" {
  name         = "dv-sync-queue"
  namespace_id = azurerm_servicebus_namespace.function_app.id
}

resource "azurerm_servicebus_queue_authorization_rule" "function_app" {
  name     = "SendListenAccessPolicy"
  queue_id = azurerm_servicebus_queue.function_app.id
  listen   = true
  send     = true
  manage   = false
}

output "functionAppId" {
  description = "The resource ID of the Function App"
  value       = azurerm_windows_function_app.function_app.id
}

output "functionAppName" {
  description = "The name of the Function App"
  value       = azurerm_windows_function_app.function_app.name
}

output "functionAppDefaultHostname" {
  description = "The default hostname of the Function App"
  value       = azurerm_windows_function_app.function_app.default_hostname
}

output "applicationInsightsConnectionString" {
  description = "The Application Insights connection string"
  value       = azurerm_application_insights.function_app.connection_string
  sensitive   = true
}

output "applicationInsightsInstrumentationKey" {
  description = "The Application Insights instrumentation key"
  value       = azurerm_application_insights.function_app.instrumentation_key
  sensitive   = true
}

output "storageAccountId" {
  description = "The resource ID of the Storage Account"
  value       = azurerm_storage_account.function_app.id
}

output "storageAccountName" {
  description = "The name of the Storage Account"
  value       = azurerm_storage_account.function_app.name
}

output "serviceBusNamespaceId" {
  description = "The resource ID of the Service Bus Namespace"
  value       = azurerm_servicebus_namespace.function_app.id
}

output "serviceBusNamespaceName" {
  description = "The name of the Service Bus Namespace"
  value       = azurerm_servicebus_namespace.function_app.name
}

output "serviceBusQueueName" {
  description = "The name of the Service Bus Queue"
  value       = azurerm_servicebus_queue.function_app.name
}

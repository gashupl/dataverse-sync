#pragma warning disable CS1591

namespace Pg.DataverseSync.Model
{
	
	
	/// <summary>
	/// The secret store where the secret is persisted.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum EnvironmentVariableDefinition_SecretStore
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Azure Key Vault", 0, "#0000ff")]
		AzureKeyVault = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Microsoft Dataverse", 1, "#0000ff")]
		MicrosoftDataverse = 1,
	}
	
	/// <summary>
	/// Status of the Environment Variable Definition
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum environmentvariabledefinition_statecode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Active", 0)]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Inactive", 1)]
		Inactive = 1,
	}
	
	/// <summary>
	/// Reason for the status of the Environment Variable Definition
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum EnvironmentVariableDefinition_StatusCode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Active", 0)]
		Active = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Inactive", 1)]
		Inactive = 2,
	}
	
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum EnvironmentVariableDefinition_Type
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Boolean", 2, "#0000ff")]
		Boolean = 100000002,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Data Source", 4, "#0000ff")]
		DataSource = 100000004,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("JSON", 3, "#0000ff")]
		Json = 100000003,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Number", 1, "#0000ff")]
		Number = 100000001,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Secret", 5, "#0000ff")]
		Secret = 100000005,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("String", 0, "#0000ff")]
		String = 100000000,
	}
	
	/// <summary>
	/// Contains information about the settable variable: its type, default value, and etc.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("environmentvariabledefinition")]
	public partial class EnvironmentVariableDefinition : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, at the time of codegen, for the environmentvariabledefinition entity
		/// </summary>
		public partial class Fields
		{
			public const string ApiId = "apiid";
			public const string ComponentState = "componentstate";
			public const string componentstateName = "componentstatename";
			public const string ConnectionReferenceId = "connectionreferenceid";
			public const string CreatedBy = "createdby";
			public const string CreatedByName = "createdbyname";
			public const string CreatedByYomiName = "createdbyyominame";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string CreatedOnBehalfByName = "createdonbehalfbyname";
			public const string CreatedOnBehalfByYomiName = "createdonbehalfbyyominame";
			public const string DefaultValue = "defaultvalue";
			public const string Description = "description";
			public const string DisplayName = "displayname";
			public const string Referencingenvdefinition_envdefinition = "envdefinition_envdefinition";
			public const string environmentvariabledefinition_environmentvariablevalue = "environmentvariabledefinition_environmentvariablevalue";
			public const string EnvironmentVariableDefinitionId = "environmentvariabledefinitionid";
			public const string Id = "environmentvariabledefinitionid";
			public const string EnvironmentVariableDefinitionIdUnique = "environmentvariabledefinitionidunique";
			public const string Hint = "hint";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string InputControlConfig = "inputcontrolconfig";
			public const string IntroducedVersion = "introducedversion";
			public const string IsCustomizable = "iscustomizable";
			public const string IsManaged = "ismanaged";
			public const string ismanagedName = "ismanagedname";
			public const string IsRequired = "isrequired";
			public const string isrequiredName = "isrequiredname";
			public const string LearnMoreUrl = "learnmoreurl";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedByName = "modifiedbyname";
			public const string ModifiedByYomiName = "modifiedbyyominame";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ModifiedOnBehalfByName = "modifiedonbehalfbyname";
			public const string ModifiedOnBehalfByYomiName = "modifiedonbehalfbyyominame";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string OverwriteTime = "overwritetime";
			public const string OwnerId = "ownerid";
			public const string OwnerIdName = "owneridname";
			public const string OwnerIdYomiName = "owneridyominame";
			public const string OwningBusinessUnit = "owningbusinessunit";
			public const string OwningBusinessUnitName = "owningbusinessunitname";
			public const string OwningTeam = "owningteam";
			public const string OwningUser = "owninguser";
			public const string ParameterKey = "parameterkey";
			public const string ParentDefinitionId = "parentdefinitionid";
			public const string ParentDefinitionIdName = "parentdefinitionidname";
			public const string Referencedenvdefinition_envdefinition = "Referencedenvdefinition_envdefinition";
			public const string SchemaName = "schemaname";
			public const string SecretStore = "secretstore";
			public const string secretstoreName = "secretstorename";
			public const string SolutionId = "solutionid";
			public const string StateCode = "statecode";
			public const string statecodeName = "statecodename";
			public const string StatusCode = "statuscode";
			public const string statuscodeName = "statuscodename";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string Type = "type";
			public const string typeName = "typename";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string ValueSchema = "valueschema";
			public const string VersionNumber = "versionnumber";
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public EnvironmentVariableDefinition(System.Guid id) : 
				base(EntityLogicalName, id)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public EnvironmentVariableDefinition(string keyName, object keyValue) : 
				base(EntityLogicalName, keyName, keyValue)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public EnvironmentVariableDefinition(Microsoft.Xrm.Sdk.KeyAttributeCollection keyAttributes) : 
				base(EntityLogicalName, keyAttributes)
		{
		}
		
		public const string AlternateKeys = "componentstate,overwritetime,schemaname";
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public EnvironmentVariableDefinition() : 
				base(EntityLogicalName)
		{
		}
		
		public const string PrimaryIdAttribute = "environmentvariabledefinitionid";
		
		public const string PrimaryNameAttribute = "schemaname";
		
		public const string EntitySchemaName = "EnvironmentVariableDefinition";
		
		public const string EntityLogicalName = "environmentvariabledefinition";
		
		public const string EntityLogicalCollectionName = "environmentvariabledefinitions";
		
		public const string EntitySetName = "environmentvariabledefinitions";
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("apiid")]
		public string ApiId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("apiid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("apiid", value);
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("componentstate")]
		public virtual ComponentState? ComponentState
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((ComponentState?)(EntityOptionSetEnum.GetEnum(this, "componentstate")));
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("componentstatename")]
		public string componentstateName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("componentstate"))
				{
					return this.FormattedValues["componentstate"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier for Connection Reference associated with Environment Variable Definition.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("connectionreferenceid")]
		public Microsoft.Xrm.Sdk.EntityReference ConnectionReferenceId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("connectionreferenceid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("connectionreferenceid", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyname")]
		public string CreatedByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdbyyominame")]
		public string CreatedByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdby"))
				{
					return this.FormattedValues["createdby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Date and time when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who created the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("createdonbehalfby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyname")]
		public string CreatedOnBehalfByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfbyyominame")]
		public string CreatedOnBehalfByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("createdonbehalfby"))
				{
					return this.FormattedValues["createdonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Default variable value to be used if no associated EnvironmentVariableValue entities exist.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("defaultvalue")]
		public string DefaultValue
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("defaultvalue");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("defaultvalue", value);
			}
		}
		
		/// <summary>
		/// Description of the variable definition.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("description")]
		public string Description
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("description");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("description", value);
			}
		}
		
		/// <summary>
		/// Display Name of the variable definition.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("displayname")]
		public string DisplayName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("displayname");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("displayname", value);
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("environmentvariabledefinitionid")]
		public System.Nullable<System.Guid> EnvironmentVariableDefinitionId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("environmentvariabledefinitionid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("environmentvariabledefinitionid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("environmentvariabledefinitionid")]
		public override System.Guid Id
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return base.Id;
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.EnvironmentVariableDefinitionId = value;
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("environmentvariabledefinitionidunique")]
		public System.Nullable<System.Guid> EnvironmentVariableDefinitionIdUnique
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("environmentvariabledefinitionidunique");
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("hint")]
		public string Hint
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("hint");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("hint", value);
			}
		}
		
		/// <summary>
		/// Sequence number of the import that created this record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("importsequencenumber")]
		public System.Nullable<int> ImportSequenceNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("importsequencenumber", value);
			}
		}
		
		/// <summary>
		/// A JSON object describing the options for the input control that should be presented to the user for setting the current value of the Environment variable.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("inputcontrolconfig")]
		public string InputControlConfig
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("inputcontrolconfig");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("inputcontrolconfig", value);
			}
		}
		
		/// <summary>
		/// Version in which the form is introduced.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("introducedversion")]
		public string IntroducedVersion
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("introducedversion");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("introducedversion", value);
			}
		}
		
		/// <summary>
		/// Tells whether the component can be customized.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("iscustomizable")]
		public Microsoft.Xrm.Sdk.BooleanManagedProperty IsCustomizable
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.BooleanManagedProperty>("iscustomizable");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("iscustomizable", value);
			}
		}
		
		/// <summary>
		/// Indicates whether the solution component is part of a managed solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ismanaged")]
		public System.Nullable<bool> IsManaged
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ismanaged");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ismanagedname")]
		public string ismanagedName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ismanaged"))
				{
					return this.FormattedValues["ismanaged"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isrequired")]
		public System.Nullable<bool> IsRequired
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("isrequired");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("isrequired", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isrequiredname")]
		public string isrequiredName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("isrequired"))
				{
					return this.FormattedValues["isrequired"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Clicking on this url will take the user to a webpage which further explains the environment variable being populated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("learnmoreurl")]
		public string LearnMoreUrl
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("learnmoreurl");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("learnmoreurl", value);
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyname")]
		public string ModifiedByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedbyyominame")]
		public string ModifiedByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedby"))
				{
					return this.FormattedValues["modifiedby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Date and time when the record was modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who modified the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("modifiedonbehalfby", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyname")]
		public string ModifiedOnBehalfByName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfbyyominame")]
		public string ModifiedOnBehalfByYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("modifiedonbehalfby"))
				{
					return this.FormattedValues["modifiedonbehalfby"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overriddencreatedon")]
		public System.Nullable<System.DateTime> OverriddenCreatedOn
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("overriddencreatedon", value);
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overwritetime")]
		public System.Nullable<System.DateTime> OverwriteTime
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overwritetime");
			}
		}
		
		/// <summary>
		/// Owner Id
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ownerid")]
		public Microsoft.Xrm.Sdk.EntityReference OwnerId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ownerid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("ownerid", value);
			}
		}
		
		/// <summary>
		/// Name of the owner
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owneridname")]
		public string OwnerIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ownerid"))
				{
					return this.FormattedValues["ownerid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Yomi name of the owner
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owneridyominame")]
		public string OwnerIdYomiName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("ownerid"))
				{
					return this.FormattedValues["ownerid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier for the business unit that owns the record
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunit")]
		public Microsoft.Xrm.Sdk.EntityReference OwningBusinessUnit
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningbusinessunit");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningbusinessunitname")]
		public string OwningBusinessUnitName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("owningbusinessunit"))
				{
					return this.FormattedValues["owningbusinessunit"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier for the team that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owningteam")]
		public Microsoft.Xrm.Sdk.EntityReference OwningTeam
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owningteam");
			}
		}
		
		/// <summary>
		/// Unique identifier for the user that owns the record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("owninguser")]
		public Microsoft.Xrm.Sdk.EntityReference OwningUser
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("owninguser");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parameterkey")]
		public string ParameterKey
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("parameterkey");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("parameterkey", value);
			}
		}
		
		/// <summary>
		/// Unique identifier for Environment Variable Definition associated with Environment Variable Definition.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentdefinitionid")]
		public Microsoft.Xrm.Sdk.EntityReference ParentDefinitionId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("parentdefinitionid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("parentdefinitionid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentdefinitionidname")]
		public string ParentDefinitionIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("parentdefinitionid"))
				{
					return this.FormattedValues["parentdefinitionid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique entity name.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("schemaname")]
		public string SchemaName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("schemaname");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("schemaname", value);
			}
		}
		
		/// <summary>
		/// Environment variable secret store.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("secretstore")]
		public virtual EnvironmentVariableDefinition_SecretStore? SecretStore
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((EnvironmentVariableDefinition_SecretStore?)(EntityOptionSetEnum.GetEnum(this, "secretstore")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("secretstore", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("secretstorename")]
		public string secretstoreName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("secretstore"))
				{
					return this.FormattedValues["secretstore"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Unique identifier of the associated solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
		public System.Nullable<System.Guid> SolutionId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("solutionid");
			}
		}
		
		/// <summary>
		/// Status of the Environment Variable Definition
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public virtual environmentvariabledefinition_statecode? StateCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((environmentvariabledefinition_statecode?)(EntityOptionSetEnum.GetEnum(this, "statecode")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("statecode", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecodename")]
		public string statecodeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("statecode"))
				{
					return this.FormattedValues["statecode"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Reason for the status of the Environment Variable Definition
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual EnvironmentVariableDefinition_StatusCode? StatusCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((EnvironmentVariableDefinition_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("statuscode", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscodename")]
		public string statuscodeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("statuscode"))
				{
					return this.FormattedValues["statuscode"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("timezoneruleversionnumber")]
		public System.Nullable<int> TimeZoneRuleVersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("timezoneruleversionnumber");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("timezoneruleversionnumber", value);
			}
		}
		
		/// <summary>
		/// Environment variable value type.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("type")]
		public virtual EnvironmentVariableDefinition_Type? Type
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((EnvironmentVariableDefinition_Type?)(EntityOptionSetEnum.GetEnum(this, "type")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("type", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("typename")]
		public string typeName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("type"))
				{
					return this.FormattedValues["type"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Time zone code that was in use when the record was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("utcconversiontimezonecode")]
		public System.Nullable<int> UTCConversionTimeZoneCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("utcconversiontimezonecode");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("utcconversiontimezonecode", value);
			}
		}
		
		/// <summary>
		/// For internal use only.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("valueschema")]
		public string ValueSchema
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("valueschema");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("valueschema", value);
			}
		}
		
		/// <summary>
		/// Version Number
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
		
		/// <summary>
		/// 1:N envdefinition_envdefinition
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("envdefinition_envdefinition", Microsoft.Xrm.Sdk.EntityRole.Referenced)]
		public System.Collections.Generic.IEnumerable<Pg.DataverseSync.Model.EnvironmentVariableDefinition> Referencedenvdefinition_envdefinition
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Pg.DataverseSync.Model.EnvironmentVariableDefinition>("envdefinition_envdefinition", Microsoft.Xrm.Sdk.EntityRole.Referenced);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<Pg.DataverseSync.Model.EnvironmentVariableDefinition>("envdefinition_envdefinition", Microsoft.Xrm.Sdk.EntityRole.Referenced, value);
			}
		}
		
		/// <summary>
		/// 1:N environmentvariabledefinition_environmentvariablevalue
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("environmentvariabledefinition_environmentvariablevalue")]
		public System.Collections.Generic.IEnumerable<Pg.DataverseSync.Model.EnvironmentVariableValue> environmentvariabledefinition_environmentvariablevalue
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntities<Pg.DataverseSync.Model.EnvironmentVariableValue>("environmentvariabledefinition_environmentvariablevalue", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntities<Pg.DataverseSync.Model.EnvironmentVariableValue>("environmentvariabledefinition_environmentvariablevalue", null, value);
			}
		}
		
		/// <summary>
		/// N:1 envdefinition_envdefinition
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("parentdefinitionid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("envdefinition_envdefinition", Microsoft.Xrm.Sdk.EntityRole.Referencing)]
		public Pg.DataverseSync.Model.EnvironmentVariableDefinition Referencingenvdefinition_envdefinition
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<Pg.DataverseSync.Model.EnvironmentVariableDefinition>("envdefinition_envdefinition", Microsoft.Xrm.Sdk.EntityRole.Referencing);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<Pg.DataverseSync.Model.EnvironmentVariableDefinition>("envdefinition_envdefinition", Microsoft.Xrm.Sdk.EntityRole.Referencing, value);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public EnvironmentVariableDefinition(object anonymousType) : 
				this()
		{
            foreach (var p in anonymousType.GetType().GetProperties())
            {
                var value = p.GetValue(anonymousType, null);
                var name = p.Name.ToLower();
            
                if (value != null && name.EndsWith("enum") && value.GetType().BaseType == typeof(System.Enum))
                {
                    value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);
                    name = name.Remove(name.Length - "enum".Length);
                }
            
                switch (name)
                {
                    case "id":
                        base.Id = (System.Guid)value;
                        Attributes["environmentvariabledefinitionid"] = base.Id;
                        break;
                    case "environmentvariabledefinitionid":
                        var id = (System.Nullable<System.Guid>) value;
                        if(id == null){ continue; }
                        base.Id = id.Value;
                        Attributes[name] = base.Id;
                        break;
                    case "formattedvalues":
                        // Add Support for FormattedValues
                        FormattedValues.AddRange((Microsoft.Xrm.Sdk.FormattedValueCollection)value);
                        break;
                    default:
                        Attributes[name] = value;
                        break;
                }
            }
		}
	}
}
#pragma warning restore CS1591

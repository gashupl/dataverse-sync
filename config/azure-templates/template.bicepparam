using './template.bicep'

// Basic configuration
param environmentSuffix = 'dev'
param functionAppName = 'func-dvsync-engine'
param serviceBusName = 'sb-dvsync-engine'
param location = 'West Europe'
param dotnetVersion = '10'  

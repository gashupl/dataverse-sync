#pragma warning disable CS1591

namespace Pg.DataverseSync.Model
{
	
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
	[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("pg_getunsynchronizedtables")]
	public partial class pg_getunsynchronizedtablesRequest : Microsoft.Xrm.Sdk.OrganizationRequest
	{
		
		public const string ActionLogicalName = "pg_getunsynchronizedtables";
		
		public pg_getunsynchronizedtablesRequest()
		{
			this.RequestName = "pg_getunsynchronizedtables";
		}
	}
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
	[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("pg_getunsynchronizedtables")]
	public partial class pg_getunsynchronizedtablesResponse : Microsoft.Xrm.Sdk.OrganizationResponse
	{
		
		public static class Fields
		{
			public const string tables = "tables";
		}
		
		public const string ActionLogicalName = "pg_getunsynchronizedtables";
		
		public pg_getunsynchronizedtablesResponse()
		{
		}
		
		public string tables
		{
			get
			{
				if (this.Results.Contains("tables"))
				{
					return ((string)(this.Results["tables"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Results["tables"] = value;
			}
		}
	}
}
#pragma warning restore CS1591

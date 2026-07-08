#pragma warning disable CS1591

namespace Pg.DataverseSync.Model
{
	
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
	[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("pg_gettables")]
	public partial class pg_gettablesRequest : Microsoft.Xrm.Sdk.OrganizationRequest
	{
		
		public const string ActionLogicalName = "pg_gettables";
		
		public pg_gettablesRequest()
		{
			this.RequestName = "pg_gettables";
		}
	}
	
	[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
	[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("pg_gettables")]
	public partial class pg_gettablesResponse : Microsoft.Xrm.Sdk.OrganizationResponse
	{
		
		public static class Fields
		{
			public const string alltables = "alltables";
		}
		
		public const string ActionLogicalName = "pg_gettables";
		
		public pg_gettablesResponse()
		{
		}
		
		public string alltables
		{
			get
			{
				if (this.Results.Contains("alltables"))
				{
					return ((string)(this.Results["alltables"]));
				}
				else
				{
					return default(string);
				}
			}
			set
			{
				this.Results["alltables"] = value;
			}
		}
	}
}
#pragma warning restore CS1591

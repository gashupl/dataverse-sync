#pragma warning disable CS1591

namespace Pg.DataverseSync.Model
{
	
	
	/// <summary>
	/// The state of this component.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum ComponentState
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Deleted", 2)]
		Deleted = 2,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Deleted Unpublished", 3)]
		DeletedUnpublished = 3,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Published", 0)]
		Published = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Unpublished", 1)]
		Unpublished = 1,
	}
}
#pragma warning restore CS1591

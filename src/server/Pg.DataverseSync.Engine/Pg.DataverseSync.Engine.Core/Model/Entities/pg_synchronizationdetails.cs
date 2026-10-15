#pragma warning disable CS1591

namespace Pg.DataverseSync.Engine.Core.Model
{
	
	
	/// <summary>
	/// Status of the Synchronization Details
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum pg_synchronizationdetails_statecode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Active", 0)]
		Active = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Inactive", 1)]
		Inactive = 1,
	}
	
	/// <summary>
	/// Reason for the status of the Synchronization Details
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	public enum pg_synchronizationdetails_StatusCode
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Active", 0)]
		Active = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		[OptionSetMetadataAttribute("Inactive", 1)]
		Inactive = 2,
	}
	
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("pg_synchronizationdetails")]
	public partial class pg_synchronizationdetails : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, at the time of codegen, for the pg_synchronizationdetails entity
		/// </summary>
		public partial class Fields
		{
			public const string CreatedBy = "createdby";
			public const string CreatedByName = "createdbyname";
			public const string CreatedByYomiName = "createdbyyominame";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string CreatedOnBehalfByName = "createdonbehalfbyname";
			public const string CreatedOnBehalfByYomiName = "createdonbehalfbyyominame";
			public const string ImportSequenceNumber = "importsequencenumber";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedByName = "modifiedbyname";
			public const string ModifiedByYomiName = "modifiedbyyominame";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ModifiedOnBehalfByName = "modifiedonbehalfbyname";
			public const string ModifiedOnBehalfByYomiName = "modifiedonbehalfbyyominame";
			public const string OrganizationId = "organizationid";
			public const string OrganizationIdName = "organizationidname";
			public const string OverriddenCreatedOn = "overriddencreatedon";
			public const string pg_checksum = "pg_checksum";
			public const string pg_lastsynchronizationdate = "pg_lastsynchronizationdate";
			public const string pg_synchronizationdetails_pg_synctableId_pg_synctable = "pg_synchronizationdetails_pg_synctableId_pg_synctable";
			public const string pg_synchronizationdetailsId = "pg_synchronizationdetailsid";
			public const string Id = "pg_synchronizationdetailsid";
			public const string pg_synctableId = "pg_synctableid";
			public const string pg_synctableIdName = "pg_synctableidname";
			public const string pg_targetrepository = "pg_targetrepository";
			public const string pg_targetrepositoryName = "pg_targetrepositoryname";
			public const string StateCode = "statecode";
			public const string statecodeName = "statecodename";
			public const string StatusCode = "statuscode";
			public const string statuscodeName = "statuscodename";
			public const string TimeZoneRuleVersionNumber = "timezoneruleversionnumber";
			public const string UTCConversionTimeZoneCode = "utcconversiontimezonecode";
			public const string VersionNumber = "versionnumber";
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public pg_synchronizationdetails(System.Guid id) : 
				base(EntityLogicalName, id)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public pg_synchronizationdetails(string keyName, object keyValue) : 
				base(EntityLogicalName, keyName, keyValue)
		{
		}
		
		[System.Diagnostics.DebuggerNonUserCode()]
		public pg_synchronizationdetails(Microsoft.Xrm.Sdk.KeyAttributeCollection keyAttributes) : 
				base(EntityLogicalName, keyAttributes)
		{
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public pg_synchronizationdetails() : 
				base(EntityLogicalName)
		{
		}
		
		public const string PrimaryIdAttribute = "pg_synchronizationdetailsid";
		
		public const string PrimaryNameAttribute = "pg_checksum";
		
		public const string EntitySchemaName = "pg_synchronizationdetails";
		
		public const string EntityLogicalName = "pg_synchronizationdetails";
		
		public const string EntityLogicalCollectionName = "pg_synchronizationdetailses";
		
		public const string EntitySetName = "pg_synchronizationdetailses";
		
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
		/// Unique identifier for the organization
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationid")]
		public Microsoft.Xrm.Sdk.EntityReference OrganizationId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("organizationid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationidname")]
		public string OrganizationIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("organizationid"))
				{
					return this.FormattedValues["organizationid"];
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
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_checksum")]
		public string pg_checksum
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<string>("pg_checksum");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("pg_checksum", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_lastsynchronizationdate")]
		public System.Nullable<System.DateTime> pg_lastsynchronizationdate
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("pg_lastsynchronizationdate");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("pg_lastsynchronizationdate", value);
			}
		}
		
		/// <summary>
		/// Unique identifier for entity instances
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_synchronizationdetailsid")]
		public System.Nullable<System.Guid> pg_synchronizationdetailsId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("pg_synchronizationdetailsid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("pg_synchronizationdetailsid", value);
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
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_synchronizationdetailsid")]
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
				this.pg_synchronizationdetailsId = value;
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_synctableid")]
		public Microsoft.Xrm.Sdk.EntityReference pg_synctableId
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("pg_synctableid");
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("pg_synctableid", value);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_synctableidname")]
		public string pg_synctableIdName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("pg_synctableid"))
				{
					return this.FormattedValues["pg_synctableid"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_targetrepository")]
		public virtual pg_TargetRepositorySet? pg_targetrepository
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((pg_TargetRepositorySet?)(EntityOptionSetEnum.GetEnum(this, "pg_targetrepository")));
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetAttributeValue("pg_targetrepository", value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null);
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_targetrepositoryname")]
		public string pg_targetrepositoryName
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				if (this.FormattedValues.Contains("pg_targetrepository"))
				{
					return this.FormattedValues["pg_targetrepository"];
				}
				else
				{
					return default(string);
				}
			}
		}
		
		/// <summary>
		/// Status of the Synchronization Details
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statecode")]
		public virtual pg_synchronizationdetails_statecode? StateCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((pg_synchronizationdetails_statecode?)(EntityOptionSetEnum.GetEnum(this, "statecode")));
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
		/// Reason for the status of the Synchronization Details
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("statuscode")]
		public virtual pg_synchronizationdetails_StatusCode? StatusCode
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return ((pg_synchronizationdetails_StatusCode?)(EntityOptionSetEnum.GetEnum(this, "statuscode")));
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
		/// N:1 pg_synchronizationdetails_pg_synctableId_pg_synctable
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("pg_synctableid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("pg_synchronizationdetails_pg_synctableId_pg_synctable")]
		public Pg.DataverseSync.Engine.Core.Model.pg_synctable pg_synchronizationdetails_pg_synctableId_pg_synctable
		{
			[System.Diagnostics.DebuggerNonUserCode()]
			get
			{
				return this.GetRelatedEntity<Pg.DataverseSync.Engine.Core.Model.pg_synctable>("pg_synchronizationdetails_pg_synctableId_pg_synctable", null);
			}
			[System.Diagnostics.DebuggerNonUserCode()]
			set
			{
				this.SetRelatedEntity<Pg.DataverseSync.Engine.Core.Model.pg_synctable>("pg_synchronizationdetails_pg_synctableId_pg_synctable", null, value);
			}
		}
		
		/// <summary>
		/// Constructor for populating via LINQ queries given a LINQ anonymous type
		/// <param name="anonymousType">LINQ anonymous type.</param>
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode()]
		public pg_synchronizationdetails(object anonymousType) : 
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
                        Attributes["pg_synchronizationdetailsid"] = base.Id;
                        break;
                    case "pg_synchronizationdetailsid":
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

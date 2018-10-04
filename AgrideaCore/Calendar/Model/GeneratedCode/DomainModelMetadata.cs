/////////////////////////////////////////////////////////////////////////////////
//                                                                             //
// <auto-generated>                                                            //
// Date=? Machine=? User=?                                                     //
// Copyright (c) 2010-2011 Agridea, All Rights Reserved                        //
// </auto-generated>                                                           //
//                                                                             //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
//using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Agridea;
using Agridea.Calendar;
using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.Metadata;
using Agridea.News;
using Agridea.Security;
using Agridea.Service;
using Agridea.Web.UI;

namespace Agridea.Calendar
{
    public class DomainModelMetadata
    {
        public IList<MetadataEntity> MetadataEntityList;
    	private IDictionary<string, MetadataEntity> metadataEntityForGuid_;
    	private MetadataEntity metadataEntity_;
    	private MetadataEntity neededMetadataEntity_;
    	private MetadataEntity needingMetadataEntity_;
    	private MetadataField metadataField_;
    	private MetadataNavigationProperty metadataNavigationProperty_;
    
    	public DomainModelMetadata()
        {
            MetadataEntityList = new List<MetadataEntity>();
    		metadataEntityForGuid_ = new Dictionary<string, MetadataEntity>();
    
    	    //No warnings
            if (metadataEntity_ == null) metadataEntity_ = null;
            if (neededMetadataEntity_ == null) neededMetadataEntity_ = null;
            if (needingMetadataEntity_ == null) needingMetadataEntity_ = null;
            if (metadataField_ == null) metadataField_ = null;
            if (metadataNavigationProperty_ == null) metadataNavigationProperty_ = null;
    
    		//Pass1 : find entities
            metadataEntity_ = new MetadataEntity
    		{
    		    Guid = "476b1ee2-94aa-454c-990a-f0a47a664e65",
    			Name = "CalendarEvent", 
    			Dimension = false,
    		};
    		MetadataEntityList.Add(metadataEntity_);
    		metadataEntityForGuid_.Add(metadataEntity_.Guid, metadataEntity_);
    
            metadataField_ = new MetadataField
    		{
    		    Guid = "7dd91293-3b6b-40b8-86ed-c43beb3bf2d0",
    			Name = "AppliesTo", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "e3370f1e-eb31-434a-b0f0-4cde41895de0",
    			Name = "Comment", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "f7a4aefb-fc2a-454f-b9b6-9e29e3b64943",
    			Name = "Description", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "9becb296-458b-437a-8cda-7cb1afb453f3",
    			Name = "EndDate", 
    			Type = "DateTime",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "dfb29f01-74aa-436d-a51a-9e7fd0bb1d36",
    			Name = "FileData", 
    			Type = "Binary",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "d316fc2e-4de4-482f-91fc-c224310882ef",
    			Name = "FileName", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "d4c2b107-a62d-4335-9dbf-aeddda69f571",
    			Name = "FileType", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "c67653a0-75fb-4035-ba40-0a8af88136e4",
    			Name = "Owner", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "b0b2bf9f-19f7-4aa0-9b76-86735e36d22a",
    			Name = "StartDate", 
    			Type = "DateTime",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "f2574de4-bd49-4520-8d26-0376b612d5a2",
    			Name = "Title", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "4424a6e4-9e7b-4eb0-ae67-ff5655c83fbd",
    			Name = "Recurrence", 
    			Type = "CalendarEventRecurrence",
    			FromMultiplicity = MultiplicityTypes.ZeroOrMany,
    			ToMultiplicity = MultiplicityTypes.OneExactly,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "01a265a8-efb0-4095-9790-d042fe285fa4",
    			Name = "Status", 
    			Type = "CalendarEventStatus",
    			FromMultiplicity = MultiplicityTypes.ZeroOrMany,
    			ToMultiplicity = MultiplicityTypes.OneExactly,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataEntity_ = new MetadataEntity
    		{
    		    Guid = "4a2d41a8-ce43-49d4-8577-0812b4e7cc0f",
    			Name = "CalendarEventRecurrence", 
    			Dimension = false,
    		};
    		MetadataEntityList.Add(metadataEntity_);
    		metadataEntityForGuid_.Add(metadataEntity_.Guid, metadataEntity_);
    
            metadataField_ = new MetadataField
    		{
    		    Guid = "21df7957-9b6e-4c8c-810a-43ce9eb99dd0",
    			Name = "Code", 
    			Type = "Int32",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "876ecb5a-bb36-4ac5-b61f-1629ba436f63",
    			Name = "Description", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataEntity_ = new MetadataEntity
    		{
    		    Guid = "251be393-8a31-42bb-9ed4-a58db1cae6db",
    			Name = "CalendarEventStatus", 
    			Dimension = false,
    		};
    		MetadataEntityList.Add(metadataEntity_);
    		metadataEntityForGuid_.Add(metadataEntity_.Guid, metadataEntity_);
    
            metadataField_ = new MetadataField
    		{
    		    Guid = "94b684ff-3e4b-449f-af82-24c6c052adfe",
    			Name = "Code", 
    			Type = "Int32",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "b0d1316d-4a9e-4f85-a7c6-a993b84fba0c",
    			Name = "Description", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
    
        	//Pass2 : add entity usages
    		metadataEntity_ = metadataEntityForGuid_["476b1ee2-94aa-454c-990a-f0a47a664e65"];  //CalendarEvent
    		//Needed entities
    		    neededMetadataEntity_ = metadataEntityForGuid_["4a2d41a8-ce43-49d4-8577-0812b4e7cc0f"]; //CalendarEventRecurrence
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		    neededMetadataEntity_ = metadataEntityForGuid_["251be393-8a31-42bb-9ed4-a58db1cae6db"]; //CalendarEventStatus
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		//Needing entities
    		metadataEntity_ = metadataEntityForGuid_["4a2d41a8-ce43-49d4-8577-0812b4e7cc0f"];  //CalendarEventRecurrence
    		//Needed entities
    		//Needing entities
    		    needingMetadataEntity_ = metadataEntityForGuid_["476b1ee2-94aa-454c-990a-f0a47a664e65"]; //CalendarEvent
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
    		metadataEntity_ = metadataEntityForGuid_["251be393-8a31-42bb-9ed4-a58db1cae6db"];  //CalendarEventStatus
    		//Needed entities
    		//Needing entities
    		    needingMetadataEntity_ = metadataEntityForGuid_["476b1ee2-94aa-454c-990a-f0a47a664e65"]; //CalendarEvent
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
        }
    }
}

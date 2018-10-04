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

namespace Agridea.Metadata
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
    		    Guid = "BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC",
    			Name = "MetadataEntity", 
    			Dimension = false,
    		};
    		MetadataEntityList.Add(metadataEntity_);
    		metadataEntityForGuid_.Add(metadataEntity_.Guid, metadataEntity_);
    
            metadataField_ = new MetadataField
    		{
    		    Guid = "92a06d16-ae46-4559-9b9a-4e3eda0cf99d",
    			Name = "Dimension", 
    			Type = "Boolean",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "e1dac6a9-773b-4bf0-9e78-b6f79cd09553",
    			Name = "Documentation", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "9fa42292-5172-461e-8374-bcba3345b292",
    			Name = "FileContent", 
    			Type = "Binary",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "428eeadb-642d-4e79-811f-0ad7f9c4d949",
    			Name = "FileName", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "e228cdb7-19a3-49ae-a3ec-1bd4730c3b9c",
    			Name = "Guid", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "4012d58b-1047-49a4-8107-e649461e64b6",
    			Name = "Name", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "3941b554-fbb4-4611-b679-779be113c101",
    			Name = "Url", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "07c2a20c-4945-4cc3-b52f-6fafe99b1083",
    			Name = "MetadataFieldList", 
    			Type = "MetadataField",
    			FromMultiplicity = MultiplicityTypes.OneExactly,
    			ToMultiplicity = MultiplicityTypes.ZeroOrMany,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "8f7f911e-0841-4165-8930-239df7d81504",
    			Name = "MetadataNavigationPropertyList", 
    			Type = "MetadataNavigationProperty",
    			FromMultiplicity = MultiplicityTypes.OneExactly,
    			ToMultiplicity = MultiplicityTypes.ZeroOrMany,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "2e999fb4-8b9f-4c87-871b-6ad7a1cc255b",
    			Name = "UsedEntityList", 
    			Type = "MetadataEntity",
    			FromMultiplicity = MultiplicityTypes.ZeroOrMany,
    			ToMultiplicity = MultiplicityTypes.ZeroOrMany,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "b5508404-99bd-402d-beaa-c2a64a99306a",
    			Name = "UsingEntityList", 
    			Type = "MetadataEntity",
    			FromMultiplicity = MultiplicityTypes.ZeroOrMany,
    			ToMultiplicity = MultiplicityTypes.ZeroOrMany,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataEntity_ = new MetadataEntity
    		{
    		    Guid = "0DA463DF-3EDE-4751-BA3B-AAC8DE43D0C5",
    			Name = "MetadataField", 
    			Dimension = false,
    		};
    		MetadataEntityList.Add(metadataEntity_);
    		metadataEntityForGuid_.Add(metadataEntity_.Guid, metadataEntity_);
    
            metadataField_ = new MetadataField
    		{
    		    Guid = "cd2e4e02-ff4b-4fe7-a13b-8380e30e1d05",
    			Name = "Documentation", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "9cb09826-e135-4147-a138-942fc8b9e6f2",
    			Name = "Guid", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "09445e33-a4cf-4bf1-942d-725933642ad0",
    			Name = "Name", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "bfec8376-c6f2-44cd-8fe2-67fb23502a46",
    			Name = "Type", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "e3f4192c-02d4-462e-9a5c-56241fca2b9a",
    			Name = "MetadataEntity", 
    			Type = "MetadataEntity",
    			FromMultiplicity = MultiplicityTypes.ZeroOrMany,
    			ToMultiplicity = MultiplicityTypes.OneExactly,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
            metadataEntity_ = new MetadataEntity
    		{
    		    Guid = "5732AB87-C974-4C95-B984-64B4AB9F4730",
    			Name = "MetadataNavigationProperty", 
    			Dimension = false,
    		};
    		MetadataEntityList.Add(metadataEntity_);
    		metadataEntityForGuid_.Add(metadataEntity_.Guid, metadataEntity_);
    
            metadataField_ = new MetadataField
    		{
    		    Guid = "fc033011-185b-4e5f-ba2f-2a1c193f83ae",
    			Name = "Documentation", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "eeb1799a-e22b-44e1-843a-f573555cd07e",
    			Name = "FromMultiplicity_", 
    			Type = "Int32",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "6a64d848-9903-4c03-9cad-bd4abac44ae5",
    			Name = "Guid", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "56bd552b-8eac-4268-8f1c-f4e480f87a81",
    			Name = "Name", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "56bd728d-53d8-45af-b40f-57f658520f24",
    			Name = "ToMultiplicity_", 
    			Type = "Int32",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataField_ = new MetadataField
    		{
    		    Guid = "471a0375-f1e9-4d66-afce-f725b7e155fb",
    			Name = "Type", 
    			Type = "String",
    		};
    		metadataEntity_.MetadataFieldList.Add(metadataField_);
            metadataNavigationProperty_ = new MetadataNavigationProperty
    		{
    		    Guid = "c562682d-2649-4437-a26e-8c74d4f7e7c9",
    			Name = "MetadataEntity", 
    			Type = "MetadataEntity",
    			FromMultiplicity = MultiplicityTypes.ZeroOrMany,
    			ToMultiplicity = MultiplicityTypes.OneExactly,
    		};
    		metadataEntity_.MetadataNavigationPropertyList.Add(metadataNavigationProperty_);
    
        	//Pass2 : add entity usages
    		metadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"];  //MetadataEntity
    		//Needed entities
    		    neededMetadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"]; //MetadataEntity
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		    neededMetadataEntity_ = metadataEntityForGuid_["0DA463DF-3EDE-4751-BA3B-AAC8DE43D0C5"]; //MetadataField
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		    neededMetadataEntity_ = metadataEntityForGuid_["5732AB87-C974-4C95-B984-64B4AB9F4730"]; //MetadataNavigationProperty
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		//Needing entities
    		    needingMetadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"]; //MetadataEntity
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
    		    needingMetadataEntity_ = metadataEntityForGuid_["0DA463DF-3EDE-4751-BA3B-AAC8DE43D0C5"]; //MetadataField
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
    		    needingMetadataEntity_ = metadataEntityForGuid_["5732AB87-C974-4C95-B984-64B4AB9F4730"]; //MetadataNavigationProperty
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
    		metadataEntity_ = metadataEntityForGuid_["0DA463DF-3EDE-4751-BA3B-AAC8DE43D0C5"];  //MetadataField
    		//Needed entities
    		    neededMetadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"]; //MetadataEntity
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		//Needing entities
    		    needingMetadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"]; //MetadataEntity
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
    		metadataEntity_ = metadataEntityForGuid_["5732AB87-C974-4C95-B984-64B4AB9F4730"];  //MetadataNavigationProperty
    		//Needed entities
    		    neededMetadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"]; //MetadataEntity
    			metadataEntity_.UsedEntityList.Add(neededMetadataEntity_);
    		//Needing entities
    		    needingMetadataEntity_ = metadataEntityForGuid_["BE885A9D-4BCB-40F0-B9CD-CDD00E847EFC"]; //MetadataEntity
    			metadataEntity_.UsingEntityList.Add(needingMetadataEntity_);
        }
    }
}

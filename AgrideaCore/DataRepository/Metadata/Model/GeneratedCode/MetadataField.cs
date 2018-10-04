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
    [Serializable]
    public partial class MetadataField : PocoBase, IEquatable<MetadataField>
    {
        #region Primitive Properties
        public virtual string Documentation { get; set;}
        public virtual string Guid { get; set;}
        public virtual string Name { get; set;}
        public virtual string Type { get; set;}

        #endregion

     
        #region Navigation Properties
        [Mandatory]
        public virtual MetadataEntity MetadataEntity  { get; set; } //MetadataField *<==>1 MetadataEntity

        #endregion

     
        #region Initialization
        public void CopyTo(MetadataField other)
        {
            other.Documentation = Documentation;
            other.Guid = Guid;
            other.Name = Name;
            other.Type = Type;
        }
        #endregion

     
        #region Identity
        public override string ToString()
        {
            return string.Format("[{0} Documentation='{1}' Guid='{2}' Name='{3}' Type='{4}' MetadataEntity='{5}']",
                base.ToString(),
                Documentation,
                Guid,
                Name,
                Type,
                MetadataEntity == null ? "@null" : string.Format("{0}", "no primitive property discriminant"));
        }
        public override int GetHashCode() { return base.GetHashCode(); }
    	public string NaturalKey()
    	{ 
    	    return 
                "No natural Key";
        }
        public override bool Equals(object other)
        {
            return Equals(other as MetadataField);
        }
        public bool Equals(MetadataField other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(this, other)) return true;
            if (!Id.Equals(other.Id)) return false;
            if (!Id.Equals(0) && Id.Equals(other.Id)) return true;
            return false;
        } 

        #endregion

     
    }
}

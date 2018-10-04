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

namespace Agridea.Security
{
    [Reference]
    [Serializable]
    public partial class Permission : PocoBase, IEquatable<Permission>
    {
        #region Primitive Properties
        public virtual string Description { get; set;}
        [Discriminant]
        [MaxLength(128)]
        public virtual string ItemName { get; set;}

        #endregion

     
        #region Navigation Properties
        [Discriminant]
        public virtual Role Role  { get; set; } //Permission *<==>1 Role

        #endregion

     
        #region Initialization
        public void CopyTo(Permission other)
        {
            other.Description = Description;
            other.ItemName = ItemName;
        }
        #endregion

     
        #region Identity
        public override string ToString()
        {
            return string.Format("[{0} Description='{1}' ItemName='{2}' Role.Name='{3}']",
                base.ToString(),
                Description,
                ItemName,
                Role == null ? "@null" : string.Format("{0}", Role.Name));
        }
        public override int GetHashCode() { return base.GetHashCode(); }
    	public string NaturalKey()
    	{ 
    	    return 
                ItemName + 
                Role.Name;
        }
        public override bool Equals(object other)
        {
            return Equals(other as Permission);
        }
        public bool Equals(Permission other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(this, other)) return true;
            if (!Id.Equals(other.Id)) return false;
            if (!Id.Equals(0) && Id.Equals(other.Id)) return true;
            return ItemName == other.ItemName &&
                   Role == other.Role;
        } 

        #endregion

     
    }
}

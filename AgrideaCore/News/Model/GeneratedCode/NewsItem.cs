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

namespace Agridea.News
{
    [Serializable]
    public partial class NewsItem : PocoBase, IEquatable<NewsItem>
    {
        #region Primitive Properties
        public virtual string Comment { get; set;}
        public virtual string Description { get; set;}
        public virtual byte[] FileData { get; set;}
        public virtual string FileName { get; set;}
        public virtual string FileType { get; set;}
        public virtual string LinkUrl { get; set;}
        public virtual string Title { get; set;}
        public virtual Nullable<System.DateTime> ValidityDateEnd { get; set;}
        public virtual Nullable<System.DateTime> ValidityDateStart { get; set;}

        #endregion

     
        #region Navigation Properties
        //None(no navigation property)
        #endregion

     
        #region Initialization
        public void CopyTo(NewsItem other)
        {
            other.Comment = Comment;
            other.Description = Description;
            other.FileData = FileData;
            other.FileName = FileName;
            other.FileType = FileType;
            other.LinkUrl = LinkUrl;
            other.Title = Title;
            other.ValidityDateEnd = ValidityDateEnd;
            other.ValidityDateStart = ValidityDateStart;
        }
        #endregion

     
        #region Identity
        public override string ToString()
        {
            return string.Format("[{0} Comment='{1}' Description='{2}' FileData='{3}' FileName='{4}' FileType='{5}' LinkUrl='{6}' Title='{7}' ValidityDateEnd='{8}' ValidityDateStart='{9}']",
                base.ToString(),
                Comment,
                Description,
                FileData,
                FileName,
                FileType,
                LinkUrl,
                Title,
                ValidityDateEnd,
                ValidityDateStart);
        }
        public override int GetHashCode() { return base.GetHashCode(); }
    	public string NaturalKey()
    	{ 
    	    return 
                "No natural Key";
        }
        public override bool Equals(object other)
        {
            return Equals(other as NewsItem);
        }
        public bool Equals(NewsItem other)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Agridea.DataRepository;
using FluentValidation;

namespace Agridea.Web.Mvc
{

    public interface IAutoComplete<TPoco> where TPoco : class, IPocoBase
    {
        int Id { get; set; }
        string DisplayName { get; }
        string Url { get; set; }
        string IdPropertyName { get; set; }

        IOrderedQueryable<TPoco> Filter(IQueryable<TPoco> source, string searchExpression);

    }
    [DataContract]
    public abstract class AutoCompleteBase<TPoco> : IAutoComplete<TPoco> where TPoco : class, IPocoBase
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "text")]
        public abstract string DisplayName { get; }

        public string Url { get; set; }

        public string IdPropertyName { get; set; }

        public abstract IOrderedQueryable<TPoco> Filter(IQueryable<TPoco> source, string searchExpression);


    }





}

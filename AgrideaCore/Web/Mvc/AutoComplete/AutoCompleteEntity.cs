using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Agridea.DataRepository;

namespace Agridea.Web.Mvc
{
    [DataContract]
    public class AutoCompleteEntity<TPoco, T> where TPoco : class, IPocoBase
            where T : class, IAutoComplete<TPoco>, new()
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }
        [DataMember(Name = "results")]
        public IEnumerable<T> Results { get; set; }
    }
}

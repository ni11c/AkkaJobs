using System;
using System.Collections.Generic;
using Agridea.Diagnostics.Contracts;

namespace Agridea.ObjectMapping
{
    public class Map
    {
        #region Members
        private Dictionary<PropertyPath, PropertyPath> map_;
        #endregion

        #region Initialization
        public Map()
        {
            map_ = new Dictionary<PropertyPath, PropertyPath>();
        }
        #endregion

        #region Services
        public void Add(PropertyPath sourcePropertyPath, PropertyPath targetPropertyPath)
        {
            Asserts<ArgumentNullException>.IsNotNull(sourcePropertyPath);
            Asserts<ArgumentNullException>.IsNotNull(targetPropertyPath);

            map_.Add(targetPropertyPath, sourcePropertyPath);
        }
        public void Remove(PropertyPath targetPropertyPath)
        {
            Asserts<ArgumentNullException>.IsNotNull(targetPropertyPath);

            map_.Remove(targetPropertyPath);
        }

        public void Clear()
        {
            map_.Clear();
        }

        public void Replace(PropertyPath sourcePropertyPath, PropertyPath targetPropertyPath)
        {
            Asserts<ArgumentNullException>.IsNotNull(sourcePropertyPath);
            Asserts<ArgumentNullException>.IsNotNull(targetPropertyPath);

            map_.Remove(targetPropertyPath);
            Add(sourcePropertyPath, targetPropertyPath);
        }
        public PropertyPath Get(PropertyPath targetPropertyPath)
        {
            Asserts<ArgumentNullException>.IsNotNull(targetPropertyPath);

            return !map_.ContainsKey(targetPropertyPath)
                ? null
                : map_[targetPropertyPath];
        }
        public IEnumerable<PropertyPath> Keys
        {
            get { return map_.Keys; }
        }
        #endregion
    }
}

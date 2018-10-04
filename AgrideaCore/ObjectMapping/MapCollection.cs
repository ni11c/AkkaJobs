using System;
using System.Collections;
using System.Collections.Generic;
using Agridea.Diagnostics.Contracts;

namespace Agridea.ObjectMapping
{
    public class MapCollection : IEnumerable<Map>
    {
        #region Members
        public  Dictionary<Tuple<Type, Type>, Map> Maps { get; private set; }
        #endregion

        #region Initialization
        public MapCollection()
        {
            Maps = new Dictionary<Tuple<Type, Type>, Map>();
        }
        #endregion

        #region Services
        IEnumerator<Map> IEnumerable<Map>.GetEnumerator()
        {
            return Maps.Values.GetEnumerator();
        }
        public IEnumerator GetEnumerator()
        {
            return Maps.Values.GetEnumerator();
        }
        public void Add(Type sourceType, Type targetType, Map map)
        {
            Asserts<ArgumentNullException>.IsNotNull(sourceType);
            Asserts<ArgumentNullException>.IsNotNull(targetType);
            Asserts<ArgumentNullException>.IsNotNull(map);

            Maps.Add(CreateTupleFor(sourceType, targetType), map);
        }
        public bool MapExists(Type sourceType, Type targetType)
        {
            Asserts<ArgumentNullException>.IsNotNull(sourceType);
            Asserts<ArgumentNullException>.IsNotNull(targetType);

            var tuple = CreateTupleFor(sourceType, targetType);
            return Maps.ContainsKey(tuple);
        }
        public Map this[Type sourceType, Type targetType]
        {
            get
            {
                Asserts<ArgumentNullException>.IsNotNull(sourceType);
                Asserts<ArgumentNullException>.IsNotNull(targetType);

                if (!MapExists(sourceType, targetType)) return null;
                return Maps[CreateTupleFor(sourceType, targetType)];
            }
        }
        #endregion

        #region Helpers
        private Tuple<Type, Type> CreateTupleFor(Type sourceType, Type targetType)
        {
            return Tuple.Create(KeyForType(sourceType), KeyForType(targetType));
        }
        private Type KeyForType(Type type)
        {
            var key = type.Name.IndexOf("_") > -1 ? type.BaseType : type; //for EF proxied types
            return key;
        }
        #endregion
    }
}

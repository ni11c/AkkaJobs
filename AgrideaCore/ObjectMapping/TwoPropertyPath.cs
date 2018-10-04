using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Agridea.Diagnostics.Contracts;
using Agridea.Service;

namespace Agridea.ObjectMapping
{
    public class TwoPropertyPath : PropertyPath
    {
        #region Members
        private string path2_;
        private IList<PropertyInfo> propertyInfos2_;
        private Func<object, object, object> computation_;
        #endregion

        #region Initialization
        public TwoPropertyPath(Type type, string path1, IList<PropertyInfo> propertyInfos1, string path2, IList<PropertyInfo> propertyInfos2, Func<object, object, object> computation)
            : base(type, path1, propertyInfos1)
        {
            Asserts<InvalidOperationException>.IsNotNull(type);
            Asserts<InvalidOperationException>.IsNotNull(path1);
            Asserts<InvalidOperationException>.IsNotNull(propertyInfos1);
            Asserts<InvalidOperationException>.IsNotNull(path2);
            Asserts<InvalidOperationException>.IsNotNull(computation);

            path2_ = path2;
            propertyInfos2_ = propertyInfos2;
            computation_ = computation;
        }
        #endregion

        #region Services
        public override string ToString()
        {
            return string.Format("[{0} Path='{1}']", GetType().Name, Path);
        }
        public override string Path
        {
            get { return string.Format("{0};{1}", path_, path2_); }
        }
        public override void SetValue<TTarget>(object target, object source, PropertyPath targetPropertyPath, Mapper mapper, IService service, params Expression<Func<TTarget, object>>[] collectionsOwningItems)
        {
            Asserts<InvalidOperationException>.IsNotNull(target);
            Asserts<InvalidOperationException>.IsNotNull(source);
            Asserts<InvalidOperationException>.IsNotNull(collectionsOwningItems);

            bool nullInChain1 = false;
            object sourceValue1 = GetValue(source, propertyInfos_, out nullInChain1);
            if (sourceValue1 == null) return; //Accomodate nulls
            bool nullInChain2 = false;
            object sourceValue2 = GetValue(source, propertyInfos2_, out nullInChain2);
            if (sourceValue2 == null) return; //Accomodate nulls
            object targetInstance = GetInstance(target, targetPropertyPath);

            DoSetValue(targetInstance, sourceValue1, sourceValue2, targetPropertyPath, mapper, service);
        }
        #endregion

        #region Members
        private void DoSetValue(object target, object source1, object source2, PropertyPath propertyPath, Mapper mapper, IService service)
        {
            Asserts<InvalidOperationException>.IsNotNull(target);
            Asserts<InvalidOperationException>.IsNotNull(source1);
            Asserts<InvalidOperationException>.IsNotNull(source2);
            Asserts<InvalidOperationException>.IsNotNull(propertyPath);
            Asserts<InvalidOperationException>.IsNotNull(mapper);

            //The target object chain has been walked thru, recreated by GetInstance
            PropertyInfo propertyInfo = propertyPath.LastPropertyInfo;
            if (propertyInfo.PropertyType.IsGenericList())
            {
                var collectionInstance1 = CreateCollection(source1, propertyInfo, mapper, service);
                var collectionInstance2 = CreateCollection(source2, propertyInfo, mapper, service);
                propertyInfo.GetSetMethod().Invoke(target, new object[] { computation_(collectionInstance1, collectionInstance2) });
            }
            else
                DoSetValue(target, propertyInfo, computation_(source1, source2));
        }
        #endregion
    }
}

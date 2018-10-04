using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agridea.Web.Mvc
{
    public class AcceptNullAsZeroModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            if (value != null)
            {
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
                return;
            }

            if (propertyDescriptor.PropertyType == typeof(int))
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, 0);
            else if (propertyDescriptor.PropertyType == typeof(uint))
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, 0);
            else if (propertyDescriptor.PropertyType == typeof(float))
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, (float)0);
            else if (propertyDescriptor.PropertyType == typeof(double))
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, (double)0);
            else if (propertyDescriptor.PropertyType == typeof(decimal))
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, 0M);
        }
    }
}

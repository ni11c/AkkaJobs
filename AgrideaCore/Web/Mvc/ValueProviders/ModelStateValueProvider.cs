using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc
{
    public class ModelStateValueProvider : IValueProvider
    {
        #region Members
        HashSet<string> prefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        ModelStateDictionary modelState;
        ControllerContext controllerContext;
        #endregion

        #region Initialization
        public ModelStateValueProvider(ControllerContext controllerContext)
        {
            Requires<ArgumentNullException>.IsNotNull(controllerContext, "controllerContext");

            this.controllerContext = controllerContext;
            this.modelState = controllerContext.Controller.ViewData.ModelState;

            FindPrefixes();
        }
        #endregion

        #region IValueProvider
        public bool ContainsPrefix(string prefix)
        {
            Requires<ArgumentNullException>.IsNotNull(prefix, "prefix");

            return prefixes.Contains(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            Requires<ArgumentNullException>.IsNotNull(key, "key");

            return modelState.ContainsKey(key) ? modelState[key].Value : null;
        }
        #endregion

        #region Helpers
        private void FindPrefixes()
        {
            if (modelState.Count > 0)
                prefixes.Add(string.Empty);

            foreach (var modelStateEntry in modelState)
                prefixes.UnionWith(GetPrefixes(modelStateEntry.Key));
        }
        static IEnumerable<string> GetPrefixes(string key)
        {
            yield return key;
            for (int i = key.Length - 1; i >= 0; i--)
            {
                switch (key[i])
                {
                    case '.':
                    case '[':
                        yield return key.Substring(0, i);
                        break;
                }
            }
        }
        #endregion
    }

    public class ModelStateValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new ModelStateValueProvider(controllerContext);
        }
    }
}
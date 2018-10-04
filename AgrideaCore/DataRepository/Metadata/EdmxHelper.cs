
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
namespace Agridea.DataRepository.Metadata
{
    public class EdmxHelper
    {
        #region Members
        readonly string EdmxNameSpace = "http://schemas.microsoft.com/ado/2009/11/edmx";
        readonly string EdmNameSpace = "http://schemas.microsoft.com/ado/2009/11/edm";
        #endregion

        #region Services
        public void InsertGuidIntoDocumentationSummary(string fullPath, bool superseed = false)
        {
            var xDoc = XDocument.Load(fullPath);
            foreach (var entityTypeElement in
                xDoc.Element(XName.Get("Edmx", EdmxNameSpace)).
                     Element(XName.Get("Runtime", EdmxNameSpace)).
                     Element(XName.Get("ConceptualModels", EdmxNameSpace)).
                     Element(XName.Get("Schema", EdmNameSpace)).
                     Elements(XName.Get("EntityType", EdmNameSpace)))
            {
                HandleGuid(entityTypeElement, superseed);

                var propertyElements = entityTypeElement.Elements(XName.Get("Property", EdmNameSpace)); //.Where(x => x.Name != "Id");
                foreach (var propertyElement in propertyElements)
                    HandleGuid(propertyElement, superseed);

                var navigationPropertyElements = entityTypeElement.Elements(XName.Get("NavigationProperty", EdmNameSpace));
                foreach (var navigationPropertyElement in navigationPropertyElements)
                    HandleGuid(navigationPropertyElement, superseed);
            }
            xDoc.Save(fullPath);
        }

        #endregion

        #region Helpers
        private void HandleGuid(XElement entityTypeElement, bool superseed)
        {
            var documentationName = XName.Get("Documentation", EdmNameSpace);
            var summaryName = XName.Get("Summary", EdmNameSpace);

            var documentationElement = entityTypeElement.Element(documentationName);
            if (documentationElement == null)
                entityTypeElement.AddFirst(new XElement(documentationName));
            documentationElement = entityTypeElement.Element(documentationName);
            if (documentationElement.Element(summaryName) == null)
                documentationElement.AddFirst(new XElement(summaryName));

            var summaryElement = documentationElement.Element(summaryName);
            if (superseed || !ContainsAGuid(summaryElement.Value))
                summaryElement.Value = Guid.NewGuid().ToString();
        }

        private bool ContainsAGuid(string s)
        {
            //415E6CB3-B6BC-4636-8F60-E98EA43649ED
            if (string.IsNullOrEmpty(s)) 
                return false;
            var containsAGuid = Regex.Match(s, @"\w\w\w\w\w\w\w\w-\w\w\w\w-\w\w\w\w-\w\w\w\w-\w\w\w\w\w\w\w\w\w\w\w\w").Success;
            return containsAGuid;
        }
        #endregion
    }
}

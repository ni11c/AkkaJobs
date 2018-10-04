using System;

namespace Agridea.Service
{
    public static class Tools
    {
        #region Services
        public static string GenerateSearchDTOFor(Type type)
        {
            string className = string.Format("Search{0}DTO", type.Name);
            string dto = Line("public class {0}", className);
            dto += Line("{{");

            dto += Line("    #region Initialization");
            dto += Line("    public {0}()", className);
            dto += Line("   {{");
            foreach (var property in type.GetReferenceProperties())
            {
                dto += Line("       {0} = new {1}();", property.Name, property.PropertyType.Name);
            }
            dto += Line("   }}");
            dto += Line("    #endregion");
            dto += Line("");

            dto += Line("    #region Properties (Input)");
            foreach (var property in type.GetPrimitiveProperties())
            {
                string typeName = property.PropertyType.IsNullableType() || property.PropertyType.IsValueType ?
                    string.Format("Nullable<{0}>", property.PropertyType.GetNonNullableType().Name) :
                    string.Format("{0}", property.PropertyType.Name);
                dto += Line("    public {0} {1} {{ get; set; }}", typeName, property.Name);
            }
            dto += Line("");

            foreach (var property in type.GetReferenceProperties())
            {
                dto += Line("    public {0} {1} {{ get; set; }}", property.PropertyType.Name, property.Name);
            }
            dto += Line("    #endregion");
            dto += Line("");

            //foreach (var property in type.GetCollectionProperties())
            //{
            //    dto += Line("    //public IList<{0}> {1} {{ get; set; }}", property.PropertyType.GetGenericArguments()[0].Name, property.Name);
            //}
            //dto += Line("    #endregion");
            //dto += Line("");

            dto += Line("    #region Properties (Output)");
            dto += Line("    public IList<{0}> {0}List {{ get; set; }}", type.Name);
            dto += Line("    #endregion");
            dto += Line("");

            dto += Line("}}");
            return dto;
        }
        public static string GenerateSearchServiceFor(Type type)
        {
            string service = Line("    public IList<{0}> GetAll{0}For(Search{0}DTO searchDTO, string ordering, out int totalCount)", type.Name);
            service += Line("{{");
            service += Line("    return DoGetAll{0}For(searchDTO, out totalCount)", type.Name);
            service += Line("        .OrderBy(ordering)");
            service += Line("        .ToList();");
            service += Line("}}");

            service += Line("    public IList<{0}> GetAll{0}For(Search{0}DTO searchDTO, string ordering, int skipCount, int takeCount, out int totalCount)", type.Name);
            service += Line("{{");
            service += Line("    return DoGetAll{0}For(searchDTO, out totalCount)", type.Name);
            service += Line("        .OrderBy(ordering)");
            service += Line("        .Skip(skipCount)");
            service += Line("        .Take(takeCount)");
            service += Line("        .ToList();");
            service += Line("}}");

            service += Line("    private IQueryable<{0}> DoGetAll{0}For(", type.Name);
            service += Line("        Search{0}DTO searchDTO,", type.Name);
            service += Line("        out int totalCount)");
            service += Line("{{");
            service += Line("    var allItems = Repository.All<{0}>();", type.Name);
            service += Line("");

            foreach (var property in type.GetPrimitiveProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    service += Line("    if(!string.IsNullOrWhiteSpace(searchDTO.{0}))", property.Name);
                    service += Line("        allItems = allItems.Where(m => m.{0}.ToUpper().Contains(searchDTO.{0}.ToUpper()));", property.Name);
                    continue;
                }
                if (property.PropertyType.IsNullableType() || property.PropertyType.IsValueType)
                {
                    service += Line("    if(searchDTO.{0}.HasValue)", property.Name);
                    service += Line("        allItems = allItems.Where(m => m.{0} == searchDTO.{0}.Value);", property.Name);
                    continue;
                }

                service += Line("    //{0}:{1} could not generate filtering", property.Name, property.PropertyType.Name);
            }
            service += Line("");

            foreach (var property in type.GetReferenceProperties())
            {
                service += Line("    if(searchDTO.{0}.Id > 0)", property.Name);
                service += Line("        allItems = allItems.Where(m => m.{0}.Id == searchDTO.{0}.Id);", property.Name);
            }
            service += Line("");

            service += Line("    totalCount = allItems.Count();");
            service += Line("    return allItems;");
            service += Line("}}");

            return service;
        }
        #endregion

        #region Helpers
        private static string Line(string format, params object[] args)
        {
            return string.Format("{0}{1}", string.Format(format, args), Environment.NewLine);
        }
        #endregion
    }
}
using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace Agridea.Security
{
    /// <summary>
    /// Permissions are defined with Permission entity which expose a string ItemName
    /// The ItemName is used to store (MVC applications) Entity/Controller names and
    /// Property names.
    /// These can come from various MVC sites (e.g. Acorda2Admin, Acorda2Recensement).
    /// Hence, name collisions could occur. Therefore, the names must be unambiguous.
    /// The more general scheme would be AssemblyName+Namespace+ClassName+PropertyName.
    /// E.g. Acorda2Recensement.Agridea.Acorda.Acorda2.Person.Ktidb.
    /// The name need to be easily parseable per se and distinguish in particular in :
    /// - Acorda2Recensement.Global.Gate
    /// whether Global is a class (entity/controller) name and Gate is a property or
    /// whether Global is a namespace and Gate a class...
    /// Pragmatic solution :
    /// - AssemblyName.Controller/Entity.Property, namespace is ignored
    /// Cons : there could be collision names, but this is little probable that two 
    /// controllers of the same site be called the same
    /// Pros : the item names are always composed of two or three parts
    /// - two : AssemblyName.ControllerName
    /// - three : AssemblyName.ControllerName.PropertyName
    /// </summary>
    public static class PermissionHelper
    {
        #region Members
        public const string Dot = ".";
        private static readonly ConcurrentDictionary<Type, string> ControllerDictionary = new ConcurrentDictionary<Type, string>();
        #endregion

        #region Services
        public static bool IsPropertyName(string itemName)
        {
            return itemName.LastIndexOf(Dot) - itemName.IndexOf(Dot) > 2;
        }
        public static string ItemNameForProperty<TItem, TProperty>(Expression<Func<TItem, TProperty>> expression) where TItem : PocoBase
        {
            var member = (expression.Body as MemberExpression).Member;
            return ItemNameForProperty(ItemNameForEntityType(member.DeclaringType), member.Name);
        }


        public static string ItemNameForController<TController>() where TController : ControllerBase
        {
            return ControllerDictionary.GetOrAdd(typeof(TController), ItemNameForControllerType);
        }
        public static string ItemNameForController(Type controller)
        {
            return ControllerDictionary.GetOrAdd(controller, ItemNameForControllerType);
        }
        public static string ItemNameForApiController<TController>() where TController : ApiController
        {
            return ItemNameForControllerType(typeof(TController));
        }

        public static string ItemNameForAction<TController>(Expression<Func<TController, ActionResult>> action) where TController : ControllerBase
        {
            return ItemNameForElement(ItemNameForController<TController>(), ActionNameFor(action));
        }
        public static string ItemNameForApiAction<TController>(Expression<Func<TController, object>> action) where TController : ApiController
        {
            return ItemNameForElement(ItemNameForApiController<TController>(), ActionNameForApi(action));
        }

        public static IList<string> GetItemNamesForControllersInSameAssemblyAs<TController>() where TController : Controller
        {
            return ItemNamesForControllersInSameAssemblyAs<TController>();
        }
        public static IList<string> GetItemNamesForController<TController>() where TController : Controller
        {
            return ItemNamesForController(typeof(TController));
        }
        public static IList<string> GetItemNamesForApiControllersInSameAssemblyAs<TController>() where TController : ApiController
        {
            return ItemNamesForApiControllersInSameAssemblyAs<TController>();
        }

        #endregion

        #region Internal Untyped Services (dont expose outside AgrideaCore !!!)
        public static string ItemNameForProperty(string entityName, string propertyName)
        {
            return ItemNameForElement(entityName, propertyName);
        }
        public static string EntityName(string itemName)
        {
            Asserts<ArgumentException>.Contains(itemName, Dot);
            if (IsPropertyName(itemName))
                return itemName.Substring(0, itemName.LastIndexOf(Dot));
            return itemName;
        }
        internal static string PropertyName(string itemName)
        {
            Asserts<ArgumentException>.IsTrue(IsPropertyName(itemName));
            return itemName.Substring(itemName.LastIndexOf(Dot) + 1);
        }
        #endregion

        #region Helpers
        private static List<string> ItemNamesForControllersInSameAssemblyAs<TController>()
        {
            var itemNames = new List<string>();
            Assembly assembly = Assembly.GetAssembly(typeof(TController));
            var controllers = assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ControllerBase)) && !x.IsAbstract)
                .OrderBy(x => x.Name);

            foreach (var controller in controllers)
                itemNames.AddRange(ItemNamesForController(controller));

            return itemNames;
        }
        private static List<string> ItemNamesForController(Type controllerType)
        {
            var itemNames = new List<string>();
            //itemNames.Add(ItemNameForController(controllerType));
            var distinctActionNames = controllerType
                .GetMethods()
                .Where(x => x.IsPublic && x.ReturnType.IsSubtypeOf(typeof(ActionResult)))
                .OrderBy(x => x.Name)
                .GroupBy(x => x.Name)
                .ToList();
            distinctActionNames.ForEach(action => itemNames.Add(ItemNameForAction(controllerType, action.First())));

            return itemNames;
        }
        private static List<string> ItemNamesForApiControllersInSameAssemblyAs<TController>()
        {
            var itemNames = new List<string>();
            Assembly assembly = Assembly.GetAssembly(typeof(TController));
            var controllers = assembly
                .GetTypes()
                .Where(x => x.IsSubtypeOf(typeof(ApiController)) && !x.IsAbstract && !x.IsAlwaysAuthorized())
                .OrderBy(x => x.Name);

            foreach (var controller in controllers)
            {
                itemNames.Add(ItemNameForController(controller));
                var distinctActionNames = controller
                    .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.IsPublic && !x.IsAlwaysAuthorized())
                    .OrderBy(x => x.Name)
                    .GroupBy(x => x.Name)
                    .ToList();
                distinctActionNames.ForEach(action => itemNames.Add(ItemNameForAction(controller, action.First())));
            }

            return itemNames;
        }
        private static string ItemNameForAction(Type controller, MethodInfo action)
        {
            return ItemNameForElement(ItemNameForController(controller), action.Name);
        }
        private static string ItemNameForElement(string containerName, string elementName)
        {
            return string.Format("{0}{1}{2}", containerName, Dot, elementName);
        }

        private static string ItemNameForEntityType(Type entity)
        {
            return ItemNameForElement(entity.Assembly.GetName().Name, entity.Name);
        }
        private static string ItemNameForControllerType(Type controller)
        {
            return ItemNameForElement(controller.Assembly.GetName().Name, MvcExpressionHelper.GetControllerName(controller));
        }
        private static string ActionNameFor<TController>(Expression<Func<TController, ActionResult>> action)
        {
            return (action.Body as MethodCallExpression).Method.Name;
        }
        private static string ActionNameForApi<TController>(Expression<Func<TController, object>> action)
        {
            return (action.Body as MethodCallExpression).Method.Name;
        }
        #endregion
    }
}

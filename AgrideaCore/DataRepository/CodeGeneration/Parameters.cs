
using Agridea.Web.Mvc;
using System.Collections.Generic;
namespace Agridea.DataRepository
{
    public static class Parameters
    {
        #region Members
        [WebFarmCompatibleAttribute(Compatible = true)]
        private static string currentPocoBase_;
        [WebFarmCompatibleAttribute(Compatible = true)]
        private static Entities current_;
        [WebFarmCompatibleAttribute(Compatible = true)]
        private static List<string> extraUsings_;
        [WebFarmCompatibleAttribute(Compatible = true)]
        private static Order order_;
        #endregion

        #region Initialization
        static Parameters()
        {
            Reset();
        }
        public static void Reset()
        {
            currentPocoBase_ = typeof(PocoBase).Name;
            current_ = new Entities();
            extraUsings_ = new List<string>();
            order_ = new Order();
        }
        #endregion

        #region  Services
        public static string CurrentPocoBase { get { return currentPocoBase_; } set { currentPocoBase_ = value; } }
        public static string NameSpace { get; set; }
        public static string ServiceInterface { get; set; }
        public static Order Order
        {
            get { return order_; }
            set { order_ = value; }
        }

        public static Entities Current { get { return current_; } }
        public static EntityMappings EntityMappings { get; set; }
        public static IList<string> ExtraUsings { get { return extraUsings_; } }

        public static string ModelNameSpace { get; set; }
        public static string WebApiModelNameSpace { get; set; }
        public static string UnitOfWork { get; set; }
        public static string SessionManagerInterface { get; set; }
        public static string SessionManager { get; set; }

        #endregion
    }
}

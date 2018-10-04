using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Agridea.Diagnostics.Contracts;

namespace Agridea.Web.Mvc.Menu
{
    public class Menu
    {
        #region Initialization

        public Menu(IMenuBuilder builder)
        {
            menuItemForUrl_ = new Dictionary<string, IMenuItem>();
            Builder = builder;
        }

        #endregion Initialization

        #region Members

        private IMenuItem rootMenuItem_;
        private readonly IDictionary<string, IMenuItem> menuItemForUrl_;

        #endregion Members

        #region Services

        public IMenuBuilder Builder { get; private set; }
        public IMenuItem RootMenuItem
        {
            get
            {
                EnsuresRootMenuItem();
                return rootMenuItem_;
            }
        }
        public IMenuItem CurrentMenuItem
        {
            get
            {
                EnsuresRootMenuItem();

                RouteData route = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                var url = route.Values.ToUrl();
                if (menuItemForUrl_.ContainsKey(url))
                    return menuItemForUrl_[url];
                return null;
            }
        }

        public void Build()
        {
            Clear();
            rootMenuItem_ = BuildMenu(Builder.Build());
            AddMenuItem(rootMenuItem_);
        }

        #endregion Services

        #region Helpers

        private void EnsuresRootMenuItem()
        {
            if (rootMenuItem_ != null)
                return;

            Build();
        }

        private void Clear()
        {
            menuItemForUrl_.Clear();
        }

        private void AddMenuItem(IMenuItem childMenuItem)
        {
            string url = childMenuItem.GetUrl();
            if (url != null && !menuItemForUrl_.ContainsKey(url))
            {
                //Log.Verbose("Adding url '{0}' to menu", url);
                Requires<InvalidOperationException>.IsFalse(menuItemForUrl_.ContainsKey(url), string.Format("Some MenuItem already exists with url '{0}'", url));
                menuItemForUrl_[url] = childMenuItem;
            }
        }

        private IMenuItem BuildMenu(IMenuItem menuItem)
        {
            foreach (var child in menuItem.Children)
            {
                AddMenuItem(BuildMenu(child));
            }
            return menuItem;
        }

        #endregion Helpers
    }
}
using System.Web.Mvc;

namespace Agridea.Web.Mvc
{
    public class StructuredRazorViewEngine : RazorViewEngine
    {
        #region Members
        private string nameSpacePrefix_;
        #endregion

        #region Initialization
        public StructuredRazorViewEngine(string nameSpacePrefix)
            : base()
        {
            nameSpacePrefix_ = nameSpacePrefix;

            AreaViewLocationFormats = new[] 
            {
                "~/Areas/{2}/Views/%1/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/%1/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.vbhtml"
            };

            AreaMasterLocationFormats = new[] 
            {
                "~/Areas/{2}/Views/%1/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/%1/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.vbhtml"
            };

            AreaPartialViewLocationFormats = new[] 
            {
                "~/Areas/{2}/Views/%1/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/%1/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.vbhtml"
            };

            ViewLocationFormats = new[] 
            {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/%1/{1}/{0}.vbhtml",
                "~/Views/%1/Shared/{0}.cshtml",
                "~/Views/%1/Shared/{0}.vbhtml"
            };

            MasterLocationFormats = new[] 
            {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/%1/{1}/{0}.vbhtml",
                "~/Views/%1/Shared/{0}.cshtml",
                "~/Views/%1/Shared/{0}.vbhtml"
            };

            PartialViewLocationFormats = new[] 
            {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/%1/{1}/{0}.vbhtml",
                "~/Views/%1/Shared/{0}.cshtml",
                "~/Views/%1/Shared/{0}.vbhtml"
            };
        }
        #endregion

        #region VirtualPathProviderViewEngine
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return base.CreatePartialView(
                controllerContext,
                ViewEngineHelper.Transform(
                    partialPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_));
        }
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return base.CreateView(
                controllerContext,
                ViewEngineHelper.Transform(
                    viewPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_),
                ViewEngineHelper.Transform(
                    masterPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_));
        }
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(
                controllerContext,
                ViewEngineHelper.Transform(
                    virtualPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_));
        }
        #endregion
    }

    public class StructuredWebFormViewEngine : WebFormViewEngine
    {
        #region Members
        private string nameSpacePrefix_;
        #endregion

        #region Initialization
        public StructuredWebFormViewEngine(string nameSpacePrefix)
            : base()
        {
            nameSpacePrefix_ = nameSpacePrefix;

            MasterLocationFormats = new[] {
                "~/Views/%1/{1}/{0}.master"
            };

            AreaMasterLocationFormats = new[] 
            {
                "~/Areas/{2}/Views/%1/{1}/{0}.master",
                "~/Areas/{2}/Views/%1/Shared/{0}.master"
            };

            ViewLocationFormats = new[] 
            {
                "~/Views/%1/{1}/{0}.aspx",
                "~/Views/%1/{1}/{0}.ascx",
                "~/Views/%1/Shared/{0}.aspx",
                "~/Views/%1/Shared/{0}.ascx"
            };

            AreaViewLocationFormats = new[] 
            {
                "~/Areas/{2}/Views/%1/{1}/{0}.aspx",
                "~/Areas/{2}/Views/%1/{1}/{0}.ascx",
                "~/Areas/{2}/Views/%1/Shared/{0}.aspx",
                "~/Areas/{2}/Views/%1/Shared/{0}.ascx"
            };

            PartialViewLocationFormats = ViewLocationFormats;
            AreaPartialViewLocationFormats = AreaViewLocationFormats;
        }
        #endregion

        #region VirtualPathProviderViewEngine
        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return base.CreatePartialView(
                controllerContext,
                ViewEngineHelper.Transform(
                    partialPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_));
        }
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return base.CreateView(
                controllerContext,
                ViewEngineHelper.Transform(
                    viewPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_),
                ViewEngineHelper.Transform(
                    masterPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_));
        }
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(
                controllerContext,
                ViewEngineHelper.Transform(
                    virtualPath,
                    controllerContext.Controller.GetType().Namespace,
                    nameSpacePrefix_));
        }
        #endregion
    }

    public static class ViewEngineHelper
    {
        #region Constants
        private const string Dot = ".";
        private const string Slash = "/";
        private const string DoubleSlash = "//";
        #endregion

        #region Services
        public const string PlaceHolder = "%1";
        public static string Transform(string path, string nameSpace, string nameSpacePrefix)
        {
            string actualNameSpace = nameSpace
                .Replace(nameSpacePrefix, string.Empty)
                .Replace(Dot, Slash)
                .Replace(DoubleSlash, Slash);
            return path
                .Replace(PlaceHolder, actualNameSpace);
        }
        #endregion
    }
}
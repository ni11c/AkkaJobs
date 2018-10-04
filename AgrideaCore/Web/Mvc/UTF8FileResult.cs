
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Agridea.Diagnostics.Contracts;
namespace Agridea.Web.Mvc
{
    public class UTF8FileResult : ActionResult
    {
        public UTF8FileResult(string fileName, byte[] fileContents, string contentType)
        {
            Requires<ArgumentException>.IsNotEmpty(fileName);
            Requires<ArgumentException>.IsNotNull(fileContents);
            Requires<ArgumentException>.IsNotEmpty(contentType);

            FileName = fileName;
            FileContents = fileContents;
            ContentType = contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var encoding = UnicodeEncoding.UTF8;
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            response.Clear();
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", (request.Browser.Browser == "IE") ? HttpUtility.UrlEncode(FileName, encoding) : FileName));
            response.ContentType = ContentType;
            response.Charset = encoding.WebName;
            response.HeaderEncoding = encoding;
            response.ContentEncoding = encoding;
            response.BinaryWrite(FileContents);
            response.End();
        }

        public byte[] FileContents { get; private set; }
        public string ContentType { get; private set; }
        public string FileName { get; private set; }
    }
}

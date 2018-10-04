using Newtonsoft.Json;

namespace System.Web.Mvc
{
    public class JsonDataContractResult : JsonResult
    {

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            var serializedObject = JsonConvert.SerializeObject(Data);
            context.HttpContext.Response.Write(serializedObject);
        }
    }
}

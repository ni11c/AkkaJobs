using Agridea.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Web.Services
{
    public static class WebServicesHelper
    {
        public static TOut Consume<TClient, TOut>(Func<TClient> proxy = null, Func<TClient, TOut> method = null) where TClient : ICommunicationObject
        {
            if (proxy == null || method == null) return default(TOut);

            TOut response;
            var client = proxy();
            try
            {
                response = method(client);
                client.Close();
            }
            catch (FaultException e)
            {
                client.Abort();
                Log.Error("Fault exception occured while calling webservice {0} with method {1}, exception details: {2}", typeof(TClient).FullName, method.Method.Name, e.Message);
                throw;
            }
            catch (CommunicationException e)
            {
                client.Abort();
                Log.Error("Communication exception occured while calling webservice {0} with method {1}, exception details: {2}", typeof(TClient).FullName, method.Method.Name, e.Message);
                throw;
            }
            catch (TimeoutException e)
            {
                client.Abort();
                Log.Error("Timeout occured while calling webservice {0} with method {1}, exception details: {2}", typeof(TClient).FullName, method.Method.Name, e.Message);
                throw;
            }
            catch (Exception e)
            {
                client.Abort();
                Log.Error(e);
                throw;
            }
            return response;
        }
        public static void DownloadFile(string url, string filePath)
        {
            var client = new WebClient();
            try
            {
                client.DownloadFile(url, filePath);
            }
            catch (WebException e)
            {
                Log.Error("A Web exception occured while downloading {0} to {1}, details: {2} inner: {3}", url, filePath, e.Message, e.InnerException!= null ? e.InnerException.Message : "");
                throw;
            }
            catch (Exception e)
            {
                Log.Error("an Exception occured while downloading {0} to {1}, details: {2} inner: {3}", url, filePath, e.Message, e.InnerException != null ? e.InnerException.Message : "");
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}

using Agridea.Web.Api.ActionFilters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;

namespace Agridea.Acorda.Acorda2.WebApi.Model
{
    /// <summary>
    /// A .Net binding for accessing Acorda Web REST Api
    /// General features (should be in AgrideaCore)
    /// </summary>
    public partial class AcordaWebApiClient : Disposable
    {
        #region Members
        private string baseAddress_;
        private string canton_;
        private string userName_;
        private string password_;
        private HttpClient httpClient_;
        private string sessionId_;
        #endregion

        #region Initialization
        /// <summary>
        /// C# binding for Acorda WebApi
        /// </summary>
        /// <param name="url">Url to access service e.g. webapi.acorda.ch, fetching all farms will be webapi.acorda.ch/api/farms</param>
        /// <param name="canton">GE, NE, JU or VD</param>
        /// <param name="userName">e.g. alanich</param>
        /// <param name="password">not showned here</param>
        public AcordaWebApiClient(string url, string canton, string userName, string password)
        {
            baseAddress_ = url;
            canton_ = canton;
            userName_ = userName;
            password_ = password;

            httpClient_ = new HttpClient
            {
                BaseAddress = new Uri(baseAddress_),
                Timeout = TimeSpan.FromMinutes(3)
            };
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                httpClient_.Dispose();
        }
        #endregion

        #region Services
        /// <summary>
        /// Loging in to WebApi is necessary, user has role and associated permissions to entities accessed thru api
        /// </summary>
        /// <returns>success or failure</returns>
        /// <throws>TBD</throws>
        public string LogIn()
        {
            var response = httpClient_
                .PostAsJsonAsync(
                    "api/login/post",
                    new LogInViewModel()
                    {
                        Canton = canton_,
                        UserName = userName_,
                        Password = password_
                    })
                .Result;
            HandleError(response);
            sessionId_ = response.Content.ReadAsAsync<string>().Result;
            return sessionId_;
        }

        /// <summary>
        /// Loging off when the session of interaction with webapi is over
        /// </summary>
        /// <returns></returns>
        /// <throws>TBD</throws>
        public void LogOff()
        {
            var response = httpClient_
                .PostAsJsonAsync(
                    "api/login/out",
                    new LogInViewModel()
                    {
                        Canton = canton_,
                        UserName = userName_,
                        Password = null
                    },
                    CancellationToken.None)
                .Result;
            HandleError(response);
            sessionId_ = null;
        }

        /// <summary>
        /// Get single entity of type TItem 
        /// </summary>
        /// <typeparam name="TItem">The type of entity to be fetched</typeparam>
        /// <returns>Result of the query</returns>
        /// <throws>TBD</throws>
        public TItem Get<TItem>() where TItem : class, new()
        {
            var queryString = string.Format("{0}/get", typeof(TItem).Name.ToLower());
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}", queryString), UriKind.Relative),
                Method = HttpMethod.Get,
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);

            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
            return response.Content.ReadAsAsync<TItem>().Result;
        }

        /// <summary>
        /// Get all entities of type TItem, possibly filtered, ordered 
        /// </summary>
        /// <typeparam name="TItem">The type of entities to be fetched</typeparam>
        /// <param name="query">Query string in the OData format, eg</param>
        /// <returns>Result of the query</returns>
        /// <throws>TBD</throws>
        public IEnumerable<TItem> GetAll<TItem>(string query) where TItem : class, new()
        {
            var queryString = string.Format("{0}s/getall/{1}", typeof(TItem).Name.ToLower(), string.IsNullOrEmpty(query)
                ? ""
                : "?" + query);

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}", queryString), UriKind.Relative),
                Method = HttpMethod.Get,
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);
            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
            return response.Content.ReadAsAsync<IEnumerable<TItem>>().Result;

        }


        /// <summary>
        /// Get an entity of type TItem using its id
        /// </summary>
        /// <typeparam name="TItem">The type of entity to be fetched</typeparam>
        /// <param name="id">The id of the entity</param>
        /// <returns>Result of the query</returns>
        /// <throws>TBD</throws>
        public TItem GetById<TItem>(int id) where TItem : class, new()
        {
            httpClient_.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //Not mandatory

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}s/getbyid/{1}", typeof(TItem).Name.ToLower(), id), UriKind.Relative),
                Method = HttpMethod.Get,
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);

            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
            return response.Content.ReadAsAsync<TItem>().Result;
        }

        /// <summary>
        /// Post (create) an entity of type TItem
        /// </summary>
        /// <typeparam name="TItem">The type of the entity to be posted</typeparam>
        /// <param name="item">The entity</param>
        /// <returns>The entity posted coming back from the persistence layer</returns>
        /// <throws>TBD</throws>
        public TItem Post<TItem>(TItem item) where TItem : class, new()
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            HttpContent content = new ObjectContent<TItem>(item, jsonFormatter);
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}s/post", typeof(TItem).Name.ToLower()), UriKind.Relative),
                Method = HttpMethod.Post,
                Content = content
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);

            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
            return response.Content.ReadAsAsync<TItem>().Result;
        }

        /// <summary>
        /// Post (create) an list of entity of type TItem
        /// </summary>
        /// <typeparam name="TItem">The type of the entity to be posted</typeparam>
        /// <param name="list">The list of entity</param>
        /// <returns>The list of entity posted coming back from the persistence layer</returns>
        /// <throws>TBD</throws>
        public void PostAll<TItem>(IList<TItem> list) where TItem : class, new()
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            HttpContent content = new ObjectContent<IList<TItem>>(list, jsonFormatter);
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}s/postall", typeof(TItem).Name.ToLower()), UriKind.Relative),
                Method = HttpMethod.Post,
                Content = content
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);

            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
        }

        /// <summary>
        /// Put (update) and entity of type TItem
        /// </summary>
        /// <typeparam name="TItem">The type of the entity to be put</typeparam>
        /// <param name="item">The entity</param>
        /// <param name="id">The id of the existing entity</param>
        /// <throws>TBD</throws>
        public void Put<TItem>(TItem item, int id) where TItem : class, new()
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            HttpContent content = new ObjectContent<TItem>(item, jsonFormatter);
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}s/Put/{1}", typeof(TItem).Name.ToLower(), id), UriKind.Relative),
                Method = HttpMethod.Put,
                Content = content
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);

            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
        }

        /// <summary>
        /// Delete and existing entity of type TItem
        /// </summary>
        /// <typeparam name="TItem">The type of the entity to be deleted</typeparam>
        /// <param name="id">The id of the existing entity</param>
        /// <throws>TBD</throws>
        public void Delete<TItem>(int id) where TItem : class, new()
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(string.Format("api/{0}s/Delete/{1}", typeof(TItem).Name.ToLower(), id), UriKind.Relative),
                Method = HttpMethod.Delete,
            };
            requestMessage.Headers.Add(HttpActionContextHelper.SessionId, sessionId_);

            var response = httpClient_.SendAsync(requestMessage).Result;
            HandleError(response);
        }
        #endregion

        #region Helpers
        private void HandleError(HttpResponseMessage response)
        {
            var success = response.IsSuccessStatusCode;
            if (!success)
            {
                string detailedMessage = null;
                try
                {
                    var result = response.Content.ReadAsAsync<object>().Result as Newtonsoft.Json.Linq.JObject;
                    var message = result.GetValue("Message");
                    var exceptionMessage = result.GetValue("ExceptionMessage");
                    var exceptionType = result.GetValue("ExceptionType");
                    detailedMessage = string.Format("Message '{0}' ExceptionMessage '{1}' ExceptionType '{2}'", message, exceptionMessage, exceptionType);
                    //Console.WriteLine(detailedMessage);
                    
                }
                catch (Exception)
                {
                    detailedMessage = response.Content.ReadAsStringAsync().Result;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedException(detailedMessage);
                throw new ApplicationException(detailedMessage);
            }
            var trace = string.Format("{0} {1} ({2}) {3}", response.IsSuccessStatusCode, (int)response.StatusCode, response.ReasonPhrase, response.Headers.Location);
            Console.WriteLine(trace);
        }
        #endregion
    }
}
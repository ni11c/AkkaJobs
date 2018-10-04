using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Agridea.DataRepository;
using Agridea.ObjectMapping;
using Agridea.Service;

namespace Agridea.Web.Api
{
    public static class ApiServerHelper
    {
        #region Services
        public static IQueryable<TViewModel> GetAll<TEntity, TViewModel>(IService service)
            where TEntity : PocoBase
            where TViewModel : class, new()
        {
            return service
                .All<TEntity>()
                .Map<TEntity, TViewModel>();
        }
        public static TViewModel GetById<TEntity, TViewModel>(IService service, int id)
            where TEntity : PocoBase
            where TViewModel : class, new()
        {
            var item = service.GetById<TEntity>(id);
            if (item == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return EasyMapper.Map<TEntity, TViewModel>(item);
        }
        public static HttpResponseMessage Post<TEntity, TViewModel>(IService service, TViewModel viewModel)
            where TEntity : PocoBase, new()
            where TViewModel : class, new()
        {
            TEntity newEntity = new TEntity();

            newEntity.MapFrom(viewModel, service);
            newEntity.Id = 0;

            service
                .Add(newEntity)
                .Save();
            newEntity = service.GetByDiscriminant<TEntity>(newEntity);
            viewModel.MapFrom(newEntity);

            var response = HttpContext.Current.Request.CreateResponse<TEntity>(HttpStatusCode.Created, viewModel);
            string uri = Url.Link("DefaultApi", new { id = newEntity.Id });
            response.Headers.Location = new Uri(uri);

            return response;
        }
        public static void Put<TItem>(IService service, TItem item, int id)
            where TItem : class, new()
        {
        }
        public static void Delete<TItem>(IService service, int id)
            where TItem : class, new()
        {
        }
        #endregion
    }
}

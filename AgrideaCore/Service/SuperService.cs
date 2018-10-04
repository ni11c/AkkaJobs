using System;
using System.Collections.Generic;
using Agridea.DataRepository;
using Agridea.Diagnostics.Logging;

namespace Agridea.Service
{
    public class SuperService : Disposable, ISuperService
    {
        #region Properties

        public IList<IService> Services { get; private set; }
        #endregion

        #region Initialization
        public SuperService(IList<IService> services)
        {
            Log.Verbose(String.Format("'{0}:{1}' New(...)", GetType().Name, GetHashCode()));
            Services = services;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var service in Services)
                service.Dispose();
        }
        #endregion

        #region ISuperService
        public ISuperService AddAndSave<TItem>(TItem item) where TItem : PocoBase
        {
            foreach (var service in Services)
                service
                    .Add(item)
                    .Save();
            return this;
        }
        public ISuperService ModifyAndSave<TItem>(TItem item) where TItem : PocoBase, new()
        {
            foreach (var service in Services)
                service
                    .AddOrUpdateByDiscriminant(service.CloneCopy(item)) //not by Id because it can change, see RemoveAndSave
                    .Save();
            return this;
        }
        public ISuperService RemoveAndSave<TItem>(TItem item) where TItem : PocoBase
        {
            IList<IService> successfullyRemovedServices = new List<IService>();
            IService serviceRefusingRemove = null;
            try
            {
                foreach (var service in Services)
                {
                    serviceRefusingRemove = service;
                    service
                        .Remove(item)
                        .Save();
                    successfullyRemovedServices.Add(service);
                }
            }
            catch (Exception exception)
            {
                foreach (var successfullyRemovedService in successfullyRemovedServices)
                {
                    item.Id = 0;
                    successfullyRemovedService
                        .Add(item)
                        .Save();
                }
                throw new InvalidOperationException(
                    string.Format("Could not remove {0} from service {1} among {2} following services modified (undo = remove;add) {3}",
                        item,
                        DataRepositoryHelper.DatabaseNameFor(serviceRefusingRemove.ConnectionString),
                        ServiceListToString(Services),
                        ServiceListToString(successfullyRemovedServices)),
                    exception);
            }
            return this;
        }
        #endregion

        #region Helpers
        private string ServiceListToString(IEnumerable<IService> services)
        {
            string toString = "[";
            string separator = "";
            foreach (var service in services)
            {
                toString += string.Format("{0}{1}", separator, DataRepositoryHelper.DatabaseNameFor(service.ConnectionString));
                separator = ",";
            }
            toString += "]";
            return toString;
        }
        #endregion
    }
}

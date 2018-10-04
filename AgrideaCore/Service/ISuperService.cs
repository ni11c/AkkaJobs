using System;
using System.Collections.Generic;
using Agridea.DataRepository;

namespace Agridea.Service
{
    /// <summary>
    /// Federates a series of services (of same schema) in order to 
    /// define shared common (reference) entities by replication.
    /// 
    /// The services (clients) need to share the same protocol :
    /// - writes (add, modify, delete) are performed thru the superservice, whereas
    /// - reads are performed thru elementary services
    /// 
    /// In case of Remove, it can happen that it is forbidden by constraints for one 
    /// among the services ; in this case the treatment is guaranteed to be transactional
    /// - either nothing is changed in any service or the change is applied onto all services
    /// - In case the change is refused, partial accepted changes are undone 
    ///   - the side effect is that the Id (FK) of re-added items have changed
    /// </summary>
    public interface ISuperService : IDisposable
    {
        ISuperService AddAndSave<TItem>(TItem item) where TItem : PocoBase;
        ISuperService ModifyAndSave<TItem>(TItem item) where TItem : PocoBase, new();
        ISuperService RemoveAndSave<TItem>(TItem item) where TItem : PocoBase;
        IList<IService> Services { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Agridea.DataRepository;
using Agridea.Diagnostics.Logging;

namespace Agridea.Service
{
    public abstract class ImporterBase<TSource, TPoco, TService>
        where TPoco : PocoBase
        where TService : IService
        where TSource : class
    {

        #region Members

        protected ImportSettings<TPoco, TService> Mapping { get; private set; }
        private string baseTypeName_;
        private string targetTypeName_;

        #endregion

        #region Services

        public virtual ImporterBase<TSource, TPoco, TService> Initialize(ImportSettings<TPoco, TService> mapping)
        {
            Mapping = mapping;
            baseTypeName_ = Mapping.BaseType.Name;
            targetTypeName_ = typeof(TPoco).Name;
            return this;
        }
        public virtual void ConvertAndAdd()
        {
            Import(true);
        }
        public virtual void ConvertAndAddOrUpdate()
        {
            Import(Mapping.DeleteDatabase);
        }

        public virtual TPoco GetByDiscriminants(TPoco source)
        {
            throw new NotImplementedException();
        }
        public virtual TPoco GetByDiscriminants(TPoco source, IList<TPoco> existingItems)
        {
            throw new NotImplementedException();
        }
        public virtual void CascadeRemove(TPoco item)
        {
            throw new NotImplementedException();
        }
        public virtual void SqlUpdate(IEnumerable<TPoco> list)
        {
            throw new NotImplementedException();
        }
        public virtual void SqlBulkCopy(IEnumerable<TPoco> items)
        {
            throw new NotImplementedException();
        }
        public virtual bool ConvertData(TSource source, TPoco target)
        {
            throw new NotImplementedException();
        }

        protected void Import(bool addOnly)
        {
            if (Mapping.SourceList.Count == 0) return;
            const int nbUpdate = 500;
            var counter = 0;
            var updateTime = Mapping.SourceList.Select(poco => poco.CreationDate).Min();
            var existingItems = new List<TPoco>();
            var newItems = new List<TPoco>();
            if (addOnly)
            {
                newItems = Mapping.SourceList.ToList();
            }
            else
            {
                foreach (var source in Mapping.SourceList)
                {
                    var existing = GetByDiscriminants(source);
                    if (existing == null)
                        newItems.Add(source);
                    else
                    {
                        source.Id = existing.Id;
                        existingItems.Add(source);
                    }
                }
                counter = 0;
                while (existingItems.Count - counter >= 0)
                {
                    SqlUpdate(existingItems.Skip(counter).Take(nbUpdate));
                    counter += nbUpdate;
                }
            }

            counter = 0;
            while (newItems.Count - counter >= 0)
            {
                Log.Info("SqlBulkCopy {0}, {1} items from {2}...", typeof(TPoco).Name, nbUpdate, counter);
                SqlBulkCopy(newItems.Skip(counter).Take(nbUpdate));
                counter += nbUpdate;
            }
            //SqlBulkCopy(newItems);

            Mapping.TargetService.Reset();
            if (addOnly)
                CheckConversion(0);
            else
                DeleteItemsOlderThan(updateTime.AddSeconds(-1));
        }
        #endregion

        #region Helpers

        private void DeleteItemsOlderThan(DateTime time)
        {
            var itemsToRemove = Mapping.TargetService.GetItemsCreatedOrModifiedBefore<TPoco>(time);
            var itemsToRemoveCount = itemsToRemove.Count;
            CheckConversion(itemsToRemoveCount);
            if (!Mapping.AuthorizeDeletion)
                Log.Info("Items won't be deleted for {0}", targetTypeName_);
            else
            {
                if (itemsToRemoveCount > 0)
                {
                    Log.Info("Deleting {0} {1}", itemsToRemoveCount, targetTypeName_);
                    itemsToRemove.ToList().ForEach(m =>
                    {
                        Log.Info("Deletion of : {0}", m.ToString());
                        CascadeRemove(m);
                        Mapping.TargetService.Save();

                    });
                }
                Mapping.TargetService.Reset();
            }
        }
        private void CheckConversion(int nbItemsToRemove)
        {
            var targetItemsCount = Mapping.TargetService.Count<TPoco>();
            var message = string.Format("Oracle items ({0}) : '{1}' / Imported items ({2}) : '{3}' ({4} item{5} will be deleted)",
                                        baseTypeName_,
                                        Mapping.SourceList.Count,
                                        targetTypeName_,
                                        Mapping.TargetService.Count<TPoco>(), nbItemsToRemove > 0 ? nbItemsToRemove.ToString() : "no",
                                        nbItemsToRemove > 1 ? "s" : string.Empty
                );
            if (targetItemsCount - nbItemsToRemove == Mapping.SourceList.Count)
                Log.Info(message);
            else
                Log.Error(message);
        }
        #endregion
    }

    public class ImportSettings<TPoco, TService>
        where TPoco : PocoBase
        where TService : IService
    {
        public Type BaseType { get; set; }
        public TService TargetService { get; set; }
        public bool DeleteDatabase { get; set; }
        public bool AuthorizeDeletion { get; set; }
        public bool AddOnly { get; set; }
        public IList<TPoco> SourceList { get; set; }


    }
}

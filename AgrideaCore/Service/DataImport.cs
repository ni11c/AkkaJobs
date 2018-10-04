using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;

namespace Agridea.Service
{
    public abstract class DataImport
    {
        #region Constants
        private const string ConvertDataMethod = "ConvertData";
        private const string RemoveCascadeMethod = "CascadeRemove";
        private const int ResetCount = 100;
        //private int counterLogTime_ = 999;
        #endregion

        #region Members
        private readonly bool authorizeDeletion_;
        private readonly bool databaseIsNew_;
        private Type sourceType_;
        private Type targetType_;
        #endregion

        #region Initialization
        protected DataImport(bool deleteDataBase, bool authorizeDeletion = true)
        {
            authorizeDeletion_ = authorizeDeletion;
            databaseIsNew_ = deleteDataBase;
        }
        protected void ImportData()
        {
            var startTime = DateTime.Now;
            Log.Info("{0} starting", GetType().Name.ToUpper());
            Log.Info("Source data initialization starting");
            InitializeLists();
            Log.Info("Source data initialization done : duration {0}", DateTime.Now - startTime);
            ImportFromOracle();
            Log.Info("{0} done : duration {1}", GetType().Name.ToUpper(), DateTime.Now - startTime);
        }
        #endregion

        #region Hooks
        protected abstract void InitializeLists();
        protected abstract void ImportFromOracle();
        #endregion

        #region Services
        public void ConvertAndAddOrUpdate<TSource, TTarget>(IList<TSource> sourceList, IService targetService)
            where TTarget : PocoBase, new()
        {
            Convert<TSource, TTarget>(sourceList, targetService, databaseIsNew_);

        }
        public void ConvertAndAdd<TSource, TTarget>(IList<TSource> sourceList, IService targetService)
            where TTarget : PocoBase, new()
        {
            Convert<TSource, TTarget>(sourceList, targetService, true);
        }
        public void ConvertSuperService<TSource, TTarget>(IList<TSource> sourceList, ISuperService targetService)
            where TTarget : PocoBase, new()
        {
            SuperConvert<TSource, TTarget>(sourceList, targetService, databaseIsNew_);
        }
        #endregion

        #region Helpers
        private void Convert<TSource, TTarget>(ICollection<TSource> sourceList, IService targetService, bool addOnly) where TTarget : PocoBase, new()
        {
            sourceType_ = typeof(TSource);
            targetType_ = typeof(TTarget);
            var convertMethod = GetConvertMethod<TSource, TTarget>();
            var startTime = DateTime.Now;
            Log.Info(string.Format("{0} {1} count '{2}'", sourceType_.Name, targetType_.Name, sourceList.Count));

            foreach (var source in sourceList)
            {
                var target = new TTarget();
                convertMethod.Invoke(this, new object[] { source, target });
                if (addOnly)
                {
                    targetService.Add(target);
                }
                else
                    targetService.AddOrUpdateByDiscriminant(target);

                SafeSave(targetService, true);
            }

            SafeSave(targetService);
            //insertMethod.Invoke(targetService, new object[] { items });
            targetService.Reset();
            Log.Info("Importing {0} done. Duration : {1}", targetType_.Name, DateTime.Now - startTime);

            if (addOnly)
                CheckConversion<TSource, TTarget>(sourceList, 0, targetService);
            else
                DeleteItems<TSource, TTarget>(sourceList, startTime, targetService);
        }

        private void SuperConvert<TSource, TTarget>(ICollection<TSource> sourceList, ISuperService targetService, bool addOnly) where TTarget : PocoBase, new()
        {

            sourceType_ = typeof(TSource);
            targetType_ = typeof(TTarget);
            var convertMethod = GetConvertMethod<TSource, TTarget>();
            var startTime = DateTime.Now;
            Log.Info(string.Format("{0} {1} count '{2}'", sourceType_.Name, targetType_.Name, sourceList.Count));

            foreach (var source in sourceList)
            {
                var target = new TTarget();
                convertMethod.Invoke(this, new object[] { source, target });
                if (addOnly)
                    targetService.AddAndSave(target);

                else
                    targetService.ModifyAndSave(target);

            }
            foreach (var service in targetService.Services)
            {
                service.Reset();
                if (addOnly)
                    CheckConversion<TSource, TTarget>(sourceList, 0, service);
                else
                    DeleteItems<TSource, TTarget>(sourceList, startTime, service);
            }
            Log.Info("Importing {0} done. Duration : {1}", targetType_.Name, DateTime.Now - startTime);
        }

        private void SafeSave(IService targetService, bool hasCountedReset = false)
        {
            try
            {
                if (hasCountedReset)
                    targetService.SaveAndResetEvery(ResetCount);
                else
                    targetService.Save().Reset();
            }
            catch (Exception e)
            {
                Log.Error("Error while trying to save {0} (from {1}.{2} Exception : {3}", sourceType_.ToString(), targetType_.ToString(), Environment.NewLine, e.Message);
                throw;
            }
        }
        private MethodInfo GetConvertMethod<TSource, TTarget>()
        {
            var convertMethod = GetType().GetMethod(ConvertDataMethod, new[] { typeof(TSource), typeof(TTarget) });
            Requires<MissingMethodException>.IsNotNull(convertMethod, string.Format("Method {0}({1} source, {2} target, ) is missing", ConvertDataMethod, typeof(TSource), typeof(TTarget)));
            return convertMethod;
        }
        private MethodInfo GetInsertMethod<TSource, TTarget>(IService targetService)
        {
            var insertMethod = targetService.GetType().GetMethod("SqlInsert", new[] { typeof(IEnumerable<TTarget>) });
            Requires<MissingMethodException>.IsNotNull(insertMethod, string.Format("Method {0}({1} source, {2} target, ) is missing", ConvertDataMethod, typeof(TSource), typeof(TTarget)));
            return insertMethod;
        }
        private void DeleteItems<TSource, TTarget>(ICollection<TSource> sourceList, DateTime importDateTime, IService targetService) where TTarget : PocoBase
        {
            var itemsToRemove = targetService.GetItemsCreatedOrModifiedBefore<TTarget>(importDateTime);
            var itemsToRemoveCount = itemsToRemove.Count;
            CheckConversion<TSource, TTarget>(sourceList, itemsToRemoveCount, targetService);
            if (!authorizeDeletion_)
                Log.Info("Items won't be deleted for {0}", targetType_.Name);
            else
            {
                if (itemsToRemoveCount > 0)
                {
                    Log.Info("Deleting {0} {1}", itemsToRemoveCount, targetType_.Name);
                    var cascadeRemoveMethodInfo = targetService.GetType().GetMethod(RemoveCascadeMethod,
                                                                                    new[] { targetType_ });
                    itemsToRemove.ToList().ForEach(m =>
                    {
                        Log.Info("Deletion of : {0}", m.ToString());
                        cascadeRemoveMethodInfo.Invoke(targetService, new[] { m });
                        targetService.Save();

                    });
                }
                targetService.Reset();
            }
        }
        private void CheckConversion<TSource, TTarget>(ICollection<TSource> sourceList, int nbItemsToRemove, IService targetService)
            where TTarget : PocoBase
        {
            var targetItemsCount = targetService.Count<TTarget>();
            var message = string.Format("Oracle items ({0}) : '{1}' / Imported items ({2}) : '{3}' ({4} item{5} will be deleted)",
                                        sourceType_.Name,
                                        sourceList.Count,
                                        targetType_.Name,
                                        targetService.Count<TTarget>(), nbItemsToRemove > 0 ? nbItemsToRemove.ToString() : "no",
                                        nbItemsToRemove > 1 ? "s" : string.Empty
                );
            if (targetItemsCount - nbItemsToRemove == sourceList.Count)
                Log.Info(message);
            else
                Log.Error(message);
        }

        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Agridea.DataRepository
{
    public interface IPocoBase
    {
        int Id { get; set; }

        string CreatedBy { get; set; }

        DateTime CreationDate { get; set; }

        string ModifiedBy { get; set; }

        DateTime? ModificationDate { get; set; }

        bool HasDiscriminant();
    }

    public interface ICombo : IPocoBase
    {
        string ComboText { get; }

        Func<IPocoBase, object> SortFunc { get; }
    }

    [Serializable]
    public abstract class PocoBase : IPocoBase
    {
        #region Initialization

        public PocoBase()
        {
            CreationDate = DateTime.Now;
            CreatedBy = null;
            ModificationDate = null;
            ModifiedBy = null;
        }

        #endregion Initialization

        #region Identity

        public override string ToString()
        {
            return string.Format("<{0} Id='{1}' CreatedBy='{2}' CreationDate='{3}' ModifiedBy='{4}' ModificationDate='{5}'>",
                 GetType().Name,
                 Id,
                 CreatedBy,
                 CreationDate,
                 ModifiedBy,
                 ModificationDate);
        }

        #endregion Identity

        #region Properties

        public int Id { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual Nullable<DateTime> ModificationDate { get; set; }

        #endregion Properties

        #region Services

        public IEnumerable<PropertyInfo> GetDiscriminants()
        {
            return GetType().GetPublicPropertiesWithVirtualSetters()
                .Where(p => p.IsDiscriminant());
        }

        public IEnumerable<PropertyInfo> GetPocoDiscriminants()
        {
            return GetDiscriminants()
                .Where(m => m.IsReference());
        }

        public IEnumerable<PropertyInfo> GetPrimitiveDiscriminants()
        {
            return GetDiscriminants()
                .Where(m => m.IsPrimitive());
        }

        public bool HasDiscriminant()
        {
            return GetType().GetProperties().Any(m => m.IsDiscriminant());
        }

        public static bool HasValue(PocoBase poco)
        {
            return poco != null && poco.Id > 0;
        }

        [Transient]
        public DateTime LatestChangeDate
        {
            get { return ModificationDate ?? CreationDate; }
        }

        #endregion Services
    }
}
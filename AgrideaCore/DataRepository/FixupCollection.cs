using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Agridea.DataRepository
{
    /// <summary>
    ///  An System.Collections.ObjectModel.ObservableCollection that raises
    ///  individual item removal notifications on clear and prevents adding duplicates.
    /// </summary>
    public class FixupCollection<T> : ObservableCollection<T>
    {
        #region Initialization
        public FixupCollection()
            : base()
        {
        }
        public FixupCollection(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }
        public FixupCollection(List<T> list)
            : base(list)
        {
        }
        #endregion

        #region ObservableCollection
        protected override void ClearItems()
        {
            new List<T>(this).ForEach(t => Remove(t));
        }
        protected override void InsertItem(int index, T item)
        {
            if (!this.Contains(item))
                base.InsertItem(index, item);
        }
        #endregion
    }
}

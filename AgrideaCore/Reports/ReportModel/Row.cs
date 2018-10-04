using System.Collections.Generic;

namespace Agridea.Reports
{
    public class Row
    {
        #region Members
        internal List<Header> headers_;
        private List<Cell> cells_;
        #endregion

        #region Initialization
        public Row()
        {
            headers_ = new List<Header>();
            cells_ = new List<Cell>();
        }
        #endregion

        #region Services
        public Row Add(Header Header)
        {
            headers_.Add(Header);
            return this;
        }
        public Header[] Headers { get { return headers_.ToArray(); } }

        public Row Add(Cell Cell)
        {
            cells_.Add(Cell);
            return this;
        }
        public Cell[] Cells { get { return cells_.ToArray(); } }
        #endregion
    }
}

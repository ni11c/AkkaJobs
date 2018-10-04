using System.Collections.Generic;

namespace Agridea.Reports
{
    public class Table
    {
        #region Members
        private float totalWidth_;
        private List<Row> rows_;
        #endregion

        #region Initialization
        public Table(float totalWidth = 100)
        {
            totalWidth_ = totalWidth;
            rows_ = new List<Row>();
            BreakPageAfter = false;
            HeaderRows = 0;
        }
        #endregion

        #region Services
        public float TotalWidth { get { return totalWidth_; } }
        public float[] RelativeWidths { get { return GetRelativeWidths(); } }
        public bool BreakPageAfter { get; set; }
        public int HeaderRows { get; set; }
        public Table Add(Row row)
        {
            rows_.Add(row);
            return this;
        }
        public Row[] Rows { get { return rows_.ToArray(); } }
        #endregion

        #region Helpers
        private float[] GetRelativeWidths()
        {
            List<float> relativeWidths = new List<float>();
            foreach (var row in Rows)
            {
                foreach (var header in row.Headers)
                    if (header.RelativeWidth > 0)
                        relativeWidths.Add(header.RelativeWidth);
                foreach (var cell in row.Cells)
                    if (cell.RelativeWidth > 0)
                        relativeWidths.Add(cell.RelativeWidth);
            }
            return relativeWidths.ToArray();
        }
        #endregion
    }
}

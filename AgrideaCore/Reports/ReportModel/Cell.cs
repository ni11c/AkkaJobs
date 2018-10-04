namespace Agridea.Reports
{
    public class Cell
    {
        #region Initialization
        public Cell()
        {
            RelativeWidth = 0;
            ColSpan = 1;

            Style = ReportStyles.Normal;
            Color = ReportColors.None;
            Alignment = ReportAlignments.None;
            Border = true;

            Export = ExportModes.Export;
        }
        #endregion

        #region Services
        #region Content
        public string Text { get; set; }
        #endregion

        #region Layout
        public float RelativeWidth { get; set; }
        public int ColSpan { get; set; }
        #endregion

        #region Style
        public ReportStyles Style { get; set; }
        public ReportColors Color { get; set; }
        public ReportAlignments Alignment { get; set; }
        public bool Border { get; set; }
        #endregion

        #region Export
        public ExportModes Export { get; set; }
        public string ExportText { get; set; }
        #endregion
        #endregion
    }
}

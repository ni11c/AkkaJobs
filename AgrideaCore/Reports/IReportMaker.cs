namespace Agridea.Reports
{
    /// <summary>
    /// Fullfilled by a report maker which transforms a report model into some media form 
    /// either in memory (Make) or file (Write)
    /// - e.g. Pdf, Html, csv...
    /// </summary>
    public interface IReportMaker
    {
        object Make(Report report, bool addHeader = false);
        void Write(Report report, string filePath);
        void Append(Report report, string filePath);
    }
}
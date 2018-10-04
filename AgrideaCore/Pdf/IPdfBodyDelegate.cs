using iTextSharp.text;

namespace Agridea.iTextSharp
{
    /// <summary>
    /// Fulfilled by a Pdf report maker to define the body of a Pdf document
    /// </summary>
    public interface IPdfBodyDelegate
    {
        void AddBody(Document document);
    }
}

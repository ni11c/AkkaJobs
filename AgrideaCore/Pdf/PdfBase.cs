using Agridea.Diagnostics.Contracts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace Agridea.iTextSharp
{
    /// <summary>
    /// Base class for Pdf documents, defines
    /// - layout, style
    /// - pdf bytes creation
    /// - hooks for start/end Document, Page, Chapter, Section, GenericTag
    /// </summary>
    public abstract class PdfBase
    {
        #region Constants

        private static readonly Rectangle DefaultSize = PageSize.A4;
        private const float DefaultRightMargin = 30f;
        private const float DefaultLeftMargin = 30f;
        private const float DefaultTopMargin = 150f;
        private const float DefaultBottomMargin = 30f;
        private const bool DefaultLandscape = false;

        #endregion Constants

        #region Initialization

        public PdfBase()
        {
            SetPropertiesDefault();
        }

        #endregion Initialization

        #region Services

        #region Layout

        public Rectangle Size { get; set; }

        public float LeftMargin { get; set; }

        public float RightMargin { get; set; }

        public float TopMargin { get; set; }

        public float BottomMargin { get; set; }

        public bool Landscape { get; set; }

        public float GetDocumentWidth(Document document)
        {
            return document.PageSize.Width - document.LeftMargin - document.RightMargin;
        }

        public float GetDocumentHeight(Document document)
        {
            return document.PageSize.Height - document.TopMargin - document.BottomMargin;
        }

        #endregion Layout

        #region Style

        public Font Normal { get; set; }

        public Font Italic { get; set; }

        public Font Bold { get; set; }

        public BaseColor LightColor { get; set; }

        public BaseColor DarkColor { get; set; }

        #endregion Style

        #region Document

        public IPdfBodyDelegate BodyDelegate { get; set; }

        public byte[] CreatePdf()
        {
            Document document = new Document(
                Landscape ? Size.Rotate() : Size,
                LeftMargin,
                RightMargin,
                TopMargin,
                BottomMargin);

            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                writer.PageEvent = new PdfBasePageEvent(this);

                try
                {
                    document.Open();
                    AddBody(writer, document);
                    document.Close();
                }
                catch (IOException)
                {
                    document.Dispose();
                }

                return stream.ToArray();
            }
        }

        #endregion Document

        #endregion Services

        #region Hooks

        protected virtual void AddBody(PdfWriter writer, Document document)
        {
            Requires<InvalidOperationException>.IsNotNull(BodyDelegate);
            BodyDelegate.AddBody(document);
        }

        public virtual void AddNewLine(Document document)
        {
            document.Add(new Paragraph(" "));
        }

        public virtual void AddNewPage(Document document)
        {
            document.NewPage();
        }

        protected virtual void StartDocument(PdfWriter writer, Document document)
        {
        }

        protected virtual void EndDocument(PdfWriter writer, Document document)
        {
        }

        protected virtual void StartPage(PdfWriter writer, Document document)
        {
        }

        protected virtual void EndPage(PdfWriter writer, Document document)
        {
        }

        protected virtual void StartChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
        {
        }

        protected virtual void EndChapter(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        protected virtual void StartParagraph(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        protected virtual void EndParagraph(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        protected virtual void StartSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title)
        {
        }

        protected virtual void EndSection(PdfWriter writer, Document document, float paragraphPosition)
        {
        }

        protected virtual void GenericTag(PdfWriter writer, Document document, Rectangle rectangle, string tag)
        {
        }

        #endregion Hooks

        #region Helpers

        private void SetPropertiesDefault()
        {
            Size = Size ?? DefaultSize;
            LeftMargin = DefaultLeftMargin;
            RightMargin = DefaultRightMargin;
            TopMargin = DefaultTopMargin;
            BottomMargin = DefaultBottomMargin;
            Landscape = DefaultLandscape;
        }

        private class PdfBasePageEvent : IPdfPageEvent
        {
            #region Members

            private PdfBase pdfBase_;

            #endregion Members

            #region Initialization

            public PdfBasePageEvent(PdfBase pdfBase)
            {
                pdfBase_ = pdfBase;
            }

            #endregion Initialization

            #region IPdfPageEvent

            public void OnOpenDocument(PdfWriter writer, Document document)
            {
                pdfBase_.StartDocument(writer, document);
            }

            public void OnCloseDocument(PdfWriter writer, Document document)
            {
                pdfBase_.EndDocument(writer, document);
            }

            public void OnStartPage(PdfWriter writer, Document document)
            {
                pdfBase_.StartPage(writer, document);
            }

            public void OnEndPage(PdfWriter writer, Document document)
            {
                pdfBase_.EndPage(writer, document);
            }

            public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
            {
                pdfBase_.StartChapter(writer, document, paragraphPosition, title);
            }

            public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition)
            {
                pdfBase_.EndChapter(writer, document, paragraphPosition);
            }

            public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition)
            {
                pdfBase_.StartParagraph(writer, document, paragraphPosition);
            }

            public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition)
            {
                pdfBase_.EndParagraph(writer, document, paragraphPosition);
            }

            public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title)
            {
                pdfBase_.StartSection(writer, document, paragraphPosition, depth, title);
            }

            public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition)
            {
                pdfBase_.EndSection(writer, document, paragraphPosition);
            }

            public void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, string text)
            {
                pdfBase_.GenericTag(writer, document, rect, text);
            }

            #endregion IPdfPageEvent
        }

        #endregion Helpers
    }
}
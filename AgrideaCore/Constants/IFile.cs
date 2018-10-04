
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Agridea.DataRepository;
using Agridea.Web.Helpers;

namespace Agridea
{
    public interface IFile : IPocoBase
    {
         byte[] FileData { get; set; }
         string FileName { get; set; }
         string FileType { get; set; }

    }

    public static class HttpPostBaseExtensions
    {
        public static void SetFileValues(this HttpPostedFileBase uploadFile, IFile file)
        {
            file.FileData = uploadFile.GetBytesFromUpload();
            file.FileName = uploadFile.GetFileNameWithoutPath();
            file.FileType = uploadFile.ContentType;
        }

        public static string GetFileNameWithoutPath(this HttpPostedFileBase uploadFile)
        {
            return uploadFile.FileName.Split(new[] { '\\' }).Last();
        }

        public static  byte[] GetBytesFromUpload(this HttpPostedFileBase file)
        {
            var length = file.ContentLength;
            var buffer = new byte[length];
            file.InputStream.Position = 0;
            file.InputStream.Read(buffer, 0, length);
            file.InputStream.Position = 0;
            return buffer;
        }

        public static bool HasExtension(this HttpPostedFileBase file, IEnumerable<string> expectedExtensions)
        {
            return file.HasContentType(MimeTypeHelper.GetMany(expectedExtensions));
        }
        
        public static bool HasContentType(this HttpPostedFileBase file, IEnumerable<string> allowedContentTypes)
        {
            return !allowedContentTypes.Any() || allowedContentTypes.Contains(file.ContentType);
        }
    }
}

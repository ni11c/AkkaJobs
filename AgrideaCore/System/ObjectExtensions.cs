using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System
{
    public static class ObjectExtensions
    {
        public static byte[] Serialize(this object instance)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, instance);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] bytes) where T : class
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return formatter.Deserialize(stream) as T;
            }
        }
    }
}

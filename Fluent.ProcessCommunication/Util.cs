using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Fluent.ProcessCommunication
{
    internal static class Util
    {
        internal static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        internal static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}

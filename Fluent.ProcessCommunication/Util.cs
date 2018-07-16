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

        internal static object FromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                return bf.Deserialize(ms);
            }
        }
    }
}

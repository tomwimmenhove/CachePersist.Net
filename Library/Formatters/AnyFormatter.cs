using System;
using System.IO;

namespace CachePersist.Net.Formatters
{
    public class AnyFormatter
    {
        private static IStreamFormatter GetStreamFormatter(Stream stream)
        {
            using (var reader = new BinaryReader(stream, System.Text.Encoding.Default, true))
            {
                var serializeName = reader.ReadString();
                return (IStreamFormatter) Activator.CreateInstance(Type.GetType(serializeName));
            }
        }

        public static void Serialize<T>(Stream stream, T value, IStreamFormatter formatter)
        {
            using (var writer = new BinaryWriter(stream))
            {
                /* Store which formatter was used */
                var serializerName = formatter.GetType().AssemblyQualifiedName;
                writer.Write(serializerName);
                writer.Flush();

                /* And the data */
                formatter.Serialize(stream, value);
            }
        }

        public static T Deserialize<T>(Stream stream)
        {
            var formatter = GetStreamFormatter(stream);

            return (T)formatter.Deserialize(stream);
        }

        public static void Serialize<T>(string filePath, T value, IStreamFormatter formatter)
        {
            using (var stream = File.Open(filePath, FileMode.Create))
            {
                Serialize(stream, value, formatter);
            }
        }

        public static T Deserialize<T>(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                return Deserialize<T>(stream);
            }
        }
    }
}

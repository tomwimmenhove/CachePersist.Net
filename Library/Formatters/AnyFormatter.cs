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
                var type = Type.GetType(serializeName);
                if (type == null)
                {
                    return null;
                }

                return (IStreamFormatter) Activator.CreateInstance(type);
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

            if (formatter == null)
            {
                throw new NotSupportedException("Could not load the formatter used to serialize the object");
            }

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

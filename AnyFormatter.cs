using System;
using System.IO;

namespace serialization
{
    class AnyFormatter
    {
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
            using (var reader = new BinaryReader(stream))
            {
                /* Get the formatter that was used */
                var serializeName = reader.ReadString();
                var formatter = (IStreamFormatter)Activator.CreateInstance(Type.GetType(serializeName));

                return (T)formatter.Deserialize(stream);
            }
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

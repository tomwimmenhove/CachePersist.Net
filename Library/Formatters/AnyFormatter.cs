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
                var serializerName = reader.ReadString();
                var type = Type.GetType(serializerName);
                if (type == null)
                {
                    return null;
                }

                return (IStreamFormatter) Activator.CreateInstance(type);
            }
        }

        private static string GetTypeName(Type type, bool fullyQualifiedNames)
        {
            if (fullyQualifiedNames)
            {
                return type.AssemblyQualifiedName;
            }

            var assemblyName = type.Assembly.GetName().Name;
            return $"{type.FullName}, {assemblyName}";
        }

        public static void Serialize<T>(Stream stream, T value, IStreamFormatter formatter,
            bool fullyQualifiedNames = false)
        {
            using (var writer = new BinaryWriter(stream))
            {
                /* Store which formatter was used */
                var serializerName = GetTypeName(formatter.GetType(), fullyQualifiedNames);
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

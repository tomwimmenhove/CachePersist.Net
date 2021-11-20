using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using ProtoBuf;

namespace CachePersist.Net.Formatters
{
    public class FormatterFinder : IFormatterFinder
    {
        private bool _allowCompression = false;
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;

        public FormatterFinder()
        { }

        public FormatterFinder(bool allowCompression,
            CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            _allowCompression = allowCompression;
            _compressionLevel = compressionLevel;
        }

        public IStreamFormatter FindFor(object obj)
        {
            var type = obj.GetType();
            var ProtoContractType = typeof(ProtoContractAttribute);

            if (type.GetCustomAttributes(ProtoContractType, false).Any())
            {
                return GetProtobufFormatterFor(type);
            }

            /* In case the type implements IEnumerable and it has a parameterless oncstructor,
             * check if the underlying type of the IEnumerable<T> has an ProtoContractAttribute */
            if (typeof(IEnumerable).IsAssignableFrom(type) &&
                type.GetConstructor(Type.EmptyTypes) != null &&
                type.GetInterfaces().Any(x => x.IsGenericType &&
                x.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) &&
                x.GetGenericArguments()[0].GetCustomAttributes(ProtoContractType, false).Any()))
            {
                return GetProtobufFormatterFor(type);
            }

            return GetBinaryFormatterFor(type);
        }

        private IStreamFormatter GetProtobufFormatterFor(Type type)
        {
            var genericFormatterType = typeof(ProtobufStreamFormatter<>);
            var formatterType = genericFormatterType.MakeGenericType(type);

            return (IStreamFormatter)Activator.CreateInstance(formatterType);
        }

        private IStreamFormatter GetBinaryFormatterFor(Type type)
        {
            if (_allowCompression)
            {
                return new BinaryCompressedStreamFormatter(_compressionLevel);
            }

            return new BinaryStreamFormatter();
        }
    }
}

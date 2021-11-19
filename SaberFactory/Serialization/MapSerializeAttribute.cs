using System;

namespace SaberFactory.Serialization
{
    public class MapSerializeAttribute : Attribute
    {
        public Type ConverterType;
    }
}
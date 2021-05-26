using System;

namespace SaberFactory.Saving
{
    public class MapSerializeAttribute : Attribute
    {
        public Type ConverterType;
    }
}
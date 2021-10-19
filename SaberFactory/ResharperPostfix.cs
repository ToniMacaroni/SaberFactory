#if RESHARPER

using JetBrains.Annotations;

namespace SaberFactory
{
    /// <summary>
    ///     Include some postfix templates for resharper
    /// </summary>
    internal static class ResharperPostfix
    {
        [SourceTemplate]
        public static void isnull(this object obj)
        {
            if (obj is null)
            {
                //$ $END$
            }
        }

        [SourceTemplate]
        public static void isnotnull(this object obj)
        {
            if (obj is { })
            {
                //$ $END$
            }
        }

        [SourceTemplate]
        public static void retnull(this object obj)
        {
            if (obj is null)
            {
            }
            //$ $END$
        }
    }
}

#endif
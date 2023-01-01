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
        public static void retnull(this object obj)
        {
            if (obj == null)
            {
                return;
            }
            
            //$ $END$
        }
    }
}

#endif
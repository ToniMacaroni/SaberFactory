#if RESHARPER

using JetBrains.Annotations;
using UnityEngine;

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
                return;
            }
            //$ $END$
        }
        
        [SourceTemplate]
        public static void uretnull(this UnityEngine.Object obj)
        {
            if (!obj)
            {
                return;
            }
            //$ $END$
        }
        
        [SourceTemplate]
        public static void equalret(this string obj)
        {
            if (obj == "$condition$")
            {
                return;
            }
            //$ $END$
        }
        
        [SourceTemplate]
        [Macro(Target = "condition")]
        public static void nequalret(this string obj)
        {
            if (obj != "$condition$")
            {
                return;
            }
            //$ $END$
        }

        [SourceTemplate]
        [Macro(Target = "generic", Expression = "completeType()")]
        [Macro(Target = "comp")]
        public static void com(this GameObject obj)
        {
            /*$ var $comp$ = obj.GetComponent<$generic$>();
            $END$ */
        }
        
        [SourceTemplate]
        [Macro(Target = "generic", Expression = "completeType()")]
        [Macro(Target = "comp")]
        public static void comc(this GameObject obj)
        {
            /*$ var $comp$ = obj.GetComponentInChildren<$generic$>();
            $END$ */
        }

        [SourceTemplate]
        public static void dbg(this object obj)
        {
            Debug.Log($"$obj$: {obj}");
        }
        
        [SourceTemplate]
        public static void dbgn(this object obj)
        {
            Debug.Log($"$obj$ valid: {obj != null}");
        }
    }
}

#endif
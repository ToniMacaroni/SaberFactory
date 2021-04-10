using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaberFactory.UI.Lib.BSML
{
    public static class BSMLTools
    {
        public static string GetKebabCaseName(Type type)
        {
            return Regex.Replace(
                type.Name,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled).Trim().ToLower();
        }
    }
}

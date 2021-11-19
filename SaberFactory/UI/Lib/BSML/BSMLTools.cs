using System;
using System.Text;

namespace SaberFactory.UI.Lib.BSML
{
    public static class BSMLTools
    {
        public static string GetKebabCaseName(Type type)
        {
            var name = type.Name;

            var builder = new StringBuilder();

            for (var i = 0; i < name.Length; i++)
            {
                if (char.IsLower(name[i]))
                {
                    builder.Append(name[i]);
                }
                else if (i == 0)
                {
                    builder.Append(char.ToLowerInvariant(name[i]));
                }
                else if (char.IsDigit(name[i]) && !char.IsDigit(name[i - 1]))
                {
                    builder.Append('-');
                    builder.Append(name[i]);
                }
                else if (char.IsDigit(name[i]))
                {
                    builder.Append(name[i]);
                }
                else if (char.IsLower(name[i - 1]))
                {
                    builder.Append('-');
                    builder.Append(char.ToLowerInvariant(name[i]));
                }
                else if (i + 1 == name.Length || char.IsUpper(name[i + 1]))
                {
                    builder.Append(char.ToLowerInvariant(name[i]));
                }
                else
                {
                    builder.Append('-');
                    builder.Append(char.ToLowerInvariant(name[i]));
                }
            }

            return builder.ToString();
        }
    }
}
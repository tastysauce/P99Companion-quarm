using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static partial class ExtensionMethods
    {
        public static bool EqualIngoreCase(this string source, string value)
        {
            return string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
        }
    }
}

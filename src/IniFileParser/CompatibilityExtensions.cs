using System;
using System.Collections.Generic;
#if !NET20
using System.Linq;
#endif
using System.Reflection;
using System.Text;

namespace IniParser
{
    internal static class CompatibilityExtensions
    {
#if !NET20
        internal static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.ToArray();
        }
#endif
    }
}

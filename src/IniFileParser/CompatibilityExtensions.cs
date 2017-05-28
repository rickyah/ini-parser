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
        internal static Version GetCurrentVersion(this Type type)
        {
            Assembly assembly;
#if NET20
            assembly = Assembly.GetExecutingAssembly();
#else
            assembly = type.GetTypeInfo().Assembly;
#endif
            return assembly.GetName().Version;
        }
    }
}

// you need this once (only), and it must be in this namespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
         | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}

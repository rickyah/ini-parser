using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniParser.Extensions
{
	public static class AssemblyExtensions
	{
		public static string GetVersion(this Assembly asm) {
			var attr = CustomAttributeExtensions.GetCustomAttribute<AssemblyFileVersionAttribute>(asm);
			if (attr != null)
				return attr.Version;
			else
				return "";
		}
	}
}

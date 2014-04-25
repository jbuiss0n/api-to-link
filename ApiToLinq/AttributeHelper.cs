using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiToLinq
{
	public static class AttributeHelper
	{
		public static IEnumerable<T> GetAttributes<T>(PropertyInfo property)
		{
			return property.GetCustomAttributes(typeof(T), false) as T[];
		}

		public static T GetAttribute<T>(PropertyInfo property)
		{
			return GetAttributes<T>(property).FirstOrDefault();
		}

		public static IEnumerable<T> GetAttributes<T>(Type type)
		{
			return type.GetCustomAttributes(typeof(T), false) as T[];
		}

		public static T GetAttribute<T>(Type type)
		{
			return GetAttributes<T>(type).FirstOrDefault();
		}
	}
}

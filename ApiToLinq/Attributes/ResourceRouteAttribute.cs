using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiToLinq.Attributes
{
	public class ResourceKeyAttribute : Attribute
	{

	}

	public interface IRouteAttribute
	{
		string RouteName { get; }

		string Related { get; }
	}

	public class ResourceRouteAttribute : Attribute, IRouteAttribute
	{
		public string RouteName { get; private set; }

		public string Related { get; private set; }

		public ResourceRouteAttribute(string related, string routeName = null)
		{
			Related = related;
			RouteName = routeName;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public class LinkedResourceRoute : Attribute, IRouteAttribute
	{
		public string DisplayName { get; private set; }

		public string RouteName { get; private set; }

		public string Related { get; private set; }

		public LinkedResourceRoute(string displayName, string related, string routeName = null)
		{
			DisplayName = displayName;
			Related = related;
			RouteName = routeName;
		}

	}
}

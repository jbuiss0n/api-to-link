using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApiToLinq.ModelBinding
{
	internal class LinkModel
	{
		public string Name { get; set; }

		public string Related { get; set; }

		public string RouteName { get; set; }

		public PropertyInfo RouteValue { get; set; }
	}
}

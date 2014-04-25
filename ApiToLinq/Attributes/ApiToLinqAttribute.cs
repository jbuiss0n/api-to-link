using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToLinq.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public abstract class ApiToLinqAttribute : Attribute
	{
		public string Name { get; private set; }

		public ApiToLinqAttribute(string name)
		{
			Name = name;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApiToLinq.ModelBinding
{
	internal class ResourceBinder
	{
		public IList<LinkModel> Links { get; private set; }

		public IList<PropertyInfo> Attributes { get; private set; }

		public ResourceBinder()
		{
			Links = new List<LinkModel>();
			Attributes = new List<PropertyInfo>();
		}
	}
}

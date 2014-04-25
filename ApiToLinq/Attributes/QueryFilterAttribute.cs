using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToLinq.Attributes
{
	public class QueryFilterAttribute : ApiToLinqAttribute
	{
		public QueryFilterAttribute(string name)
			: base(name)
		{
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiToLinq.Attributes
{
	public class QueryConditionAttribute : ApiToLinqAttribute
	{
		public QueryConditionAttribute(string name)
			: base(name)
		{
		}
	}
}
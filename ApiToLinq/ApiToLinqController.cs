using ApiToLinq.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.ComponentModel;
using ApiToLinq.ModelBinding;

namespace ApiToLinq
{
	public abstract class ApiToLinqController<T> : ApiController
		where T : new()
	{
		private IDictionary<string, MethodInfo> m_conditions = new Dictionary<string, MethodInfo>();
		private IDictionary<string, MethodInfo> m_filters = new Dictionary<string, MethodInfo>();

		public ApiToLinqController()
		{
			var methods = GetType().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var method in methods)
			{
				FillMethodAttributes<QueryConditionAttribute>(method, m_conditions);

				FillMethodAttributes<QueryFilterAttribute>(method, m_filters);
			}
		}

		protected IHttpActionResult Query(IQueryable<T> data)
		{
			var conditions = new List<Func<T, bool>>();
			var filters = new List<Func<IQueryable<T>, IQueryable<T>>>();

			foreach (var keyValue in Request.GetQueryNameValuePairs())
			{
				if (m_conditions.ContainsKey(keyValue.Key))
				{
					var parameters = GetParametersFromQueryValue(keyValue.Value, m_conditions[keyValue.Key].GetParameters());
					conditions.Add(m_conditions[keyValue.Key].Invoke(this, parameters.ToArray()) as Func<T, bool>);
				}
				else if (m_filters.ContainsKey(keyValue.Key))
				{
					var parameters = GetParametersFromQueryValue(keyValue.Value, m_filters[keyValue.Key].GetParameters());
					filters.Add(m_filters[keyValue.Key].Invoke(this, parameters.ToArray()) as Func<IQueryable<T>, IQueryable<T>>);
				}
			}

			foreach (var condition in conditions)
			{
				data = data.Where(condition).AsQueryable();
			}

			foreach (var filter in filters)
			{
				data = filter(data);
			}

			return Ok(data.ToList());
		}

		protected IHttpActionResult Ok(T item)
		{
			var resource = GetResource(item);

			return Ok(resource);
		}

		protected IHttpActionResult Ok(IEnumerable<T> collections)
		{
			return Ok(collections.Select(i => GetResource(i)));
		}

		private IDictionary<string, object> GetResource(T model)
		{
			var binder = ResourceFactory.GetResourceBinder(typeof(T));
			var resource = new Dictionary<string, object>();

			foreach (var link in binder.Links)
			{
				resource.Add(link.Name, GetResourceLink(model, link));
			}

			foreach (var attribute in binder.Attributes)
			{
				resource.Add(attribute.Name, attribute.GetValue(model));
			}

			return resource;
		}

		private IDictionary<string, object> GetResourceLink(T model, LinkModel link)
		{
			return new Dictionary<string, object>
			{
				{ "related", link.Related },
				{ "href", GetRouteUrl(model, link) },
			};
		}

		private string GetRouteUrl(T model, LinkModel link)
		{
			return String.Format("{0}://{1}{2}",
				Request.RequestUri.Scheme,
				Request.RequestUri.Authority,
				Url.Route(link.RouteName, new Dictionary<string, object> { { link.RouteValue.Name, link.RouteValue.GetValue(model) } })
				);
		}

		private IEnumerable<object> GetParametersFromQueryValue(string query, ParameterInfo[] parameterInfos)
		{
			var values = query.Split(',');
			for (int i = 0; i < parameterInfos.Length; i++)
			{
				yield return Convert.ChangeType(values[i], parameterInfos[i].ParameterType);
			}
		}

		private static void FillMethodAttributes<TAttribute>(MethodInfo method, IDictionary<string, MethodInfo> dictionnary)
			where TAttribute : ApiToLinqAttribute
		{
			var conditionAttribute = ((TAttribute[])method.GetCustomAttributes(typeof(TAttribute), true)).FirstOrDefault();
			if (conditionAttribute != null)
			{
				dictionnary.Add(conditionAttribute.Name, method);
			}
		}
	}
}

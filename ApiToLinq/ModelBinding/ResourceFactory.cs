using ApiToLinq.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiToLinq.ModelBinding
{
	internal static class ResourceFactory
	{
		private static IDictionary<Type, ResourceBinder> m_factory;

		static ResourceFactory()
		{
			m_factory = new Dictionary<Type, ResourceBinder>();
		}

		public static ResourceBinder GetResourceBinder(Type type)
		{
			if (!m_factory.ContainsKey(type))
				m_factory.Add(type, CreateResourceBinder(type));

			return m_factory[type];
		}

		private static ResourceBinder CreateResourceBinder(Type type)
		{
			var binder = new ResourceBinder();
			var properties = type.GetProperties();
			var key = GetKeyProperty(properties);

			binder.Links.Add(GetModelLink(type, key));

			foreach (var property in properties)
			{
				if (AttributeHelper.GetAttribute<ApiIgnoreAttribute>(property) != null)
					continue;

				var resourceLink = GetPropertyLink(type, property);

				if (resourceLink != null)
				{
					binder.Links.Add(resourceLink);
				}
				else
				{
					binder.Attributes.Add(property);
				}
			}

			foreach (var linkedAttributes in AttributeHelper.GetAttributes<LinkedResourceRoute>(type))
			{
				binder.Links.Add(GetLink(type, key, linkedAttributes));
			}

			return binder;
		}

		private static PropertyInfo GetKeyProperty(PropertyInfo[] properties)
		{
			var keyProperty = properties.First(p => p.GetCustomAttributes(typeof(ResourceKeyAttribute), true) != null);

			if (keyProperty == null)
				throw new InvalidOperationException("The resource has no key property ! Use the ResourceKeyAttribute to specifiy one.");

			return keyProperty;
		}

		private static LinkModel GetPropertyLink(Type type, PropertyInfo property)
		{
			var attribute = AttributeHelper.GetAttribute<ResourceRouteAttribute>(property);

			if (attribute == null)
				return null;

			return GetLink(type, property, attribute);
		}

		private static LinkModel GetModelLink(Type type, PropertyInfo keyProperty)
		{
			var attribute = AttributeHelper.GetAttribute<ResourceRouteAttribute>(type);

			if (attribute == null)
				return null;

			return GetLink(type, keyProperty, attribute);
		}

		private static LinkModel GetLink(Type type, PropertyInfo property, ResourceRouteAttribute resourceRouteAttribute)
		{
			return new LinkModel
			{
				Name = GetDisplayName(property),
				Related = resourceRouteAttribute.Related,
				RouteName = resourceRouteAttribute.RouteName ?? resourceRouteAttribute.Related,
				RouteValue = property,
			};
		}

		private static LinkModel GetLink(Type type, PropertyInfo keyProperty, LinkedResourceRoute linkedResourceRoute)
		{
			return new LinkModel
			{
				Name = linkedResourceRoute.DisplayName,
				Related = linkedResourceRoute.Related,
				RouteName = linkedResourceRoute.RouteName ?? linkedResourceRoute.Related,
				RouteValue = keyProperty,
			};
		}

		private static string GetDisplayName(PropertyInfo property)
		{
			var attribute = AttributeHelper.GetAttribute<DisplayNameAttribute>(property);
			return attribute != null ? attribute.DisplayName : property.Name;
		}
	}
}

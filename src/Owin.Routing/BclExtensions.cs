﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Owin.Routing
{
	internal static class CustomAttributeProviderExtensions
	{
		public static T[] GetAttributes<T>(this ICustomAttributeProvider provider, bool inherit = true) where T : Attribute
		{
			return (T[])provider.GetCustomAttributes(typeof(T), inherit);
		}

		public static T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit = true) where T : Attribute
		{
			var attrs = provider.GetAttributes<T>(inherit);
			return attrs.Length > 0 ? attrs[0] : null;
		}
	}

	internal static class DictionaryExtensions
	{
		public static T Get<T>(this IDictionary<string, object> dictionary, string key) where T : class
		{
			object value;
			return dictionary.TryGetValue(key, out value) ? value.ToType<T>() : null;
		}
	}

	internal static class ConvertExtensions
	{
		public static object ToType(this object value, Type type)
		{
			if (type.IsInstanceOfType(value))
			{
				return value;
			}

			if (type == typeof(Guid))
			{
				return new Guid(Convert.ToString(value, CultureInfo.InvariantCulture));
			}

			if (type.IsEnum)
			{
				return value.ToEnum(type);
			}

			var c = value as IConvertible;
			if (c != null)
			{
				return Convert.ChangeType(value, type);
			}

			var converter = TypeDescriptor.GetConverter(type);
			return converter.ConvertFrom(value);
		}

		public static T ToType<T>(this object value)
		{
			return (T)value.ToType(typeof(T));
		}

		public static object ToEnum(this object value, Type type)
		{
			if (value == null)
			{
				return Activator.CreateInstance(type);
			}

			var s = value as string;
			if (s != null)
			{
				var converter = TypeDescriptor.GetConverter(type);
				return converter.ConvertFrom(value);
			}
			return Enum.ToObject(type, value);
		}

		public static T ToEnum<T>(this object value)
		{
			return (T)value.ToEnum(typeof(T));
		}
	}
}
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Client.Utility
{
	public static class EnvironmentEx
	{
		public static string GetEnvironmentVariableOrThrow(string variableName)
		{
			var value = Environment.GetEnvironmentVariable(variableName);
			if (string.IsNullOrEmpty(value))
			{
				throw new InvalidOperationException($"Environment variable '{variableName}' is not set.");
			}
			return value;
		}

		public static T GetEnvironmentVariableOrThrow<T>(string variableName)
		{
			var value = Environment.GetEnvironmentVariable(variableName);
			if (string.IsNullOrEmpty(value))
			{
				throw new InvalidOperationException($"Environment variable '{variableName}' is not set.");
			}
			return ConvertValue<T>(value);
		}

		private static T ConvertValue<T>(string value)
		{
			var targetType = typeof(T);

			// string
			if (targetType == typeof(string))
				return (T)(object)value;

			// enums
			if (targetType.IsEnum)
				return (T)Enum.Parse(targetType, value, ignoreCase: true);

			// basic primitives (int, bool, double, float, decimal, etc.)
			return (T)Convert.ChangeType(
				value,
				targetType,
				CultureInfo.InvariantCulture);
		}
	}
}

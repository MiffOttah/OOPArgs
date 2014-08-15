using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.OOPArgs
{
    /// <summary>
    /// Helper methods for parsing strings based on the given type.
    /// </summary>
    internal static class StringToObjectParser
    {
        internal static bool ParseStringToObject(string strValue, Type propertyType, IFormatProvider formatProvider, out object value)
        {
            switch (propertyType.FullName)
            {
                case "System.String":
                    value = strValue;
                    return true;

                case "System.Boolean":
                    value = ParseBoolean(strValue);
                    return true;

                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                    try
                    {
                        value = Convert.ChangeType(strValue, propertyType, formatProvider);
                        return true;
                    }
                    catch (FormatException)
                    {
                        // it didn't parse
                        value = null;
                        return false;
                    }

                default:
                    throw new InvalidArgumentTypeException(propertyType);
            }
        }

        private static bool ParseBoolean(string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue)) return false;

            switch (strValue.ToUpperInvariant().Trim())
            {
                case "FALSE":
                case "NO":
                case "OFF":
                case "N":
                case "F":
                    return false;

                default:
                    return true;
            }
        }
    }
}

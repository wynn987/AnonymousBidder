using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousBidder.Common
{
    public class EnumHelper
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// Get all the names
        public static IEnumerable<T> GetNames<T>()
        {
            return Enum.GetNames(typeof(T)).Cast<T>();
        }

        /// Get the name for the enum value
        public static string GetName<T>(T enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        /// Get the underlying value for the Enum string
        public static int GetValue<T>(string enumString)
        {
            return (int)Enum.Parse(typeof(T), enumString.Trim());
        }

        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();
             
            if (name == null)
            {
                return string.Empty;
            }
            var field = type.GetField(name);
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }
        public static string GetDisplayValue<T>(string value)
        {
            Type type = typeof(T);

            var fieldInfo = type.GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
            typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}

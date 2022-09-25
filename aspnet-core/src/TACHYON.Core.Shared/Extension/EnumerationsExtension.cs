using Abp.Localization;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TACHYON
{
    public static class EnumerationsExtension
    {
        public static string GetEnumDescription(this Enum value)
        {
            try
            {
                // Get the Description attribute value for the enum value
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                    return attributes[0].Description;
                else
                    return value.ToString();
            }
            catch
            {
                return "";
            }
        }

    }

    public static class EnumLocalizationExtension
    {
        public static string ToLocalizedDisplayName(this Enum value)
        {
            var displayName = value.ToString();
            var fieldInfo = value.GetType().GetField(displayName);
            if (fieldInfo != null)
            {
                var attribute = fieldInfo.GetCustomAttributes(typeof(AbpEnumDisplayNameAttribute), true)
                    .Cast<AbpEnumDisplayNameAttribute>().Single();

                if (attribute != null)
                {
                    displayName = attribute.DisplayName;
                }
            }
            return displayName;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AbpEnumDisplayNameAttribute : AbpDisplayNameAttribute
    {
        /// <summary>
        /// <see cref="AbpDisplayNameAttribute"/> for enum values.
        /// </summary>
        public AbpEnumDisplayNameAttribute(string sourceName, string key) : base(sourceName, key)
        {
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;

namespace TACHYON.Extension
{
    public static class CustomExtensions
    {

        #region String

        //public static bool ToLowerContains(this String str, string s)
        //{
        //    return str.ToLower().Contains(s.ToLower());

        //}

        #endregion

        public static List<FieldInfo> GetAllPublicConstants(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .ToList();
        }
        /// <summary>
        /// This Method is Used To Convert Enum Type To it Display Name
        /// Please Don't Use it for Any Other Types
        /// </summary>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetStringOfPropertyValue(this string type, string property, object val)
        {
            var mType = Type.GetType(type);
            var obj = Activator.CreateInstance(mType);

            // Here We Want To Get Enum Base Type 
            // To Know What Value Type Can Assignable For This Enum
            var propInfo = mType.GetProperty(property);
            if (propInfo == null)
                throw new ArgumentNullException(property, $"There is No {property} in This Type {type}");

            var baseType = Enum.GetUnderlyingType(propInfo.PropertyType);
            val = Convert.ChangeType(val, baseType);
            propInfo.SetValue(obj, val);

            return propInfo.GetValue(obj)?.ToString();
        }
    }


}
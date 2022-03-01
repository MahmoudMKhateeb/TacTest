using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Rating;

namespace TACHYON.Extension
{
    public static class CustomExtensions
    {

        #region String

        public static bool ToLowerContains(this String str, string s)
        {
            return str.ToLower().Contains(s.ToLower());

        }
        public static bool ContainsOther(this IHasKey entityHasKey)
         => entityHasKey.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower());
        #endregion



        public static void SeedEntity<TEntity>(this DbSet<TEntity> context)
            where TEntity : FullAuditedEntity, IHasKey, new()
        {
            if (context.Any(OthersExpressions.ContainsOthersKeyExpression)) return;
            var entity = new TEntity()
            { CreationTime = DateTime.Now, IsDeleted = false, Key = TACHYONConsts.OthersDisplayName };
            context.Add(entity);
        }


        public static void CheckTranslation<TEntity, TTranslation, TKey>(this DbSet<TEntity> context, DbSet<TTranslation> transContext
            , TKey key)
            where TEntity : FullAuditedEntity<TKey>, IHasKey, IMultiLingualEntity<TTranslation>
            where TTranslation : class, IEntityTranslation<TEntity, TKey>, IHasDisplayName, new()
            where TKey : IComparable<TKey>
        {
            var entityWithoutTranslation = context
                    .FirstOrDefault(x => x.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()));
            if (entityWithoutTranslation == null) return;
            var translations = new List<TTranslation>()
            {
                new TTranslation() {DisplayName = "Others",Language = "en",CoreId = entityWithoutTranslation.Id},
                new TTranslation() {DisplayName = "أخرى",Language = "ar-EG",CoreId = entityWithoutTranslation.Id}
            };
            transContext.AddRange(translations);
        }


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

        public static decimal GetEntityRatingAverage(this IEnumerable<Tuple<decimal, RateType>> ratingLogs, Func<Tuple<decimal, RateType>, bool> selector)
        {
            var entityRatingLogs = ratingLogs.Where(selector).ToList();
            if (!entityRatingLogs.Any()) return 0;

            return entityRatingLogs.Sum(x => x.Item1) / entityRatingLogs.Count;
        }


        //        Usage:
        //Console.WriteLine(1.In(2, 1, 3));
        //Console.WriteLine(1.In(2, 3));
        //Console.WriteLine(UserStatus.Active.In(UserStatus.Removed, UserStatus.Banned));
        public static bool In<T>(this T val, params T[] values) where T : struct
        {
            return values.Contains(val);
        }

    }

}
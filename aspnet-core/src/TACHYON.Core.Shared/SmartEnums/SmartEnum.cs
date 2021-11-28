using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TACHYON.SmartEnums
{
    public abstract class SmartEnum : IComparable
    {
        public string DisplayName { get; }

        public int Value { get; }

        protected SmartEnum(string displayName, int value)
        {
            DisplayName = displayName;
            Value = value;
        }

        public static IEnumerable<T> GetAll<T>() where T : SmartEnum
        {
            var fields = typeof(T).GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SmartEnum otherValue))
                return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Value.GetHashCode();


        public static bool TryGetFromValueOrName<T>(
            string valueOrName,
            out T smartEnum)
            where T : SmartEnum
        {
            return TryParse(item => item.DisplayName == valueOrName, out smartEnum) ||
                   int.TryParse(valueOrName, out var value) &&
                   TryParse(item => item.Value == value, out smartEnum);
        }

        public static T FromValue<T>(int value) where T : SmartEnum
        {
            var matchingItem = Parse<T, int>(value, "nameOrValue", item => item.Value == value);
            return matchingItem;
        }

        public static T FromName<T>(string name) where T : SmartEnum
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.DisplayName == name);
            return matchingItem;
        }

        private static bool TryParse<TSmartEnum>(
            Func<TSmartEnum, bool> predicate,
            out TSmartEnum smartEnum)
            where TSmartEnum : SmartEnum
        {
            smartEnum = GetAll<TSmartEnum>().FirstOrDefault(predicate);
            return smartEnum != null;
        }

        private static TSmartEnum Parse<TSmartEnum, TIntOrString>(
            TIntOrString nameOrValue,
            string description,
            Func<TSmartEnum, bool> predicate)
            where TSmartEnum : SmartEnum
        {
            var matchingItem = GetAll<TSmartEnum>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                throw new InvalidOperationException(
                    $"'{nameOrValue}' is not a valid {description} in {typeof(SmartEnum)}");
            }

            return matchingItem;
        }

        public int CompareTo(object other) => Value.CompareTo(((SmartEnum)other).Value);


        public override string ToString()
            => DisplayName;
    }
}
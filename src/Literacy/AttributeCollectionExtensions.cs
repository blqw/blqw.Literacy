using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.Reflection
{
    public static class AttributeCollectionExtensions
    {
        public static bool Exists(this IEnumerable<Attribute> attributes, Type attributeType)
            => attributes?.Any(x => attributeType.IsInstanceOfType(x)) ?? false;

        public static bool Exists<T>(this IEnumerable<Attribute> attributes)
            => attributes?.OfType<T>().Any() ?? false;

        public static bool Exists<T>(this IEnumerable<Attribute> attributes, Func<T, bool> predicate)
            => attributes?.OfType<T>().Any(predicate) ?? false;

        public static IEnumerable<Attribute> Where(this IEnumerable<Attribute> attributes, Type attributeType)
            => attributes?.Where(x => attributeType.IsInstanceOfType(x)) ?? Array.Empty<Attribute>();

        public static IEnumerable<T> Where<T>(this IEnumerable<Attribute> attributes)
            => attributes?.OfType<T>() ?? Array.Empty<T>();

        public static IEnumerable<T> Where<T>(this IEnumerable<Attribute> attributes, Func<T, bool> predicate)
            => attributes?.OfType<T>().Where(predicate) ?? Array.Empty<T>();

        public static Attribute First(this IEnumerable<Attribute> attributes, Type attributeType)
            => attributes?.FirstOrDefault(x => attributeType.IsInstanceOfType(x));

        public static T First<T>(this IEnumerable<Attribute> attributes)
            => attributes?.FirstOrDefault(x => x is T) is T t ? t : default;

        public static T First<T>(this IEnumerable<Attribute> attributes, Func<T, bool> predicate)
            => attributes?.FirstOrDefault(x => x is T v ? predicate(v) : false) is T t ? t : default;
    }
}

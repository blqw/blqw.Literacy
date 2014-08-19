using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public static class TypesHelper
    {
        private static readonly Dictionary<Type, TypeInfo> _cache = CreateCache();

        private static Dictionary<Type, TypeInfo> CreateCache()
        {
            var count = 0;
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                count += ass.GetTypes().Length;
            }
            return new Dictionary<Type, TypeInfo>(count);
        }

        public static TypeInfo GetTypeInfo(
#if !NF2
this
#endif
Type type)
        {
            if (type == null)
            {
                return null;
            }
            TypeInfo info;
            if (_cache.TryGetValue(type, out info))
            {
                return info;
            }
            lock (_cache)
            {
                if (_cache.TryGetValue(type, out info) == false)
                {
                    info = new TypeInfo(type);
                    _cache.Add(type, info);
                }
                return info;
            }
        }

        /// <summary> 指示类型是否是数字类型(不考虑溢出的情况下是否可以强转成任意数字类型)
        /// </summary>
        public static bool IsNumberType(
#if !NF2
this
#endif
Type t)
        {
            if (t == null)
            {
                return false;
            }
            if (t.IsEnum)
            {
                return true;
            }
            var code = Type.GetTypeCode(t);
            if (code >= TypeCode.SByte && code <= TypeCode.Decimal)
            {
                return true;
            }
            var type = Nullable.GetUnderlyingType(t);
            if (type == null)
            {
                return false;
            }
            return IsNumberType(type);
        }

        /// <summary> 检查一个类型是否为可空值类型
        /// </summary>
        public static bool IsNullable(
#if !NF2
this
#endif
Type t)
        {
            if (t == null)
            {
                return false;
            }
            return t.IsValueType
                && t.IsGenericType
                && t.IsGenericTypeDefinition == false
                && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary> 检查当前类型和指定类型是否有继承关系
        /// </summary>
        /// <param name="parent">当前类型</param>
        /// <param name="child">指定类型</param>
        public static bool IsChild(
#if !NF2
this
#endif
Type parent, Type child)
        {
            return parent != null && parent.IsAssignableFrom(child);
        }

        /// <summary> 检查指定对象是否是当前类型(或其子类类型)的实例
        /// </summary>
        /// <param name="parent">当前类型(父类)</param>
        /// <param name="obj">指定对象</param>
        /// <returns>存在继承关系返回true,否则返回false</returns>
        public static bool IsChild(
#if !NF2
this
#endif
Type parent, object obj)
        {
            return parent != null && parent.IsInstanceOfType(obj);
        }

        /// <summary> 获取类型的拓展枚举
        /// </summary>
        public static TypeCodes GetTypeCodes(
#if !NF2
this
#endif
Type t)
        {
            if (t == null)
            {
                return TypeCodes.Empty;
            }
            return GetTypeInfo(t).TypeCodes;
        }

        ///<summary> 获取类型名称的友好展现形式
        /// </summary>
        /// <param name="t"></param>
        public static string DisplayName(
#if !NF2
this
#endif
Type t)
        {
            if (t == null)
            {
                return "null";
            }
            return GetTypeInfo(t).DisplayName;
        }

        /// <summary> 获取类型是否属于特殊类型
        /// 除了基元类型以外,Guid,TimeSpan,DateTime,DBNull,所有指针,以及这些类型的可空值类型,都属于特殊类型
        /// </summary>
        public static bool IsSpecialType(
#if !NF2
            this
#endif
Type t)
        {
            if (t == null)
            {
                return false;
            }
            if (t.IsPrimitive)
            {
                return true;
            }
            return (int)GetTypeInfo(t).TypeCodes < 100;
        }
    }


}

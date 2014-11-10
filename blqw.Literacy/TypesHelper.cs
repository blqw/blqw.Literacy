using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary> 存放用于处理Type对象的静态方法
    /// </summary>
    public static class TypesHelper
    {
        private static class GenericCache<T>
        {
            public readonly static TypeInfo TypeInfo = GetTypeInfo(typeof(T));
        }

        private static readonly Dictionary<Type, TypeInfo> Cache = new Dictionary<Type, TypeInfo>();

        /// <summary> 获取TypeInfo对象
        /// </summary>
        /// <typeparam name="T">用于构造TypeInfo对象的Type类型</typeparam>
        /// <returns></returns>
        public static TypeInfo GetTypeInfo<T>()
        {
            return GenericCache<T>.TypeInfo;
        }


        /// <summary> 获取TypeInfo对象
        /// </summary>
        /// <param name="type">用于构造TypeInfo对象的Type类型实例,不可为null</param>
        /// <returns></returns>
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
            var key = type;
            TypeInfo info;
            if (Cache.TryGetValue(key, out info))
            {
                return info;
            }
            lock (Cache)
            {
                if (Cache.TryGetValue(key, out info) == false)
                {
                    info = new TypeInfo(type);
                    Cache.Add(key, info);
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

        /// <summary> 指示对象是否是数字类型
        /// </summary>
        public static bool IsNumber(object obj)
        {
            if (obj is Enum)
            {
                return true;
            }
            var conv = obj as IConvertible;
            if (conv != null)
            {
                var code = conv.GetTypeCode();
                return code >= TypeCode.SByte && code <= TypeCode.Decimal;
            }
            return false;
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
        /// 除了基元类型以外String,Guid,TimeSpan,DateTime,DBNull,所有指针,以及这些类型的可空值类型,都属于特殊类型
        /// <para>(基元类型包括Boolean、Byte、SByte、Int16、UInt16、Int32、UInt32、Int64、UInt64、IntPtr、UIntPtr、Char、Double 和 Single)</para>
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
            var codes = (int)GetTypeInfo(t).TypeCodes;
            return codes > 2 && codes < 100;
        }

    }


}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary> 用于拓展系统Type对象的属性和方法
    /// </summary>
    [DebuggerDisplay("Type: {DisplayName} TypeCodes: {TypeCodes} TypeCode: {TypeCode}")]
    public sealed class TypeInfo
    {
        /// <summary> 构造用于拓展系统Type对象的属性和方法的对象
        /// </summary>
        /// <param name="type">用于构造TypeInfo的Type对象实例,不可为null</param>
        internal TypeInfo(System.Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            Type = type;
            TypeCode = System.Type.GetTypeCode(type);
            IsArray = type.IsArray;

            IsMakeGenericType = type.IsGenericType && !type.IsGenericTypeDefinition;
            var valueType = Nullable.GetUnderlyingType(type);
            if (valueType != null) //判断可空值类型
            {
                IsNullable = true;
                NullableUnderlyingType = TypesHelper.GetTypeInfo(valueType);
                type = valueType;
            }

            if (type.IsEnum)
            {
                IsNumberType = true;
                EnumUnderlyingType = TypesHelper.GetTypeInfo(Enum.GetUnderlyingType(Type));
            }
            else if (IsNullable)
            {
                IsNumberType = NullableUnderlyingType.IsNumberType;
            }
            else
            {
                IsNumberType = (TypeCode >= TypeCode.SByte && TypeCode <= TypeCode.Decimal);
            }
        }

        #region Literacy

        private Literacy _literacy;

        /// <summary> 获取严格区分大小写的Literacy对象
        /// </summary>
        public Literacy Literacy
        {
            get
            {
                if (_literacy != null)
                {
                    return _literacy;
                }
                lock (this)
                {
                    if (_literacy == null)
                    {
                        _literacy = new Literacy(this, false);
                    }
                }
                return _literacy;
            }
        }

        private Literacy _ignoreCaseLiteracy;

        /// <summary> 获取不区分大小写的Literacy对象
        /// </summary>
        public Literacy IgnoreCaseLiteracy
        {
            get
            {
                if (_ignoreCaseLiteracy != null)
                {
                    return _ignoreCaseLiteracy;
                }
                lock (this)
                {
                    if (_ignoreCaseLiteracy == null)
                    {
                        _ignoreCaseLiteracy = new Literacy(this, true);
                    }
                }
                return _ignoreCaseLiteracy;
            }
        }

        #endregion

        /// <summary> 获取当前类型
        /// </summary>
        public readonly System.Type Type;
        /// <summary> 如果IsNullable为true, 该字段获取可空值类型的实际类型, 否则为空
        /// </summary>
        public readonly TypeInfo NullableUnderlyingType;
        /// <summary> 如果Type.IsEnum为true, 该字段获取枚举类型的实际类型, 否则为空
        /// </summary>
        public readonly TypeInfo EnumUnderlyingType;
        /// <summary> 是否为可空值类型
        /// </summary>
        public readonly bool IsNullable;
        /// <summary> 获取一个值，通过该值指示 System.Type 是否为数组。
        /// </summary>
        public readonly bool IsArray;
        /// <summary> 获取一个值，该值指示当前类型是否是已经构造完成的泛型类型。
        /// </summary>
        public readonly bool IsMakeGenericType;

        /// <summary> 指示当前类型是否是数字类型(不考虑溢出的情况下是否可以强转成任意数字类型)
        /// </summary>
        public readonly bool IsNumberType;

        /// <summary> 获取类型枚举
        /// </summary>
        public readonly TypeCode TypeCode;

        private TypeCodes _typeCodes = (blqw.TypeCodes)(-1);
        /// <summary> 获取类型的拓展枚举
        /// </summary>
        public TypeCodes TypeCodes
        {
            get
            {
                if (_typeCodes < 0)
                {
                    _typeCodes = GetTypeCodes();
                }
                return _typeCodes;
            }
        }

        private int _isSpecialType;
        /// <summary> 获取一个值，通过该值指示类型是否属于特殊类型
        /// 除了基元类型以外,Guid,TimeSpan,DateTime,DBNull,所有指针,以及这些类型的可空值类型,都属于特殊类型
        /// </summary>
        public bool IsSpecialType
        {
            get
            {
                if (_isSpecialType == 0)
                {
                    if (Type.IsPrimitive)
                    {
                        _isSpecialType = 1;
                    }
                    else
                    {
                        var codes = (int)TypeCodes;
                        if (codes > 2 && codes < 100)
                        {
                            _isSpecialType = 1;
                        }
                        else
                        {
                            _isSpecialType = 2;
                        }
                    }
                }
                return _isSpecialType == 1;
            }
        }

        private string _displayName;
        /// <summary> 获取当前类型名称的友好展现形式
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (_displayName == null)
                {
                    _displayName = GetDisplayName();
                }
                return _displayName;
            }
        }

        private TypeInfo[] _genericArgumentsTypeInfo;

        /// <summary> 获取当前泛型类型的泛型参数信息,如果不是已构造的泛型类型,返回null
        /// </summary>
        public TypeInfo[] GenericArgumentsTypeInfo
        {
            get
            {
                if (!IsMakeGenericType)
                {
                    return null;
                }
                if (_genericArgumentsTypeInfo != null)
                {
                    return _genericArgumentsTypeInfo;
                }
                lock (this)
                {
                    if (IsMakeGenericType && _genericArgumentsTypeInfo == null)
                    {
                        var args = Type.GetGenericArguments();
                        _genericArgumentsTypeInfo = new TypeInfo[args.Length];
                        for (int i = 0; i < args.Length; i++)
                        {
                            _genericArgumentsTypeInfo[i] = TypesHelper.GetTypeInfo(args[i]);
                        }
                    }
                }
                return _genericArgumentsTypeInfo;
            }
        }

        /// <summary> 数据类型转换,失败返回false
        /// </summary>
        public LiteracyTryParse TryParse
        {
            get
            {
                if (_tryParse == null)
                {
                    _tryParse = CreateDelegate();
                }
                return _tryParse;
            }
        }

        /// <summary> 数据类型转换,失败抛出异常
        /// </summary>
        /// <param name="input">等待转换的数据</param>
        public object Convert(object input)
        {
            object value;
            if (TryParse(input, out value))
            {
                return value;
            }
            throw new InvalidCastException(string.Concat("值 '", ((object)input ?? "<NULL>").ToString(), "' 无法转为 ", DisplayName, " 类型"));
        }

        #region 私有方法

        private LiteracyTryParse _tryParse;

        /// <summary> 获取当前类型的 TypeCodes 值
        /// </summary>
        private TypeCodes GetTypeCodes()
        {
            if (IsNullable) //可空值类型
            {
                return NullableUnderlyingType.GetTypeCodes();
            }
            if (Type.IsEnum)
            {
                return blqw.TypeCodes.Enum;
            }
            if (Type.IsArray)
            {
                return blqw.TypeCodes.IList;
            }
            if (IsMakeGenericType && Type.Name.StartsWith("<>f__AnonymousType")) //判断匿名类
            {
                return TypeCodes.AnonymousType;
            }

            var interfaces =  Type.GetInterfaces();
            var length = interfaces.Length;
            for (int i = 0; i < length; i++)
            {
                var inf = interfaces[i];
                if (inf.IsGenericTypeDefinition)
                {
                    
                }
                else if (inf.IsGenericType)
                {
                    inf = inf.GetGenericTypeDefinition();
                }
                else
                {
                    continue;
                }
                if (inf == typeof(IList<>))
                {
                    return TypeCodes.IListT;
                }
                else if (inf == typeof(IDictionary<,>))
                {
                    return TypeCodes.IDictionaryT;
                }
            }

            if (TypeCode == TypeCode.Object)
            {
                if (Type == typeof(TimeSpan))
                {
                    return TypeCodes.TimeSpan;
                }
                else if (Type == typeof(Guid))
                {
                    return TypeCodes.Guid;
                }
                else if (Type == typeof(System.Text.StringBuilder))
                {
                    return TypeCodes.StringBuilder;
                }
                else if (Type == typeof(System.Data.DataSet))
                {
                    return TypeCodes.DataSet;
                }
                else if (Type == typeof(System.Data.DataTable))
                {
                    return TypeCodes.DataTable;
                }
                else if (Type == typeof(System.Data.DataView))
                {
                    return TypeCodes.DataView;
                }
                else if (Type == typeof(IntPtr))
                {
                    return TypeCodes.IntPtr;
                }
                else if (Type == typeof(UIntPtr))
                {
                    return TypeCodes.UIntPtr;
                }
                else if (Type == typeof(System.Xml.XmlDocument))
                {
                    return TypeCodes.Xml;
                }
                else if (typeof(System.Collections.IList).IsAssignableFrom(Type))
                {
                    return TypeCodes.IList;
                }
                else if (typeof(System.Collections.IDictionary).IsAssignableFrom(Type))
                {
                    return TypeCodes.IDictionary;
                }
                else if (typeof(System.Data.Common.DbDataReader).IsAssignableFrom(Type))
                {
                    return TypeCodes.DbDataReader;
                }
                else if (typeof(System.Data.Common.DbParameter).IsAssignableFrom(Type))
                {
                    return TypeCodes.DbParameter;
                }
                else if (typeof(Type).IsAssignableFrom(Type))
                {
                    return TypeCodes.Type;
                }
            }
            return (TypeCodes)TypeCode;
        }

        ///<summary> 获取类型名称的友好展现形式
        /// </summary>
        private string GetDisplayName()
        {
            if (IsNullable)
            {
                return NullableUnderlyingType.DisplayName + "?";
            }
            if (Type.IsGenericType)
            {
                string[] generic;
                if (Type.IsGenericTypeDefinition) //泛型定义
                {
                    var args = Type.GetGenericArguments();
                    generic = new string[args.Length];
                }
                else
                {
                    var infos = GenericArgumentsTypeInfo;
                    generic = new string[infos.Length];
                    for (int i = 0; i < infos.Length; i++)
                    {
                        generic[i] = infos[i].DisplayName;
                    }
                }
                return GetSimpleName(Type) + "<" + string.Join(", ", generic) + ">";
            }
            else
            {
                return GetSimpleName(Type);
            }
        }

        private static string GetSimpleName(Type t)
        {
            string name;
            switch (t.Namespace)
            {
                case "System":
                case "System.Collections":
                case "System.Collections.Generic":
                case "System.Data":
                case "System.Text":
                    name = t.Name;
                    break;
                default:
                    name = t.Namespace + "." + t.Name;
                    break;
            }
            var index = name.LastIndexOf('`');
            if (index > -1)
            {
                name = name.Remove(index);
            }
            return name;
        }

        private LiteracyTryParse CreateDelegate()
        {
            switch (TypeCodes)
            {
                case TypeCodes.Empty:
                    return (object input, out object result) => {
                        result = null;
                        return (input == null || input is DBNull);
                    };
                case TypeCodes.DBNull:
                    return (object input, out object result) => {
                        if (input == null || input is DBNull)
                        {
                            result = DBNull.Value;
                            return true;
                        }
                        result = null;
                        return false;
                    };
                case TypeCodes.Boolean:
                    return CreateDelegate<Boolean>(Convert2.TryParseBoolean, IsNullable);
                case TypeCodes.Char:
                    return CreateDelegate<Char>(Convert2.TryParseChar, IsNullable);
                case TypeCodes.SByte:
                    return CreateDelegate<SByte>(Convert2.TryParseSByte, IsNullable);
                case TypeCodes.Byte:
                    return CreateDelegate<Byte>(Convert2.TryParseByte, IsNullable);
                case TypeCodes.Int16:
                    return CreateDelegate<Int16>(Convert2.TryParseInt16, IsNullable);
                case TypeCodes.UInt16:
                    return CreateDelegate<UInt16>(Convert2.TryParseUInt16, IsNullable);
                case TypeCodes.Int32:
                    return CreateDelegate<Int32>(Convert2.TryParseInt32, IsNullable);
                case TypeCodes.UInt32:
                    return CreateDelegate<UInt32>(Convert2.TryParseUInt32, IsNullable);
                case TypeCodes.Int64:
                    return CreateDelegate<Int64>(Convert2.TryParseInt64, IsNullable);
                case TypeCodes.UInt64:
                    return CreateDelegate<UInt64>(Convert2.TryParseUInt64, IsNullable);
                case TypeCodes.Single:
                    return CreateDelegate<Single>(Convert2.TryParseSingle, IsNullable);
                case TypeCodes.Double:
                    return CreateDelegate<Double>(Convert2.TryParseDouble, IsNullable);
                case TypeCodes.Decimal:
                    return CreateDelegate<Decimal>(Convert2.TryParseDecimal, IsNullable);
                case TypeCodes.DateTime:
                    return CreateDelegate<DateTime>(Convert2.TryParseDateTime, IsNullable);
                case TypeCodes.String:
                    return CreateDelegate<String>(Convert2.TryParseString, IsNullable);
                case TypeCodes.Guid:
                    return CreateDelegate<Guid>(Convert2.TryParseGuid, IsNullable);
                case TypeCodes.TimeSpan:
                    return CreateDelegate<TimeSpan>(Convert2.TryParseTimeSpan, IsNullable);
                case TypeCodes.IntPtr:
                    return CreateDelegate<IntPtr>(Convert2.TryParseIntPtr, IsNullable);
                case TypeCodes.UIntPtr:
                    return CreateDelegate<UIntPtr>(Convert2.TryParseUIntPtr, IsNullable);
                case TypeCodes.Enum:
                    var type = IsNullable ? NullableUnderlyingType.Type : Type;
                    var parse = Convert2.TryParseEnumMethod.MakeGenericMethod(type);
                    var create = this.GetType().GetMethod("CreateDelegate", BindingFlags.NonPublic | BindingFlags.Static);
                    create = create.MakeGenericMethod(type);
                    return (LiteracyTryParse)create.Invoke(null, new object[] { Delegate.CreateDelegate(typeof(LiteracyTryParse<>).MakeGenericType(type), parse), IsNullable });
                default:
                    if (IsNullable)
                    {
                        type = NullableUnderlyingType.Type;
                        return (object input, out object result) => {
                            if (input == null || input is DBNull)
                            {
                                result = null;
                                return true;
                            }
                            else if (type.IsInstanceOfType(input))
                            {
                                result = input;
                                return true;
                            }
                            result = null;
                            return false;
                        };
                    }
                    else
                    {
                        type = Type;
                        return (object input, out object result) => {
                            if (input == null || input is DBNull)
                            {
                                if (type.IsClass)
                                {
                                    result = null;
                                    return true;
                                }
                            }
                            else if (type.IsInstanceOfType(input))
                            {
                                result = input;
                                return true;
                            }
                            result = null;
                            return false;
                        };
                    }
            }
        }

        private static LiteracyTryParse CreateDelegate<T>(LiteracyTryParse<T> tryParse, bool isnullable)
        {
            if (isnullable)
            {
                return (object input, out object result) => {
                    if (input == null || input is DBNull)
                    {
                        result = null;
                        return true;
                    }
                    T t;
                    if (tryParse(input, out t))
                    {
                        result = t;
                        return true;
                    }
                    result = null;
                    return false;
                };
            }
            return (object input, out object result) => {
                T t;
                if (tryParse(input, out t))
                {
                    result = t;
                    return true;
                }
                result = null;
                return false;
            };
        }


        #endregion

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            var info = obj as TypeInfo;
            if (info == null)
            {
                return false;
            }
            return object.ReferenceEquals(this.Type, info.Type);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

    }
}

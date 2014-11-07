using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace blqw
{
    /// <summary> 将一个基本数据类型转换为另一个基本数据类型。
    /// </summary>
    public static class Convert2
    {

        #region DbType

        public static DbType TypeCodesToDbType(TypeCodes typeCodes)
        {
            switch (typeCodes)
            {
                case TypeCodes.Boolean:
                    return DbType.Boolean;
                case TypeCodes.Byte:
                    return DbType.Byte;
                case TypeCodes.Char:
                    return DbType.Boolean;
                case TypeCodes.DBNull:
                    return DbType.Object;
                case TypeCodes.DateTime:
                    return DbType.DateTime;
                case TypeCodes.Decimal:
                    return DbType.Decimal;
                case TypeCodes.Double:
                    return DbType.Double;
                case TypeCodes.Empty:
                    return DbType.Object;
                case TypeCodes.Int16:
                    return DbType.Int16;
                case TypeCodes.Int32:
                    return DbType.Int32;
                case TypeCodes.Int64:
                    return DbType.Int64;
                case TypeCodes.SByte:
                    return DbType.SByte;
                case TypeCodes.Single:
                    return DbType.Single;
                case TypeCodes.String:
                    return DbType.String;
                case TypeCodes.UInt16:
                    return DbType.UInt16;
                case TypeCodes.UInt32:
                    return DbType.UInt32;
                case TypeCodes.UInt64:
                    return DbType.UInt64;
                case TypeCodes.Guid:
                    return DbType.Guid;
                case TypeCodes.TimeSpan:
                    return DbType.Time;
                case TypeCodes.StringBuilder:
                    return DbType.String;
                case TypeCodes.Enum:
                    return DbType.Int32;
                case TypeCodes.Xml:
                    return DbType.Xml;
                default:
                    break;
            }
            throw new InvalidCastException("无法将" + typeCodes.ToString() + "转换为DbType");
        }

        public static DbType TypeToDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.Boolean;
                case TypeCode.DBNull:
                    return DbType.Object;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Empty:
                    return DbType.Object;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                case TypeCode.Object:
                default:
                    break;
            }
            if (type == typeof(Guid))
            {
                return DbType.Guid;
            }
            else if (type == typeof(byte[]))
            {
                return DbType.Binary;
            }
            else if (type == typeof(System.Xml.XmlDocument))
            {
                return DbType.Xml;
            }
            return DbType.Object;
        }

        public static Type DbTypeToType(DbType dbtype)
        {
            switch (dbtype)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof(String);
                case DbType.Binary:
                    return typeof(Byte[]);
                case DbType.Boolean:
                    return typeof(Boolean);
                case DbType.Byte:
                    return typeof(Byte);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return typeof(DateTime);
                case DbType.Decimal:
                case DbType.VarNumeric:
                case DbType.Currency:
                    return typeof(Decimal);
                case DbType.Double:
                    return typeof(Double);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(Int16);
                case DbType.Int32:
                    return typeof(Int32);
                case DbType.Int64:
                    return typeof(Int64);
                case DbType.Object:
                    return typeof(Object);
                case DbType.SByte:
                    return typeof(SByte);
                case DbType.Single:
                    return typeof(Single);
                case DbType.UInt16:
                    return typeof(UInt16);
                case DbType.UInt32:
                    return typeof(UInt32);
                case DbType.UInt64:
                    return typeof(UInt64);
                case DbType.Xml:
                    return typeof(System.Xml.XmlDocument);
                default:
                    throw new InvalidCastException("无效的DbType值:" + dbtype.ToString());
            }
        }

        #endregion

        #region ToType
        public static bool ToBoolean(object input, bool defaultValue = default(bool), bool throwOnError = true)
        {
            bool value;
            if (TryParseBoolean(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static byte ToByte(object input, byte defaultValue = default(byte), bool throwOnError = true)
        {
            byte value;
            if (TryParseByte(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static char ToChar(object input, char defaultValue = default(char), bool throwOnError = true)
        {
            char value;
            if (TryParseChar(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static DateTime ToDateTime(object input, DateTime defaultValue = default(DateTime), bool throwOnError = true)
        {
            DateTime value;
            if (TryParseDateTime(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static TimeSpan ToTimeSpan(object input, TimeSpan defaultValue = default(TimeSpan), bool throwOnError = true)
        {
            TimeSpan value;
            if (TryParseTimeSpan(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static decimal ToDecimal(object input, decimal defaultValue = default(decimal), bool throwOnError = true)
        {
            decimal value;
            if (TryParseDecimal(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static double ToDouble(object input, double defaultValue = default(double), bool throwOnError = true)
        {
            double value;
            if (TryParseDouble(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static Guid ToGuid(object input, Guid defaultValue = default(Guid), bool throwOnError = true)
        {
            Guid value;
            if (TryParseGuid(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static byte[] ToBytes(object input, byte[] defaultValue = default(byte[]), bool throwOnError = true)
        {
            byte[] value;
            if (TryParseBytes(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static short ToInt16(object input, short defaultValue = default(short), bool throwOnError = true)
        {
            short value;
            if (TryParseInt16(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static int ToInt32(object input, int defaultValue = default(int), bool throwOnError = true)
        {
            int value;
            if (TryParseInt32(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static long ToInt64(object input, long defaultValue = default(long), bool throwOnError = true)
        {
            long value;
            if (TryParseInt64(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static sbyte ToSByte(object input, sbyte defaultValue = default(sbyte), bool throwOnError = true)
        {
            sbyte value;
            if (TryParseSByte(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static float ToSingle(object input, float defaultValue = default(float), bool throwOnError = true)
        {
            float value;
            if (TryParseSingle(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static ushort ToUInt16(object input, ushort defaultValue = default(ushort), bool throwOnError = true)
        {
            ushort value;
            if (TryParseUInt16(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static uint ToUInt32(object input, uint defaultValue = default(uint), bool throwOnError = true)
        {
            uint value;
            if (TryParseUInt32(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static ulong ToUInt64(object input, ulong defaultValue = default(ulong), bool throwOnError = true)
        {
            ulong value;
            if (TryParseUInt64(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static string ToString(object input, string defaultValue = null, bool throwOnError = true)
        {
            if (TryParseString(input, out defaultValue))
            {
                return defaultValue;
            }
            else
            {
                return input.ToString();
            }
        }
        public static TEnum ToEnum<TEnum>(object input, TEnum defaultValue, bool throwOnError = true)
                where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            TEnum value;
            if (TryParseEnum<TEnum>(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static Enum ToEnum(object input, Enum defaultValue, bool throwOnError = true)
        {
            Enum value;
            if (TryParseEnum(input, defaultValue.GetType(), out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static T ToObject<T>(object input, T defaultValue = default(T), bool throwOnError = true)
        {
            T value;
            if (GenericCache<T>.TryParse(input, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }
        public static object ChangedType(object input, Type outputType, object defaultValue = null, bool throwOnError = true)
        {
            object value;
            if (TryParseObject(input, outputType, out value))
            {
                return value;
            }
            if (throwOnError)
                ThrowError(input, defaultValue);
            return defaultValue;
        }

        private static void ThrowError<T>(object input, T value)
        {
            string x;
            if (input == null)
            {
                x = "<null>";
            }
            else if (input is DBNull)
            {
                x = "<DBNull>";
            }
            else
            {
                x = input.ToString();
            }
            var name = TypesHelper.GetTypeInfo<T>().DisplayName;
            throw new InvalidCastException(string.Concat("值 '", x, "' 无法转为 ", name, " 类型"));
        }
        #endregion

        #region TryParse
        public static bool TryParseBoolean(object input, out bool result)
        {
            if (input is bool)
            {
                result = (bool)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToBoolean(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null);
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = false;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null) != 0; return true;
                    case TypeCode.Char: result = conv.ToChar(null) != 0; return true;
                    case TypeCode.Int16: result = conv.ToInt16(null) != 0; return true;
                    case TypeCode.Int32: result = conv.ToInt32(null) != 0; return true;
                    case TypeCode.Int64: result = conv.ToInt64(null) != 0; return true;
                    case TypeCode.SByte: result = conv.ToSByte(null) != 0; return true;
                    case TypeCode.Double: result = conv.ToDouble(null) != 0; return true;
                    case TypeCode.Single: result = conv.ToSingle(null) != 0; return true;
                    case TypeCode.UInt16: result = conv.ToUInt16(null) != 0; return true;
                    case TypeCode.UInt32: result = conv.ToUInt32(null) != 0; return true;
                    case TypeCode.UInt64: result = conv.ToUInt64(null) != 0; return true;
                    case TypeCode.Decimal: result = conv.ToDecimal(null) != 0; return true;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = false;
                return false;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length != 1)
                    {
                        result = false;
                        return false;
                    }
                    result = BitConverter.ToBoolean(bs, 0);
                    return true;
                }
            }
            return StringToBoolean(input.ToString(), out result);
        }
        public static bool TryParseByte(object input, out byte result)
        {
            if (input is byte)
            {
                result = (byte)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToByte(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (byte)1 : (byte)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte:
                        result = conv.ToByte(null);
                        return true;
                    case TypeCode.Char:
                        result = (byte)conv.ToChar(null);
                        return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 255)
                            {
                                result = 0;
                                return false;
                            }
                            result = (byte)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length != 1)
                    {
                        result = 0;
                        return false;
                    }
                    result = bs[0];
                    return true;
                }
            }
            return StringToByte(input.ToString(), out result);
        }
        public static bool TryParseChar(object input, out char result)
        {
            if (input is char)
            {
                result = (char)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                if (str.Length == 0)
                {
                    result = default(char);
                    return false;
                }
                result = str[0];
                return true;
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? 'T' : 'F';
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = default(char);
                        return false;
                    case TypeCode.Byte:
                        result = (char)conv.ToByte(null);
                        return true;
                    case TypeCode.Char:
                        result = conv.ToChar(null);
                        return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 255)
                            {
                                result = default(char);
                                return false;
                            }
                            result = (char)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 2)
                    {
                        result = default(char);
                        return false;
                    }
                    result = BitConverter.ToChar(bs, 0);
                    return true;
                }
            }
            result = default(char);
            return false;
        }
        public static bool TryParseDateTime(object input, out DateTime result)
        {
            if (input is DateTime)
            {
                result = (DateTime)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToDateTime(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.DateTime:
                        result = conv.ToDateTime(null);
                        return true;
                    case TypeCode.Byte:
                    case TypeCode.Boolean:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Decimal:
                        result = default(DateTime);
                        return false;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = default(DateTime);
                return false;
            }
            return StringToDateTime(input.ToString(), out result);
        }
        public static bool TryParseTimeSpan(object input, out TimeSpan result)
        {
            if (input is TimeSpan)
            {
                result = (TimeSpan)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToTimeSpan(str, out result);
            }
            else if (input == null)
            {
                result = default(TimeSpan);
                return false;
            }
            return StringToTimeSpan(input.ToString(), out result);
        }
        public static bool TryParseDecimal(object input, out decimal result)
        {
            if (input is Decimal)
            {
                result = (Decimal)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToDecimal(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (Decimal)1 : (Decimal)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = (Decimal)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (Decimal)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (Decimal)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (Decimal)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (Decimal)conv.ToSByte(null); return true;
                    case TypeCode.Double: result = (Decimal)conv.ToDouble(null); return true;
                    case TypeCode.Single: result = (Decimal)conv.ToSingle(null); return true;
                    case TypeCode.UInt16: result = (Decimal)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (Decimal)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = (Decimal)conv.ToUInt64(null); return true;
                    case TypeCode.Decimal: result = (Decimal)conv.ToDecimal(null); return true;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            return StringToDecimal(input.ToString(), out result);
        }
        public static bool TryParseDouble(object input, out double result)
        {
            if (input is Double)
            {
                result = (Double)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToDouble(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (Double)1 : (Double)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = (Double)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (Double)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (Double)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (Double)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (Double)conv.ToSByte(null); return true;
                    case TypeCode.Double: result = (Double)conv.ToDouble(null); return true;
                    case TypeCode.Single: result = (Double)conv.ToSingle(null); return true;
                    case TypeCode.UInt16: result = (Double)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (Double)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = (Double)conv.ToUInt64(null); return true;
                    case TypeCode.Decimal: result = (Double)conv.ToDecimal(null); return true;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 8)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToDouble(bs, 0);
                    return true;
                }
            }
            return StringToDouble(input.ToString(), out result);
        }
        public static bool TryParseSingle(object input, out float result)
        {
            if (input is Single)
            {
                result = (Single)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToSingle(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (Single)1 : (Single)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte:
                        result = conv.ToByte(null);
                        return true;
                    case TypeCode.Char:
                        result = (Single)conv.ToChar(null);
                        return true;
                    case TypeCode.Int16: result = (Single)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (Single)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (Single)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (Single)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < Single.MinValue || a > Single.MaxValue)
                            {
                                result = 0;
                                return false;
                            }
                            result = (Single)a;
                            return true;
                        }
                    case TypeCode.Single: result = (Single)conv.ToSingle(null); return true;
                    case TypeCode.UInt16: result = (Single)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (Single)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = (Single)conv.ToUInt64(null); return true;
                    case TypeCode.Decimal: result = (Single)conv.ToDecimal(null); return true;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 4)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToSingle(bs, 0);
                    return true;
                }
            }
            return StringToSingle(input.ToString(), out result);
        }
        public static bool TryParseInt16(object input, out short result)
        {
            if (input is short)
            {
                result = (short)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToInt16(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (short)1 : (short)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = (short)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = conv.ToInt16(null); return true;
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.SByte: result = (short)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -32768 || a > 32767)
                            {
                                result = 0;
                                return false;
                            }
                            result = (short)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 2)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToInt16(bs, 0);
                    return true;
                }
            }
            return StringToInt16(input.ToString(), out result);
        }
        public static bool TryParseInt32(object input, out int result)
        {
            if (input is int)
            {
                result = (int)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToInt32(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (int)1 : (int)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = conv.ToByte(null); return true;
                    case TypeCode.Char: result = conv.ToChar(null); return true;
                    case TypeCode.Int16: result = conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = conv.ToInt32(null); return true;
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.SByte: result = (int)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (int)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -2147483648 || a > 2147483647)
                            {
                                result = 0;
                                return false;
                            }
                            result = (int)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < -2147483648 || a > 2147483647)
                {
                    result = 0;
                    return false;
                }
                result = (int)a;
                return true;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 2147483647)
                {
                    result = 0;
                    return false;
                }
                result = (int)a;
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 4)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToInt32(bs, 0);
                    return true;
                }
            }
            return StringToInt32(input.ToString(), out result);
        }
        public static bool TryParseInt64(object input, out long result)
        {
            if (input is long)
            {
                result = (long)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToInt64(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (long)1 : (long)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (long)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (long)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (long)conv.ToInt16(null); return true;
                    case TypeCode.Int32: result = (long)conv.ToInt32(null); return true;
                    case TypeCode.Int64: result = (long)conv.ToInt64(null); return true;
                    case TypeCode.SByte: result = (long)conv.ToSByte(null); return true;
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -9223372036854775808L || a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -9223372036854775808L || a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (long)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (long)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -9223372036854775808L || a > 9223372036854775807L)
                            {
                                result = 0;
                                return false;
                            }
                            result = (long)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else if (input is IntPtr)
            {
                result = ((IntPtr)input).ToInt64();
                return true;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 9223372036854775807L)
                {
                    result = 0;
                    return false;
                }
                result = (long)a;
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 8)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToInt64(bs, 0);
                    return true;
                }
            }
            return StringToInt64(input.ToString(), out result);
        }
        public static bool TryParseSByte(object input, out sbyte result)
        {
            if (input is sbyte)
            {
                result = (sbyte)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToSByte(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (sbyte)1 : (sbyte)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte:
                        {
                            var a = conv.ToByte(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Char:
                        {
                            var a = conv.ToChar(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.UInt16:
                        {
                            var a = conv.ToUInt16(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < -128 || a > 127)
                            {
                                result = 0;
                                return false;
                            }
                            result = (sbyte)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            return StringToSByte(input.ToString(), out result);
        }
        public static bool TryParseUInt16(object input, out ushort result)
        {
            if (input is ushort)
            {
                result = (ushort)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToUInt16(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (ushort)1 : (ushort)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (ushort)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (ushort)conv.ToChar(null); return true;
                    case TypeCode.Int16: result = (ushort)conv.ToInt16(null); return true;
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = conv.ToUInt16(null); return true;
                    case TypeCode.UInt32:
                        {
                            var a = conv.ToUInt32(null);
                            if (a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 65535)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ushort)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 2)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToUInt16(bs, 0);
                    return true;
                }
            }
            return StringToUInt16(input.ToString(), out result);
        }
        public static bool TryParseUInt32(object input, out uint result)
        {
            if (input is uint)
            {
                result = (uint)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToUInt32(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (uint)1 : (uint)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (uint)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (uint)conv.ToChar(null); return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (uint)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = conv.ToUInt32(null); return true;
                    case TypeCode.UInt64:
                        {
                            var a = conv.ToUInt64(null);
                            if (a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0 || a > 4294967295)
                            {
                                result = 0;
                                return false;
                            }
                            result = (uint)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < 0 || a > 4294967295)
                {
                    result = 0;
                    return false;
                }
                result = (uint)a;
                return true;
            }
            else if (input is UIntPtr)
            {
                var a = ((UIntPtr)input).ToUInt64();
                if (a > 4294967295)
                {
                    result = 0;
                    return false;
                }
                result = (uint)a;
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 4)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToUInt32(bs, 0);
                    return true;
                }
            }
            return StringToUInt32(input.ToString(), out result);
        }
        public static bool TryParseUInt64(object input, out ulong result)
        {
            if (input is ulong)
            {
                result = (ulong)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToUInt64(str, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        result = conv.ToBoolean(null) ? (ulong)1 : (ulong)0;
                        return true;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                        result = 0;
                        return false;
                    case TypeCode.Byte: result = (ulong)conv.ToByte(null); return true;
                    case TypeCode.Char: result = (ulong)conv.ToChar(null); return true;
                    case TypeCode.Int16:
                        {
                            var a = conv.ToInt16(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    case TypeCode.Int32:
                        {
                            var a = conv.ToInt32(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    case TypeCode.Int64:
                        {
                            var a = conv.ToInt64(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    case TypeCode.SByte:
                        {
                            var a = conv.ToSByte(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    case TypeCode.Double:
                        {
                            var a = conv.ToDouble(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    case TypeCode.Single:
                        {
                            var a = conv.ToSingle(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    case TypeCode.UInt16: result = (ulong)conv.ToUInt16(null); return true;
                    case TypeCode.UInt32: result = (ulong)conv.ToUInt32(null); return true;
                    case TypeCode.UInt64: result = conv.ToUInt64(null); return true;
                    case TypeCode.Decimal:
                        {
                            var a = conv.ToDecimal(null);
                            if (a < 0)
                            {
                                result = 0;
                                return false;
                            }
                            result = (ulong)a;
                            return true;
                        }
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = 0;
                return false;
            }
            else if (input is IntPtr)
            {
                var a = ((IntPtr)input).ToInt64();
                if (a < 0)
                {
                    result = 0;
                    return false;
                }
                result = (ulong)a;
                return true;
            }
            else if (input is UIntPtr)
            {
                result = ((UIntPtr)input).ToUInt64();
                return true;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    if (bs.Length == 0 || bs.Length > 8)
                    {
                        result = 0;
                        return false;
                    }
                    result = BitConverter.ToUInt64(bs, 0);
                    return true;
                }
            }
            return StringToUInt64(input.ToString(), out result);
        }
        public static bool TryParseGuid(object input, out Guid result)
        {
            if (input is Guid)
            {
                result = (Guid)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToGuid(str, out result);
            }
            var bs = input as byte[];
            if (bs != null)
            {
                if (bs.Length == 16)
                {
                    result = new Guid(bs);
                    return true;
                }
                else
                {
                    result = default(Guid);
                    return false;
                }
            }
            else if (input == null)
            {
                result = default(Guid);
                return false;
            }
            return StringToGuid(input.ToString(), out result);
        }
        public static bool TryParseString(object input, out string result)
        {
            var str = input as string;
            if (str != null)
            {
                result = str;
            }
            else if (input == null)
            {
                result = null;
            }
            else
            {
                var bs = input as byte[];
                if (bs != null)
                {
                    result = Encoding.UTF8.GetString(bs);
                }
                else
                {
                    result = input.ToString();
                }
            }
            return true;
        }
        public static bool TryParseBytes(object input, out byte[] result)
        {
            if (input is byte[])
            {
                result = (byte[])input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                if (str.Length == 0)
                {
                    result = new byte[0];
                    return true;
                }
                result = Encoding.UTF8.GetBytes(str);
                return true;
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                        result = null;
                        return false;
                    case TypeCode.Boolean: result = BitConverter.GetBytes(conv.ToByte(null)); return true;
                    case TypeCode.Byte: result = BitConverter.GetBytes(conv.ToByte(null)); return true;
                    case TypeCode.Char: result = BitConverter.GetBytes(conv.ToChar(null)); return true;
                    case TypeCode.Int16: result = BitConverter.GetBytes(conv.ToInt16(null)); return true;
                    case TypeCode.Int32: result = BitConverter.GetBytes(conv.ToInt32(null)); return true;
                    case TypeCode.Int64: result = BitConverter.GetBytes(conv.ToInt64(null)); return true;
                    case TypeCode.SByte:
                        result = null;
                        return false;
                    case TypeCode.Double: result = BitConverter.GetBytes(conv.ToDouble(null)); return true;
                    case TypeCode.Single: result = BitConverter.GetBytes(conv.ToSingle(null)); return true;
                    case TypeCode.UInt16: result = BitConverter.GetBytes(conv.ToUInt16(null)); return true;
                    case TypeCode.UInt32: result = BitConverter.GetBytes(conv.ToUInt32(null)); return true;
                    case TypeCode.UInt64: result = BitConverter.GetBytes(conv.ToUInt64(null)); return true;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = null;
                return true;
            }
            result = null;
            return false;
        }
        public static bool TryParseIntPtr(object input, out IntPtr result)
        {
            if (input is IntPtr)
            {
                result = (IntPtr)input;
                return true;
            }
            long l;
            if (TryParseInt64(input, out l))
            {
                result = new IntPtr(l);
                return true;
            }
            result = default(IntPtr);
            return false;
        }
        public static bool TryParseUIntPtr(object input, out UIntPtr result)
        {
            if (input is UIntPtr)
            {
                result = (UIntPtr)input;
                return true;
            }
            ulong l;
            if (TryParseUInt64(input, out l))
            {
                result = new UIntPtr(l);
                return true;
            }
            result = default(UIntPtr);
            return false;
        }
        public static bool TryParseEnum(object input, Type enumType, out Enum result)
        {
            var em = input as Enum;
            if (em != null)
            {
                result = em;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
                return StringToEnum(str, enumType, out result);
            }
            var conv = input as IConvertible;
            if (conv != null)
            {
                switch (conv.GetTypeCode())
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Boolean:
                        result = null;
                        return false;
                    case TypeCode.Byte: result = (Enum)Enum.ToObject(enumType, conv.ToByte(null)); return true;
                    case TypeCode.Char: result = (Enum)Enum.ToObject(enumType, conv.ToChar(null)); return true;
                    case TypeCode.Int16: result = (Enum)Enum.ToObject(enumType, conv.ToInt16(null)); return true;
                    case TypeCode.Int32: result = (Enum)Enum.ToObject(enumType, conv.ToInt32(null)); return true;
                    case TypeCode.Int64: result = (Enum)Enum.ToObject(enumType, conv.ToInt64(null)); return true;
                    case TypeCode.SByte: result = (Enum)Enum.ToObject(enumType, conv.ToInt64(null)); return true;
                    case TypeCode.Double: result = (Enum)Enum.ToObject(enumType, conv.ToDouble(null)); return true;
                    case TypeCode.Single: result = (Enum)Enum.ToObject(enumType, conv.ToSingle(null)); return true;
                    case TypeCode.UInt16: result = (Enum)Enum.ToObject(enumType, conv.ToUInt16(null)); return true;
                    case TypeCode.UInt32: result = (Enum)Enum.ToObject(enumType, conv.ToUInt32(null)); return true;
                    case TypeCode.UInt64: result = (Enum)Enum.ToObject(enumType, conv.ToUInt64(null)); return true;
                    default:
                        break;
                }
            }
            else if (input == null)
            {
                result = null;
                return false;
            }
            return StringToEnum(input.ToString(), enumType, out result);
        }

#if NF2
        private static bool NF2EnumTryParse<T>(object input, out T result)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), input.ToString(), false);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }
#endif

        public static bool TryParseEnum<T>(object input, out T result)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            if (typeof(T).IsEnum == false)
            {
                throw new ArgumentOutOfRangeException("T", "类型不是枚举");
            }
            if (input is T)
            {
                result = (T)input;
                return true;
            }
            var str = input as string;
            if (str != null)
            {
#if NF2
                return NF2EnumTryParse(str, out result);
#else
                return Enum.TryParse<T>(str, false, out result);
#endif
            }
            if (input == null)
            {
                result = default(T);
                return false;
            }
            switch (Type.GetTypeCode(input.GetType()))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                    result = default(T);
                    return false;
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result = (T)Convert.ChangeType(input, Enum.GetUnderlyingType(typeof(T))); return true;
                default:
                    break;
            }
#if NF2
            return NF2EnumTryParse(input + "", out result);
#else
            return Enum.TryParse<T>(input + "", false, out result);
#endif
        }
        public static bool TryParseObject(object input, Type outputType, out object result)
        {
            return TypesHelper.GetTypeInfo(outputType).TryParse(input, out result);
        }
        public static bool TryParse<T>(object input, out T result)
        {
            return GenericCache<T>.TryParse(input, out result);
        }

        private static class GenericCache<T>
        {
            public readonly static LiteracyTryParse<T> TryParse = InitTryParse();

            private static LiteracyTryParse<T> InitTryParse()
            {
                var ti = TypesHelper.GetTypeInfo<T>();
                var codes = ti.TypeCodes;
                var handler = typeof(LiteracyTryParse<T>);
                var conv = typeof(Convert2);
                switch (codes)
                {
                    case TypeCodes.Empty:
                        return (object input, out T result) => {
                            result = default(T);
                            return (input == null || input is DBNull);
                        };
                    case TypeCodes.DBNull:
                        return (object input, out T result) => {
                            if (input == null || input is DBNull)
                            {
                                result = (T)(object)DBNull.Value;
                                return true;
                            }
                            result = default(T);
                            return false;
                        };
                    case TypeCodes.Boolean:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseBoolean"));
                    case TypeCodes.Char:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseChar"));
                    case TypeCodes.SByte:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseSByte"));
                    case TypeCodes.Byte:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseByte"));
                    case TypeCodes.Int16:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseInt16"));
                    case TypeCodes.UInt16:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseUInt16"));
                    case TypeCodes.Int32:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseInt32"));
                    case TypeCodes.UInt32:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseUInt32"));
                    case TypeCodes.Int64:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseInt64"));
                    case TypeCodes.UInt64:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseUInt64"));
                    case TypeCodes.Single:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseSingle"));
                    case TypeCodes.Double:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseDouble"));
                    case TypeCodes.Decimal:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseDecimal"));
                    case TypeCodes.DateTime:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseDateTime"));
                    case TypeCodes.String:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseString"));
                    case TypeCodes.Guid:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseGuid"));
                    case TypeCodes.TimeSpan:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseTimeSpan"));
                    case TypeCodes.IntPtr:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseIntPtr"));
                    case TypeCodes.UIntPtr:
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, conv.GetMethod("TryParseUIntPtr"));
                    case TypeCodes.Enum:
                        var type = ti.IsNullable ? ti.NullableUnderlyingType.Type : ti.Type;
                        var parse = TryParseEnumMethod.MakeGenericMethod(type);
                        return (LiteracyTryParse<T>)Delegate.CreateDelegate(handler, parse);
                    default:
                        type = ti.IsNullable ? ti.NullableUnderlyingType.Type : ti.Type;
                        return (object input, out T result) => {
                            if (input is T)
                            {
                                result = (T)input;
                                return true;
                            }
                            result = default(T);
                            return false;
                        };
                }
            }
        }

        internal static readonly System.Reflection.MethodInfo TryParseEnumMethod = GetTryParseEnumMethod();

        private static System.Reflection.MethodInfo GetTryParseEnumMethod()
        {
            foreach (var m in typeof(Convert2).GetMethods())
            {
                if (m.Name == "TryParseEnum" && m.IsGenericMethod)
                {
                    return m;
                }
            }
            return null;
        }

        #endregion

        public static LiteracyTryParse CreateDelegate(Type outputType)
        {
            return TypesHelper.GetTypeInfo(outputType).TryParse;
        }

        #region StringToAny
        private static bool StringToBoolean(string input, out bool value)
        {
            if (input == null)
            {
                value = false;
                return false;
            }
            else
            {
                switch (input.Length)
                {
                    case 1:
                        switch (input[0])
                        {
                            case '1':
                            case 'T':
                            case 't':
                            case '对':
                            case '對':
                            case '真':
                            case '是':
                                value = true;
                                return true;
                            case '0':
                            case 'F':
                            case 'f':
                            case '错':
                            case '錯':
                            case '假':
                            case '否':
                                value = false;
                                return true;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        if (input[0] == '-' && input[1] == '1')
                        {
                            value = true;
                            return true;
                        }
                        break;
                    case 4:
                        if (input.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            value = true;
                            return true;
                        }
                        break;
                    case 5:
                        if (input.Equals("false", StringComparison.OrdinalIgnoreCase))
                        {
                            value = true;
                            return true;
                        }
                        break;
                    default:
                        break;
                }
            }
            value = false;
            return false;
        }
        private static bool StringToDateTime(string input, out DateTime value)
        {
            return DateTime.TryParse(input, out  value);
        }
        private static bool StringToTimeSpan(string input, out TimeSpan value)
        {
            return TimeSpan.TryParse(input, out  value);
        }
        private static bool StringToByte(string input, out byte value)
        {
            if (byte.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return byte.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToDecimal(string input, out decimal value)
        {
            if (decimal.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return decimal.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToDouble(string input, out double value)
        {
            if (double.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return double.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToInt16(string input, out short value)
        {
            if (short.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return short.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToInt32(string input, out int value)
        {
            if (int.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return int.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToInt64(string input, out long value)
        {
            if (long.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return long.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToSByte(string input, out sbyte value)
        {
            if (sbyte.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return sbyte.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToSingle(string input, out float value)
        {
            if (float.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return float.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToUInt16(string input, out ushort value)
        {
            if (ushort.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return ushort.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToUInt32(string input, out uint value)
        {
            if (uint.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return uint.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToUInt64(string input, out ulong value)
        {
            if (ulong.TryParse(input, out value))
            {
                return true;
            }
            if (HexString(ref input))
            {
                return ulong.TryParse(input, _hexstyle, _numformat, out value);
            }
            value = 0;
            return false;
        }
        private static bool StringToGuid(string input, out Guid value)
        {
            if (input == null || input.Length == 0)
            {
                value = Guid.Empty;
                return false;
            }

#if !NF2
            if (Guid.TryParse(input, out value))
            {
                return true;
            }
#else
            var l = input.Length;
            var a = input[0];
            var b = input[l - 1];
            if (a == ' ' || b == ' ')
            {
                input = input.Trim();
                a = input[0];
                b = input[l - 1];
            }

            if ((a == '{' && b == '}') || l == 32 || l == 36)
            {
                try
                {
                    value = new Guid(input);
                    return true;
                }
                catch { }
            }
#endif
            else
            {
                try
                {
                    var bs = Convert.FromBase64String(input);
                    if (bs.Length != 16)
                    {
                        value = Guid.Empty;
                        return false;
                    }
                    value = new Guid(bs);
                    return true;
                }
                catch { }
            }
            value = Guid.Empty;
            return false;
        }
        private static bool StringToEnum(string input, Type enumType, out Enum value)
        {
            try
            {
                value = (Enum)Enum.Parse(enumType, input, false);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
        private static bool HexString(ref string s)
        {
            if (s == null)
            {
                return false;
            }
            var c = s[0];
            if (char.IsWhiteSpace(c)) //有空格去空格
            {
                s = s.TrimStart();
            }
            if (s.Length > 2) //判断是否是0x 或者 &h 开头
            {
                switch (c)
                {
                    case '0':
                        switch (s[1])
                        {
                            case 'x':
                            case 'X':
                                s = s.Remove(0, 2);
                                return true;
                            default:
                                return true;
                        }
                    case '&':
                        switch (s[1])
                        {
                            case 'h':
                            case 'H':
                                s = s.Remove(0, 2);
                                return true;
                            default:
                                return false;
                        }
                    default:
                        return false;
                }
            }
            return false;
        }
        private static bool TryParseHex(string s, out decimal value, decimal max)
        {
            if (s == null)
            {
                value = 0;
                return false;
            }
            if (char.IsWhiteSpace(s, 0))
            {
                s = s.Trim();
            }
            if (s.Length > 2)
            {
                switch (s[0])
                {
                    case '0':
                        switch (s[1])
                        {
                            case 'x':
                            case 'X':
                                s = s.Remove(0, 2);
                                break;
                        }
                        break;
                    case '&':
                        switch (s[1])
                        {
                            case 'h':
                            case 'H':
                                s = s.Remove(0, 2);
                                break;
                            default:
                                value = 0;
                                return false;
                        }
                        break;
                }
            }
            decimal i;
            if (decimal.TryParse(s, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out i))
            {
                if (i <= max)
                {
                    value = i;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        private static readonly NumberStyles _hexstyle = NumberStyles.HexNumber;
        private static readonly NumberFormatInfo _numformat = NumberFormatInfo.InvariantInfo;
        #endregion

        #region ToEntity

        public static T ToEntity<T>(DbDataReader reader)
        {
            AreNull(reader, "reader");
            T model = default(T);
            FillEntity(reader, ref model);
            return model;
        }

        public static bool FillEntity<T>(DbDataReader reader, ref T model)
        {
            AreNull(reader, "reader");
            if (reader.Read() == false)
            {
                return false;
            }

            var ti = TypesHelper.GetTypeInfo<T>();
            if (ti.IsSpecialType)
            {
                model = ToObject<T>(reader[0], throwOnError: true);
            }
            else
            {
                var lit = ti.IgnoreCaseLiteracy;
                if (model == null)
                {
                    model = (T)ti.IgnoreCaseLiteracy.NewObject();
                }
                var props = GetProperties(reader, lit);
                FillEntity<T>(reader, props, model);
            }
            return true;
        }

        public static List<T> ToList<T>(DbDataReader reader)
        {
            AreNull(reader, "reader");
            var ti = TypesHelper.GetTypeInfo<T>();
            var lit = ti.IgnoreCaseLiteracy;
            var props = GetProperties(reader, lit);
            var list = new List<T>();
            if (ti.IsSpecialType)
            {
                while (reader.Read())
                {
                    list.Add(ToObject<T>(reader[0], throwOnError: true));
                }
            }
            else
            {
                while (reader.Read())
                {
                    var model = (T)lit.NewObject();
                    FillEntity<T>(reader, props, model);
                    list.Add(model);
                }
            }

            return list;
        }

        private static void AreNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static T ToEntity<T>(DataRow row)
        {
            AreNull(row, "row");
            var model = default(T);
            FillEntity(row, ref model);
            return model;
        }

        public static bool FillEntity<T>(DataRow row, ref T model)
        {
            AreNull(row, "row");
            if (row.HasErrors)
            {
                return false;
            }
            var ti = TypesHelper.GetTypeInfo(typeof(T));
            if (ti.IsSpecialType)
            {
                model = ToObject<T>(row[0], throwOnError: true);
            }
            else
            {
                var lit = ti.IgnoreCaseLiteracy;
                if (model == null)
                {
                    model = (T)lit.NewObject();
                }
                var props = GetProperties(row.Table, lit);
                FillEntity<T>(row, props, model);
            }
            return true;
        }

        public static List<T> ToList<T>(DataTable table)
        {
            AreNull(table, "table");
            var ti = TypesHelper.GetTypeInfo(typeof(T));
            var lit = ti.IgnoreCaseLiteracy;
            var props = GetProperties(table, lit);
            var list = new List<T>();
            if (ti.IsSpecialType)
            {
                foreach (DataRow row in table.Rows)
                {
                    list.Add(ToObject<T>(row[0], throwOnError: true));
                }
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    var model = (T)lit.NewObject();
                    FillEntity<T>(row, props, model);
                    list.Add(model);
                }
            }
            return list;
        }

        #region 私有方法
        private static ObjectProperty[] GetProperties(DataTable table, Literacy lit)
        {
            var cols = table.Columns;
            var length = cols.Count;
            var props = new ObjectProperty[length];
            for (int i = 0; i < length; i++)
            {
                var name = cols[i].ColumnName;
                var p = lit.Property[name.Replace("_", "")] ?? lit.Property[name];
                if (p != null && p.CanWrite)
                {
                    props[i] = p;
                }
                props[i] = p;
            }
            return props;
        }
        private static ObjectProperty[] GetProperties(DbDataReader reader, Literacy lit)
        {
            var length = reader.FieldCount;
            var props = new ObjectProperty[length];
            for (int i = 0; i < length; i++)
            {
                var name = reader.GetName(i);
                var p = lit.Property[name.Replace("_", "")] ?? lit.Property[name];
                if (p != null && p.CanWrite)
                {
                    props[i] = p;
                }
            }
            return props;
        }
        private static void FillEntity<T>(DbDataReader reader, ObjectProperty[] props, T model)
        {
            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];
                if (p != null)
                {
                    p.SetValue(model, p.TypeInfo.Convert(reader[i]));
                }
            }
        }
        private static void FillEntity<T>(DataRow row, ObjectProperty[] props, T model)
        {
            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];
                if (p != null)
                {
                    p.SetValue(model, p.TypeInfo.Convert(row[i]));
                }
            }
        }
        #endregion

        #endregion

    }
}

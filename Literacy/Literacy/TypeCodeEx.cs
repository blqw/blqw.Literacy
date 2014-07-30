using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary> 指定对象的类型。兼容System.TypeCode,并提供更多
    /// </summary>
    [Serializable]
    public enum TypeCodeEx
    {
        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18,

        Guid = 10019,
        TimeSpan = 10020,
        IList = 10021,
        IDictionary = 10022,
        StringBuilder = 10023,
        Enum = 10024,
    }
}

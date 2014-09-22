using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary> 指定对象的类型。
    /// </summary>
    [Serializable]
    public enum TypeCodes
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

        Guid = 51,
        TimeSpan = 52,
        IntPtr = 53,
        UIntPtr = 54,

        IList = 221,
        IDictionary = 222,
        StringBuilder = 223,
        IListT = 225,
        IDictionaryT = 226,
        //[Obsolete("已弃用",true)]
        //IEnumerable = 227,
        DbDataReader = 228,
        DataSet = 229,
        DataTable = 230,
        DataView = 231,
        AnonymousType = 232,
        Enum = 233,
        Type = 234,
        DbParameter = 245,
        Xml = 246,
    }
}

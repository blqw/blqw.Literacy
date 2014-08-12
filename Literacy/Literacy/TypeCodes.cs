using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary> 指定对象的类型。兼容System.TypeCode,并提供更多
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

        Guid = 99,
        TimeSpan = 98,

        IList = 10021,
        IDictionary = 10022,
        StringBuilder = 10023,
        IListT = 10025,
        IDictionaryT = 10026,
        IEnumerable = 10027,
        IDataReader = 10028,
        DataSet = 10029,
        DataTable = 10030,
        DataView = 10031,
        AnonymousType = 1032,
    }
}

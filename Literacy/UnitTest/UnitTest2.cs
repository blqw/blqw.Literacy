using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blqw;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestTypesHelper()
        {
            Assert.IsTrue(TypesHelper.IsNumber(1));
            Assert.IsTrue(TypesHelper.IsNumber(1.1));
            Assert.IsFalse(TypesHelper.IsNumber(Guid.Empty));
            int? i = 1;
            Assert.IsTrue(TypesHelper.IsNumber(i));
            i = null;
            Assert.IsFalse(TypesHelper.IsNumber(i));

            Assert.IsTrue(TypesHelper.IsSpecialType(typeof(DateTime)));
            Assert.IsFalse(TypesHelper.IsSpecialType(typeof(UnitTest2)));

            Assert.IsTrue(TypesHelper.IsNullable(typeof(DateTime?)));
            Assert.IsFalse(TypesHelper.IsNullable(typeof(DateTime)));

            Assert.IsTrue(TypesHelper.IsNumberType(typeof(int)));
            Assert.IsTrue(TypesHelper.IsNumberType(typeof(TypeCode)));
            Assert.IsFalse(TypesHelper.IsNumberType(typeof(DateTime)));

            Assert.AreEqual("Int32?", TypesHelper.DisplayName(typeof(Int32?)));
            Assert.AreEqual("IDictionary<List<Dictionary<String, Int32?>>, List<DateTime>>", TypesHelper.DisplayName(typeof(IDictionary<List<Dictionary<String, Int32?>>, List<DateTime>>)));
        }

        [TestMethod]
        public void TestTypeInfo1()
        {
            var int_info = TypesHelper.GetTypeInfo(typeof(int?));
            var string_info = TypesHelper.GetTypeInfo<string>();
            var list_info = TypesHelper.GetTypeInfo<List<UnitTest2>>();
            var dict_info = TypesHelper.GetTypeInfo<Dictionary<Guid, decimal>>();
            var ilist_info = TypesHelper.GetTypeInfo(typeof(IList<>));
            var arr_info = TypesHelper.GetTypeInfo<DateTime[]>();


            Assert.IsTrue(int_info.IsSpecialType);
            Assert.IsFalse(int_info.IsArray);
            Assert.IsTrue(int_info.IsMakeGenericType);
            Assert.IsTrue(int_info.IsNullable);
            Assert.IsTrue(int_info.IsNumberType);
            Assert.AreEqual(typeof(int), int_info.NullableUnderlyingType.Type);

            Assert.IsTrue(string_info.IsSpecialType);
            Assert.IsFalse(string_info.IsArray);
            Assert.IsFalse(string_info.IsMakeGenericType);
            Assert.IsFalse(string_info.IsNullable);
            Assert.IsFalse(string_info.IsNumberType);
            Assert.IsNull(string_info.NullableUnderlyingType);

            Assert.IsFalse(list_info.IsSpecialType);
            Assert.IsFalse(list_info.IsArray);
            Assert.IsTrue(list_info.IsMakeGenericType);
            Assert.IsFalse(list_info.IsNullable);
            Assert.IsFalse(list_info.IsNumberType);

            Assert.AreEqual(1, list_info.GenericArgumentsTypeInfo.Length);
            Assert.IsFalse(list_info.GenericArgumentsTypeInfo[0].IsSpecialType);
            Assert.IsFalse(list_info.GenericArgumentsTypeInfo[0].IsArray);
            Assert.IsFalse(list_info.GenericArgumentsTypeInfo[0].IsMakeGenericType);
            Assert.IsFalse(list_info.GenericArgumentsTypeInfo[0].IsNullable);
            Assert.IsFalse(list_info.GenericArgumentsTypeInfo[0].IsNumberType);

            Assert.IsFalse(dict_info.IsSpecialType);
            Assert.IsFalse(dict_info.IsArray);
            Assert.IsTrue(dict_info.IsMakeGenericType);
            Assert.IsFalse(dict_info.IsNullable);
            Assert.IsFalse(dict_info.IsNumberType);

            Assert.AreEqual(2, dict_info.GenericArgumentsTypeInfo.Length);
            Assert.IsTrue(dict_info.GenericArgumentsTypeInfo[0].IsSpecialType);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[0].IsArray);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[0].IsMakeGenericType);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[0].IsNullable);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[0].IsNumberType);

            Assert.IsTrue(dict_info.GenericArgumentsTypeInfo[1].IsSpecialType);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[1].IsArray);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[1].IsMakeGenericType);
            Assert.IsFalse(dict_info.GenericArgumentsTypeInfo[1].IsNullable);
            Assert.IsTrue(dict_info.GenericArgumentsTypeInfo[1].IsNumberType);

            Assert.IsFalse(ilist_info.IsSpecialType);
            Assert.IsFalse(ilist_info.IsArray);
            Assert.IsFalse(ilist_info.IsMakeGenericType);
            Assert.IsFalse(ilist_info.IsNullable);
            Assert.IsFalse(ilist_info.IsNumberType);
            Assert.IsNull(ilist_info.GenericArgumentsTypeInfo);

            Assert.IsFalse(arr_info.IsSpecialType);
            Assert.IsTrue(arr_info.IsArray);
            Assert.IsFalse(arr_info.IsMakeGenericType);
            Assert.IsFalse(arr_info.IsNullable);
            Assert.IsFalse(arr_info.IsNumberType);
        }

        [TestMethod]
        public void TestTypeInfo2()
        {
            var int_info    = TypesHelper.GetTypeInfo(typeof(int?));
            var string_info = TypesHelper.GetTypeInfo<string>();
            var list_info   = TypesHelper.GetTypeInfo<List<UnitTest2>>();
            var dict_info   = TypesHelper.GetTypeInfo<Dictionary<Guid, decimal>>();
            var ilist_info  = TypesHelper.GetTypeInfo(typeof(IList<>));
            var arr_info    = TypesHelper.GetTypeInfo<DateTime[]>();

            Assert.AreEqual("Int32?", int_info.DisplayName);
            Assert.AreEqual("String", string_info.DisplayName);
            Assert.AreEqual("List<UnitTest.UnitTest2>", list_info.DisplayName);
            Assert.AreEqual("Dictionary<Guid, Decimal>", dict_info.DisplayName);
            Assert.AreEqual("IList<>", ilist_info.DisplayName);
            Assert.AreEqual("DateTime[]", arr_info.DisplayName);

            Assert.AreEqual(TypeCodes.Int32, int_info.TypeCodes);
            Assert.AreEqual(TypeCodes.String, string_info.TypeCodes);
            Assert.AreEqual(TypeCodes.IListT, list_info.TypeCodes);
            Assert.AreEqual(TypeCodes.IDictionaryT, dict_info.TypeCodes);
            Assert.AreEqual(TypeCodes.Object, ilist_info.TypeCodes);
            Assert.AreEqual(TypeCodes.IList, arr_info.TypeCodes);

        }
    }
}

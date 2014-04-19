using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest
{
    public struct StructEntity
    {
        public StructEntity(string str, int i)
            :this()
        {
            ClassProperty = str;
            StructProperty = i;
        }

        public StructEntity( string privateClassProperty,
                            int privateStructProperty,
                            string privateStaticClassProperty,
                            int privateStaticStructProperty,
                            string privateClassField,
                            int privateStructField,
                            string privateStaticClassField,
                            int privateStaticStructField)
            :this()
        {
            PrivateClassProperty = privateClassProperty;
            PrivateStructProperty = privateStructProperty;
            PrivateStaticClassProperty = privateStaticClassProperty;
            PrivateStaticStructProperty = privateStaticStructProperty;
            PrivateClassField = privateClassField;
            PrivateStructField = privateStructField;
            PrivateStaticClassField = privateStaticClassField;
            PrivateStaticStructField = privateStaticStructField;
        }

        //公共实例属性
        public string ClassProperty { get; set; }
        public int StructProperty { get; set; }

        //公共静态属性
        public static string StaticClassProperty { get; set; }
        public static int StaticStructProperty { get; set; }

        //私有实例属性
        string PrivateClassProperty { get; set; }
        int PrivateStructProperty { get; set; }

        //私有静态属性
        static string PrivateStaticClassProperty { get; set; }
        static int PrivateStaticStructProperty { get; set; }

        //公共实例字段
        public string ClassField;
        public int StructField;

        //公共静态字段
        public static string StaticClassField;
        public static int StaticStructField;

        //私有实例字段
        string PrivateClassField;
        int PrivateStructField;

        //私有静态字段
        static string PrivateStaticClassField;
        static int PrivateStaticStructField;


        //公共实例方法
        public object[] Method(string str, int i)
        {
            ClassProperty = str;
            StructProperty = i;
            return new object[] { str, i };
        }

        public int MethodRef(ref string str, ref int i)
        {
            str += str;
            i += i;
            ClassProperty = str;
            StructProperty = i;
            return i;
        }

        public string MethodOut(out string str, out int i)
        {
            str = "A";
            i = 1;
            ClassProperty = str;
            StructProperty = i;
            return str;
        }

        //私有实例方法
        object[] PrivateMethod(string str, int i)
        {
            ClassProperty = str;
            StructProperty = i;
            return new object[] { str, i };
        }

        int PrivateMethodRef(ref string str, ref int i)
        {
            str += str;
            i += i;
            ClassProperty = str;
            StructProperty = i;
            return i;
        }

        string PrivateMethodOut(out string str, out int i)
        {
            str = "A";
            i = 1;
            ClassProperty = str;
            StructProperty = i;
            return str;
        }

        //公共静态方法
        public static object[] StaticMethod(string str, int i)
        {
            return new object[] { str, i };
        }

        public static int StaticMethodRef(ref string str, ref int i)
        {
            str += str;
            i += i;
            return i;
        }

        public static string StaticMethodOut(out string str, out int i)
        {
            str = "A";
            i = 1;
            return str;
        }

        //私有静态方法
        static object[] PrivateStaticMethod(string str, int i)
        {
            return new object[] { str, i };
        }

        static int PrivateStaticMethodRef(ref string str, ref int i)
        {
            str += str;
            i += i;
            return i;
        }

        static string PrivateStaticMethodOut(out string str, out int i)
        {
            str = "A";
            i = 1;
            return str;
        }
    }
}

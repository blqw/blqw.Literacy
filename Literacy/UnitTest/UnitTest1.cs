using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        //测试引用类型的属性取值/赋值
        [TestMethod]
        public void TestClassProperty()
        {
            ClassEntity obj = new ClassEntity();

            blqw.Literacy lit = new blqw.Literacy(obj.GetType());

            object value;
            //公共实例属性
            lit.Property["ClassProperty"].SetValue(obj, "A");
            value = lit.Property["ClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "A");

            lit.Property["StructProperty"].SetValue(obj, 1);
            value = lit.Property["StructProperty"].GetValue(obj);
            Assert.AreEqual(value, 1);

            //公共静态属性
            lit.Load.StaticProperty(false);
            lit.Property["StaticClassProperty"].SetValue(obj, "B");
            value = lit.Property["StaticClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "B");

            lit.Property["StaticStructProperty"].SetValue(obj, 2);
            value = lit.Property["StaticStructProperty"].GetValue(obj);
            Assert.AreEqual(value, 2);

            //私有实例属性
            lit.Load.NonPublicProperty();

            lit.Property["PrivateClassProperty"].SetValue(obj, "C");
            value = lit.Property["PrivateClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "C");

            lit.Property["PrivateStructProperty"].SetValue(obj, 3);
            value = lit.Property["PrivateStructProperty"].GetValue(obj);
            Assert.AreEqual(value, 3);

            //私有静态属性
            lit.Load.StaticProperty(true);

            lit.Property["PrivateStaticClassProperty"].SetValue(obj, "D");
            value = lit.Property["PrivateStaticClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "D");

            lit.Property["PrivateStaticStructProperty"].SetValue(obj, 4);
            value = lit.Property["PrivateStaticStructProperty"].GetValue(obj);
            Assert.AreEqual(value, 4);
        }
        //测试值类型的属性取值
        [TestMethod]
        public void TestStructProperty()
        {
            //值类型不支持成员赋值操作
            StructEntity obj = new StructEntity("C", 3, "D", 4, null, 0, null, 0);
            obj.ClassProperty = "A";
            obj.StructProperty = 1;
            StructEntity.StaticClassProperty = "B";
            StructEntity.StaticStructProperty = 2;

            blqw.Literacy lit = new blqw.Literacy(obj.GetType());

            object value;
            //公共实例属性
            value = lit.Property["ClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "A");

            value = lit.Property["StructProperty"].GetValue(obj);
            Assert.AreEqual(value, 1);

            //公共静态属性
            lit.Load.StaticProperty(false);
            value = lit.Property["StaticClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "B");

            value = lit.Property["StaticStructProperty"].GetValue(obj);
            Assert.AreEqual(value, 2);

            //私有实例属性
            lit.Load.NonPublicProperty();

            value = lit.Property["PrivateClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "C");

            value = lit.Property["PrivateStructProperty"].GetValue(obj);
            Assert.AreEqual(value, 3);

            //私有静态属性
            lit.Load.StaticProperty(true);

            value = lit.Property["PrivateStaticClassProperty"].GetValue(obj);
            Assert.AreEqual(value, "D");

            value = lit.Property["PrivateStaticStructProperty"].GetValue(obj);
            Assert.AreEqual(value, 4);
        }
        //测试引用类型的字段取值/赋值
        [TestMethod]
        public void TestClassField()
        {
            ClassEntity obj = new ClassEntity();

            blqw.Literacy lit = new blqw.Literacy(obj.GetType());

            object value;
            //公共实例字段
            lit.Load.PublicField();
            lit.Field["ClassField"].SetValue(obj, "A");
            value = lit.Field["ClassField"].GetValue(obj);
            Assert.AreEqual(value, "A");

            lit.Field["StructField"].SetValue(obj, 1);
            value = lit.Field["StructField"].GetValue(obj);
            Assert.AreEqual(value, 1);

            //公共静态字段
            lit.Load.StaticField(false);
            lit.Field["StaticClassField"].SetValue(obj, "B");
            value = lit.Field["StaticClassField"].GetValue(obj);
            Assert.AreEqual(value, "B");

            lit.Field["StaticStructField"].SetValue(obj, 2);
            value = lit.Field["StaticStructField"].GetValue(obj);
            Assert.AreEqual(value, 2);

            //私有实例字段
            lit.Load.NonPublicField();

            lit.Field["PrivateClassField"].SetValue(obj, "C");
            value = lit.Field["PrivateClassField"].GetValue(obj);
            Assert.AreEqual(value, "C");

            lit.Field["PrivateStructField"].SetValue(obj, 3);
            value = lit.Field["PrivateStructField"].GetValue(obj);
            Assert.AreEqual(value, 3);

            //私有静态字段
            lit.Load.StaticField(true);

            lit.Field["PrivateStaticClassField"].SetValue(obj, "D");
            value = lit.Field["PrivateStaticClassField"].GetValue(obj);
            Assert.AreEqual(value, "D");

            lit.Field["PrivateStaticStructField"].SetValue(obj, 4);
            value = lit.Field["PrivateStaticStructField"].GetValue(obj);
            Assert.AreEqual(value, 4);
        }
        //测试值类型的字段取值
        [TestMethod]
        public void TestStructField()
        {
            //值类型不支持成员赋值操作
            StructEntity obj = new StructEntity(null, 0, null, 0, "C", 3, "D", 4);
            obj.ClassField = "A";
            obj.StructField = 1;
            StructEntity.StaticClassField = "B";
            StructEntity.StaticStructField = 2;

            blqw.Literacy lit = new blqw.Literacy(obj.GetType());

            object value;
            lit.Load.PublicField();
            //公共实例字段
            value = lit.Field["ClassField"].GetValue(obj);
            Assert.AreEqual(value, "A");

            value = lit.Field["StructField"].GetValue(obj);
            Assert.AreEqual(value, 1);

            //公共静态字段
            lit.Load.StaticField(false);
            value = lit.Field["StaticClassField"].GetValue(obj);
            Assert.AreEqual(value, "B");

            value = lit.Field["StaticStructField"].GetValue(obj);
            Assert.AreEqual(value, 2);

            //私有实例字段
            lit.Load.NonPublicField();

            value = lit.Field["PrivateClassField"].GetValue(obj);
            Assert.AreEqual(value, "C");

            value = lit.Field["PrivateStructField"].GetValue(obj);
            Assert.AreEqual(value, 3);

            //私有静态字段
            lit.Load.StaticField(true);

            value = lit.Field["PrivateStaticClassField"].GetValue(obj);
            Assert.AreEqual(value, "D");

            value = lit.Field["PrivateStaticStructField"].GetValue(obj);
            Assert.AreEqual(value, 4);
        }
        //测试引用类型方法调用,参数包含值类型,引用类型, ref 和out,
        [TestMethod]
        public void TestClassMethod()
        {
            ClassEntity obj = new ClassEntity();
            var methods = obj.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);

            foreach (var method in methods)
            {
                string str = "Z";
                int i = 26;
                object[] args = new object[] { str, i };

                var caller = blqw.Literacy.CreateCaller(method);
                if (method.Name.EndsWith("Ref"))
                {
                    var val = caller(obj, args);
                    obj.MethodRef(ref str, ref i);
                    Assert.AreEqual(val, i);
                }
                else if (method.Name.EndsWith("Out"))
                {
                    var val = caller(obj, args);
                    obj.MethodOut(out str, out i);
                    Assert.AreEqual(val, str);
                }
                else if (method.Name.EndsWith("Method"))
                {
                    args = obj.Method(str, i);
                    str = (string)args[0];
                    i = (int)args[1];
                    args = (object[])caller(obj, str, i);
                }
                else
                {
                    continue;
                }

                Assert.AreEqual(args[0], str);
                Assert.AreEqual(args[1], i);
                if (method.Name.Contains("Static") == false)
                {
                    Assert.AreEqual(obj.ClassProperty, str);
                    Assert.AreEqual(obj.StructProperty, i);
                }
            }
        }

        [TestMethod]
        public void TestStructMethod()
        {
            StructEntity obj = new StructEntity();
            var methods = obj.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);

            foreach (var method in methods)
            {
                string str = "Z";
                int i = 26;
                object[] args = new object[] { str, i };

                var caller = blqw.Literacy.CreateCaller(method);
                if (method.Name.EndsWith("Ref"))
                {
                    var val = caller(obj, args);
                    obj.MethodRef(ref str, ref i);
                    Assert.AreEqual(val, i);
                }
                else if (method.Name.EndsWith("Out"))
                {
                    var val = caller(obj, args);
                    obj.MethodOut(out str, out i);
                    Assert.AreEqual(val, str);
                }
                else if (method.Name.EndsWith("Method"))
                {
                    args = obj.Method(str, i);
                    str = (string)args[0];
                    i = (int)args[1];
                    args = (object[])caller(obj, str, i);
                }
                else
                {
                    continue;
                }

                Assert.AreEqual(args[0], str);
                Assert.AreEqual(args[1], i);
                if (method.Name.Contains("Static") == false)
                {
                    Assert.AreEqual(obj.ClassProperty, str);
                    Assert.AreEqual(obj.StructProperty, i);
                }
            }
        }




        [TestMethod]
        public void TestCreateClass()
        {
            var new1 = blqw.Literacy.CreateNewObject(typeof(ClassEntity), null);
            var obj1 = (ClassEntity)new1();
            Assert.AreNotEqual(null, obj1);

            var new2 = blqw.Literacy.CreateNewObject(typeof(ClassEntity), new Type[] { typeof(string), typeof(int) });
            var obj2 = (ClassEntity)new2("A", 1);
            Assert.AreEqual("A", obj2.ClassProperty);
            Assert.AreEqual(1, obj2.StructProperty);
        }

        [TestMethod]
        public void TestCreateStruct()
        {
            var new1 = blqw.Literacy.CreateNewObject(typeof(StructEntity), null);
            var obj1 = (StructEntity)new1();
            Assert.AreNotEqual(null, obj1);

            var new2 = blqw.Literacy.CreateNewObject(typeof(StructEntity), new Type[] { typeof(string), typeof(int) });
            var obj2 = (StructEntity)new2("A", 1);
            Assert.AreEqual("A", obj2.ClassProperty);
            Assert.AreEqual(1, obj2.StructProperty);
        }
    }
}

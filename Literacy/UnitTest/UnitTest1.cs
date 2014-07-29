using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
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
        //测试引用类型方法调用
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
        //测试值类型的方法调用
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
        //测试初始化引用类型
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
        //测试初始化值类型
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

        //测试类型特性
        [TestMethod]
        public void TestTypeAttr()
        {
            var lit1 = blqw.Literacy.Cache(typeof(ClassEntity), false);
            Assert.AreEqual(3, lit1.Attributes.Count);
            Assert.IsTrue(lit1.Attributes.Exists(typeof(TestAttribute)));
            Assert.IsTrue(lit1.Attributes.Exists<TestAttribute>());
            Assert.IsTrue(lit1.Attributes.Exists<TestAttribute>(it => it.ID == 0));
            Assert.IsFalse(lit1.Attributes.Exists(typeof(TestMethodAttribute)));
            Assert.IsFalse(lit1.Attributes.Exists<TestMethodAttribute>());
            Assert.IsFalse(lit1.Attributes.Exists<TestMethodAttribute>(it => true));
            Assert.IsFalse(lit1.Attributes.Exists<TestMethodAttribute>(it => false));

            Assert.AreEqual(3, lit1.Attributes.Where(typeof(TestAttribute)).Count());
            Assert.AreEqual(3, lit1.Attributes.Where<TestAttribute>().Count());
            Assert.AreEqual(2, lit1.Attributes.Where<TestAttribute>(it => it.ID > 0).Count());
            Assert.AreEqual(0, lit1.Attributes.Where(typeof(TestMethodAttribute)).Count());
            Assert.AreEqual(0, lit1.Attributes.Where<TestMethodAttribute>().Count());
            Assert.AreEqual(0, lit1.Attributes.Where<TestMethodAttribute>(it => true).Count());
            Assert.AreEqual(0, lit1.Attributes.Where<TestMethodAttribute>(it => false).Count());


            var lit2 = blqw.Literacy.Cache(typeof(StructEntity), false);
            Assert.AreEqual(2, lit2.Attributes.Count);
            Assert.IsNotNull(lit2.Attributes.First(typeof(TestAttribute)));
            Assert.IsNotNull(lit2.Attributes.First<TestAttribute>());
            Assert.IsNotNull(lit2.Attributes.First<TestAttribute>(it => it.ID == 0));

            Assert.IsNull(lit2.Attributes.First(typeof(TestMethodAttribute)));
            Assert.IsNull(lit2.Attributes.First<TestMethodAttribute>());
            Assert.IsNull(lit2.Attributes.First<TestMethodAttribute>(it => true));
            Assert.IsNull(lit2.Attributes.First<TestMethodAttribute>(it => false));

        }
        //测试属性特性
        [TestMethod]
        public void TestPropertyAttr()
        {
            var lit1 = blqw.Literacy.Cache(typeof(ClassEntity), false);
            var lit2 = blqw.Literacy.Cache(typeof(StructEntity), false);
            lit1.Load.NonPublicProperty();
            lit1.Load.StaticProperty(true);
            lit2.Load.NonPublicProperty();
            lit2.Load.StaticProperty(true);

            Assert.AreEqual(2, lit1.Property["ClassProperty"].Attributes.Count);
            Assert.AreEqual(1, lit1.Property["StructProperty"].Attributes.Count);
            Assert.AreEqual(1, lit1.Property["StaticClassProperty"].Attributes.Count);
            Assert.AreEqual(0, lit1.Property["StaticStructProperty"].Attributes.Count);
            Assert.AreEqual(6, lit1.Property["StaticClassProperty"].Attributes.First<TestAttribute>().ID);


            Assert.AreEqual(1, lit2.Property["ClassProperty"].Attributes.Count);
            Assert.AreEqual(1, lit2.Property["StructProperty"].Attributes.Count);
            Assert.AreEqual(2, lit2.Property["StaticClassProperty"].Attributes.Count);
            Assert.AreEqual(0, lit2.Property["StaticStructProperty"].Attributes.Count);
            Assert.AreEqual(12, lit2.Property["ClassProperty"].Attributes.First<TestAttribute>().ID);
        }


    }
}

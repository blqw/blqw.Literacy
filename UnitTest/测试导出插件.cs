using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blqw.ReflectionComponent;
using System.Reflection;
using System.ComponentModel.Composition;

namespace UnitTest
{
    [TestClass]
    public class UnitTest2
    {
        class MyClass
        {
            public MyClass()
            {
                MEFPart.Import(this);
            }

            [Import("MemberInfoWrapper")]
            public Func<MemberInfo, MemberInfo> Warp;

            [Import("CreateGetter")]
            public Func<MemberInfo, Func<object, object>> GetGeter;

            [Import("CreateSetter")]
            public Func<MemberInfo, Action<object, object>> GetSeter;

            [Import("CreateCaller")]
            public Func<MethodInfo, Func<object, object[], object>> GetCaller;
        }

        class MyClass2
        {
            public int ID { get; set; }
            public string Name;
            public int Call(string s)
            {
                return int.Parse(s);
            }

            public static int Call2(string s)
            {
                return int.Parse(s) + 1;
            }
        }

        [TestMethod]
        public void 测试导出插件()
        {
            var c = new MyClass();
            Assert.IsNotNull(c.Warp);
            Assert.IsNotNull(c.GetGeter);
            Assert.IsNotNull(c.GetSeter);
            Assert.IsNotNull(c.GetCaller);

            var d = new MyClass2
            {
                ID = 1,
                Name = "blqw",
            };

            var t = c.Warp(d.GetType());
            Assert.IsNotNull(t);
            Assert.AreEqual("TypeEx", t.GetType().Name);

            var set = c.GetSeter(d.GetType().GetProperty("ID"));
            Assert.IsNotNull(set);
            set(d, 2);
            Assert.AreEqual(2, d.ID);

            var get = c.GetGeter(d.GetType().GetProperty("ID"));
            Assert.IsNotNull(get);
            Assert.AreEqual(2, get(d));

            var set2 = c.GetSeter(d.GetType().GetField("Name"));
            Assert.IsNotNull(set2);
            set2(d, "blqw2");
            Assert.AreEqual("blqw2", d.Name);

            var get2 = c.GetGeter(d.GetType().GetField("Name"));
            Assert.IsNotNull(get2);
            Assert.AreEqual("blqw2", get2(d));

            var cell = c.GetCaller(d.GetType().GetMethod("Call"));
            Assert.IsNotNull(cell);
            Assert.AreEqual(5, cell(d, new object[] { "5" }));


            var cell2 = c.GetCaller(d.GetType().GetMethod("Call2"));
            Assert.IsNotNull(cell);
            Assert.AreEqual(11, cell2(null, new object[] { "10" }));

        }
    }
}

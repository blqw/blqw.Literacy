using blqw;
using blqw.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            User u = new User();
            CodeTimer.Initialize();
            CodeTimer.Time("MethodInfo", 1000000, () => GetName2(u));
            CodeTimer.Time("dynamic", 1000000, () => GetName3(u));
            CodeTimer.Time("Literacy", 1000000, () => GetName(u));



            var tester = new TesterBase[]
            {
                new LinqTester(),
                new CreateDelegateTester(),
                new LiteracyTester(),
            };

            foreach (var t in tester)
            {
                t.TestCount = 1000000;
                t.State = new User() { Name = "blqw1" };
                t.Start();
            }

        }

        static ObjectProperty prop;

        public static object GetName(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (prop == null)
            {
                prop = new Literacy(obj.GetType()).Property["Name"];
                if (prop == null) throw new NotSupportedException("对象不包含Name属性");
            }
            return prop.GetValue(obj);
        }

        static PropertyInfo pName;

        public static object GetName2(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (pName == null)
            {
                pName = typeof(User).GetProperty("Name");
            }
            return pName.GetValue(obj, null); //缓存了反射Name属性
        }

        public static object GetName3(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return ((dynamic)obj).Name;
        }


    }
}

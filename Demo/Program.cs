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
            CodeTimer.Time("Literacy", 1000000, () => GetName(u));
            CodeTimer.Time("dynamic", 1000000, () => GetName3(u));



            var tester = new TesterBase[]
            {
                new LinqTester(),
                new SystemReflectionTester(),
                new LiteracyTester(),
            };

            foreach (var t in tester)
            {
                t.TestCount = 100000;
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

        static MethodInfo getName;

        public static object GetName2(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (getName == null)
            {
                getName = typeof(User).GetProperty("Name").GetGetMethod();
            }
            return getName.Invoke(obj, null); //缓存了反射Name属性
        }

        public static object GetName3(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return ((dynamic)obj).Name;
        }


    }
}

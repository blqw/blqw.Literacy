using blqw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Demo
{
    class Program
    {
        public class User2
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime? Birthday { get; set; }
            public bool Sex { get; set; }
            public string AAA { get; set; }
        }

        public class User3
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime? Birthday { get; set; }
            public bool Sex { get; set; }

            [MemberMapping("AAA")]
            public DateTime BBB { get; set; }
        }
        static void Main(string[] args)
        {
            var user2 = new User2 {
                ID = 1,
                Name = "aaa",
                Sex = true,
                Birthday = DateTime.Now,
                AAA = "2014-1-1"
            };

            var user = Convert2.ToEntity<User3>(user2, false);


            //SystemReflection(100000);
            //SystemReflection_Fast(100000);
            //LiteracyGetSet(100000);
            //User u = new User();
            //CodeTimer.Initialize();
            //CodeTimer.Time("MethodInfo", 1000000, () => GetName2(u));
            //CodeTimer.Time("Literacy", 1000000, () => GetName(u));
            //CodeTimer.Time("dynamic", 1000000, () => GetName3(u));
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

        public static void SystemReflection(int count)
        {
            CodeTimer.Initialize();
            var u = new SingleModel { Name = "blqw" };
            PropertyInfo p = null;

            CodeTimer.Time("SystemReflectionInit", 1, () => {
                p = u.GetType().GetProperty("Name");
            });
            CodeTimer.Time("SystemReflection", count, () => {
                var s = p.GetValue(u, null);
                p.SetValue(u, s, null);
            });
        }

        public static void SystemReflection_Fast(int count)
        {
            CodeTimer.Initialize();
            //这种方式必须声明强类型的委托类型,必须保证参数类型正确,或者自己验证
            var u = new SingleModel { Name = "blqw" };
            Func<SingleModel, string> get = null;
            Action<SingleModel, string> set = null;

            CodeTimer.Time("SystemReflection_FastInit", 1, () => {
                var p = typeof(SingleModel).GetProperty("Name");
                get = (Func<SingleModel, string>)Delegate.CreateDelegate(typeof(Func<SingleModel, string>), p.GetGetMethod());
                set = (Action<SingleModel, string>)Delegate.CreateDelegate(typeof(Action<SingleModel, string>), p.GetSetMethod());
            });


            CodeTimer.Time("SystemReflection_Fast", count, () => {
                var s = get(u);
                set(u, s);
            });
        }

        public static void LiteracyGetSet(int count)
        {
            CodeTimer.Initialize();
            var u = new SingleModel { Name = "blqw" };
            LiteracyGetter get = null;
            LiteracySetter set = null;

            CodeTimer.Time("LiteracyGetSetInit", 1, () => {
                var p = u.GetType().GetProperty("Name");
                get = Literacy.CreateGetter(p);
                set = Literacy.CreateSetter(p);
            });

            CodeTimer.Time("LiteracyGetSet", count, () => {
                var s = get(u);
                set(u, s);
            });
        }

    }
}

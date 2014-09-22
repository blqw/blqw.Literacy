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
        static void Main(string[] args)
        {
            SystemReflection(100000);
            SystemReflection_Fast(100000);
            LiteracyGetSet(100000);
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

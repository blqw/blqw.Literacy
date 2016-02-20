using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class SystemReflectionTester : TesterBase
    {
        Func<object, object> _Get = null;
        Action<object, object> _Set = null;

        protected override void Initialize()
        {
            var t1 = State.GetType();
            var p = t1.GetProperty("Name");
            var t2 = p.PropertyType;

            var get = Delegate.CreateDelegate(
                typeof(Func<,>).MakeGenericType(t1,t2), p.GetGetMethod());
            var set = Delegate.CreateDelegate(
                typeof(Action<,>).MakeGenericType(t1, t2), p.GetSetMethod());


            var handler = (IHandler)Activator.CreateInstance(typeof(InnerHandler<,>).MakeGenericType(t1, t2), get, set);
            _Get = handler.Get;
            _Set = handler.Set;

        }

        protected override void Testing()
        {
            var s = _Get(State);
            _Set(State, s);
        }

        interface IHandler
        {
            void Set(object instance, object value);
            object Get(object instance);
        }

        class InnerHandler<T1, T2>: IHandler
        {
            public InnerHandler(Func<T1, T2> func, Action<T1, T2> action)
            {
                Func = func;
                Action = action;
            }

            Func<T1, T2> Func;

            Action<T1, T2> Action;

            public object Get(object instance)
            {
                return Func((T1)instance);
            }

            public void Set(object instance, object value)
            {
                Action((T1)instance, (T2)value);
            }

        }
    }
}

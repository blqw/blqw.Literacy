using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class LinqTester : TesterBase
    {
        Func<object, object> _Get = null;
        Action<object, object> _Set = null;

        protected override void Initialize()
        {
            var property = State.GetType().GetProperty("Name");
            var o = Expression.Parameter(typeof(object), "o");
            var cast = Expression.Convert(o, property.DeclaringType);
            var p = Expression.Property(cast, property);

            {
                var ret = Expression.Convert(p, typeof(object));
                var get = Expression.Lambda<Func<object, object>>(ret, o);
                _Get = get.Compile();
            }

            {
                var v = Expression.Parameter(typeof(object), "v");
                var val = Expression.Convert(v, property.PropertyType);
                var assign = Expression.MakeBinary(ExpressionType.Assign, p, val);
                var ret = Expression.Convert(assign, typeof(object));
                var set = Expression.Lambda<Action<object, object>>(ret, o, v);
                _Set = set.Compile();
            }
        }

        protected override void Testing()
        {
            var s = _Get(State);
            _Set(State, s);
        }
    }
}

using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.Serialization;
using blqw.Reflection;

namespace blqw.Reflection
{
    /// <summary> 输出插件
    /// </summary>
    static class ExportComponent
    {
        /// <summary> 包装反射对象
        /// </summary>
        [Export("MemberInfoWrapper")]
        [ExportMetadata("Priority", 100)]
        public static MemberInfo MemberInfoWrapper(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (member is IObjectReference)
            {
                return member;
            }
            var type = member as Type;
            if (type != null)
            {
                return TypeEx.Cache(type);
            }

            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    return new ConstructorInfoEx((ConstructorInfo)member);
                case MemberTypes.Method:
                    return new MethodInfoEx((MethodInfo)member);
                case MemberTypes.Field:
                    return new FieldInfoEx((FieldInfo)member);
                case MemberTypes.Property:
                    return new PropertyInfoEx((PropertyInfo)member);
                default:
                    return member;
            }
        }


        [Export("CreateGetter")]
        [ExportMetadata("Priority", 100)]
        public static Func<object, object> CreateGetter(MemberInfo fieldOrProperty)
        {
            var p = ObjectProperty.Cache(fieldOrProperty);
            if (p == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldOrProperty), "参数只能是字段或者属性");
            }
            if (p.CanRead)
            {
                return Convert<Func<object, object>>(p.GetValue);
            }
            return null;
        }


        [Export("CreateSetter")]
        [ExportMetadata("Priority", 100)]
        public static Action<object, object> CreateSetter(MemberInfo fieldOrProperty)
        {
            var p = ObjectProperty.Cache(fieldOrProperty);
            if (p == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldOrProperty), "参数只能是字段或者属性");
            }
            if (p.CanWrite)
            {
                return Convert<Action<object, object>>(p.SetValue);
            }
            return null;
        }

        [Export("CreateCaller")]
        [ExportMetadata("Priority", 100)]
        public static Func<object, object[], object> CreateCaller(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            var caller = Literacy.CreateCaller(method);
            return Convert<Func<object, object[], object>>((o, a) => caller(o, a));
        }


        private static T Convert<T>(T t)
        {
            var @delegate = (Delegate)(object)t;
            return (T)(object)@delegate.Method.CreateDelegate(typeof(T), @delegate.Target);
        }
    }
}

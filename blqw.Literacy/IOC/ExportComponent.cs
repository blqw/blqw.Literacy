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
    }
}

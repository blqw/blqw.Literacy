using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Reflection
{
    class MethodInfoEx : MethodInfo, IObjectReference
    {
        MethodInfo _Method;
        LiteracyCaller _Caller;

        public MethodInfoEx(MethodInfo method)
        {   
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _Method = method;
            _Caller = Literacy.Cache(method);
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return _Caller(obj, parameters);
        }

        public override bool Equals(object obj)
        {
            var m = obj as MethodInfoEx;
            if (m != null)
            {
                return _Method.Equals(m._Method);
            }
            return _Method.Equals(obj);
        }

        public override string ToString()
        {
            return Component.Converter.ToString(_Method);
        }

        #region override

        public override MethodInfo GetBaseDefinition()
        {
            return _Method.GetBaseDefinition();
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { return _Method.ReturnTypeCustomAttributes; }
        }

        public override MethodAttributes Attributes
        {
            get { return _Method.Attributes; }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _Method.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return _Method.GetParameters();
        }


        public override RuntimeMethodHandle MethodHandle
        {
            get { return _Method.MethodHandle; }
        }

        public override Type DeclaringType
        {
            get { return _Method.DeclaringType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _Method.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _Method.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _Method.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _Method.Name; }
        }

        public override Type ReflectedType
        {
            get { return _Method.ReflectedType; }
        }

        public override Delegate CreateDelegate(Type delegateType)
        {
            return _Method.CreateDelegate(delegateType);
        }

        public override Delegate CreateDelegate(Type delegateType, object target)
        {
            return _Method.CreateDelegate(delegateType, target);
        }
        public override MethodBody GetMethodBody()
        {
            return _Method.GetMethodBody();
        }

        public override CallingConventions CallingConvention
        {
            get
            {
                return _Method.CallingConvention;
            }
        }
        public override bool ContainsGenericParameters
        {
            get
            {
                return _Method.ContainsGenericParameters;
            }
        }
        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return _Method.CustomAttributes;
            }
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _Method.GetCustomAttributesData();
        }
        public override Type[] GetGenericArguments()
        {
            return _Method.GetGenericArguments();
        }
        public override MethodInfo GetGenericMethodDefinition()
        {
            return _Method.GetGenericMethodDefinition();
        }
        public override int GetHashCode()
        {
            return _Method.GetHashCode();
        }
        public override bool IsGenericMethod
        {
            get
            {
                return _Method.IsGenericMethod;
            }
        }
        public override bool IsGenericMethodDefinition
        {
            get
            {
                return _Method.IsGenericMethodDefinition;
            }
        }
        public override bool IsSecurityCritical
        {
            get
            {
                return _Method.IsSecurityCritical;
            }
        }
        public override bool IsSecuritySafeCritical
        {
            get
            {
                return _Method.IsSecuritySafeCritical;
            }
        }
        public override bool IsSecurityTransparent
        {
            get
            {
                return _Method.IsSecurityTransparent;
            }
        }
        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            return _Method.MakeGenericMethod(typeArguments);
        }
        public override MemberTypes MemberType
        {
            get
            {
                return _Method.MemberType;
            }
        }
        public override int MetadataToken
        {
            get
            {
                return _Method.MetadataToken;
            }
        }
        public override MethodImplAttributes MethodImplementationFlags
        {
            get
            {
                return _Method.MethodImplementationFlags;
            }
        }
        public override Module Module
        {
            get
            {
                return _Method.Module;
            }
        }
        public override ParameterInfo ReturnParameter
        {
            get
            {
                return _Method.ReturnParameter;
            }
        }
        public override Type ReturnType
        {
            get
            {
                return _Method.ReturnType;
            }
        } 
        #endregion

        #region IObjectReference 成员

        public object GetRealObject(StreamingContext context)
        {
            return _Method;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Reflection
{
    class ConstructorInfoEx : ConstructorInfo, IObjectReference
    {
        ConstructorInfo _Constructor;
        LiteracyNewObject _New;
        public ConstructorInfoEx(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            _Constructor = constructor;
            _New = Literacy.CreateNewObject(constructor);
        }
        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return _New(parameters);
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            return _New(parameters);
        }


        public override bool Equals(object obj)
        {
            var c = obj as ConstructorInfoEx;
            if (c != null)
            {
                return _Constructor.Equals(c._Constructor);
            }
            return _Constructor.Equals(obj);
        }

        public override string ToString()
        {
            return Component.Converter.ToString(_Constructor);
        }

        #region override
        public override MethodAttributes Attributes
        {
            get { return _Constructor.Attributes; }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _Constructor.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return _Constructor.GetParameters();
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { return _Constructor.MethodHandle; }
        }

        public override Type DeclaringType
        {
            get { return _Constructor.DeclaringType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _Constructor.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _Constructor.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _Constructor.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _Constructor.Name; }
        }

        public override Type ReflectedType
        {
            get { return _Constructor.ReflectedType; }
        }

        public override CallingConventions CallingConvention
        {
            get
            {
                return _Constructor.CallingConvention;
            }
        }
        public override bool ContainsGenericParameters
        {
            get
            {
                return _Constructor.ContainsGenericParameters;
            }
        }
        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return _Constructor.CustomAttributes;
            }
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _Constructor.GetCustomAttributesData();
        }
        public override Type[] GetGenericArguments()
        {
            return _Constructor.GetGenericArguments();
        }
        public override int GetHashCode()
        {
            return _Constructor.GetHashCode();
        }
        public override MethodBody GetMethodBody()
        {
            return _Constructor.GetMethodBody();
        }
        public override bool IsGenericMethod
        {
            get
            {
                return _Constructor.IsGenericMethod;
            }
        }
        public override bool IsGenericMethodDefinition
        {
            get
            {
                return _Constructor.IsGenericMethodDefinition;
            }
        }
        public override bool IsSecurityCritical
        {
            get
            {
                return _Constructor.IsSecurityCritical;
            }
        }
        public override bool IsSecuritySafeCritical
        {
            get
            {
                return _Constructor.IsSecuritySafeCritical;
            }
        }
        public override bool IsSecurityTransparent
        {
            get
            {
                return _Constructor.IsSecurityTransparent;
            }
        }
        public override MemberTypes MemberType
        {
            get
            {
                return _Constructor.MemberType;
            }
        }
        public override int MetadataToken
        {
            get
            {
                return _Constructor.MetadataToken;
            }
        }
        public override MethodImplAttributes MethodImplementationFlags
        {
            get
            {
                return _Constructor.MethodImplementationFlags;
            }
        }
        public override Module Module
        {
            get
            {
                return _Constructor.Module;
            }
        } 
        #endregion
        
        #region IObjectReference 成员

        public object GetRealObject(StreamingContext context)
        {
            return _Constructor;
        }

        #endregion
    }
}

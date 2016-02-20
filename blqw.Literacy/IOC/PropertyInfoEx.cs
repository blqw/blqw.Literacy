using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Reflection
{
    class PropertyInfoEx : PropertyInfo, IObjectReference
    {
        PropertyInfo _Property;
        ObjectProperty _ObjectProperty;

        public PropertyInfoEx(ObjectProperty objectProperty)
        {
            if (objectProperty == null)
            {
                throw new ArgumentNullException("objectProperty");
            }
            if (objectProperty.MemberInfo.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("不是属性(PropertyInfo)", "objectProperty");
            }
            _Property = (PropertyInfo)objectProperty.MemberInfo;
            _ObjectProperty = objectProperty;
        }

        public PropertyInfoEx(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            _Property = property;
            _ObjectProperty = ObjectProperty.Cache(property);
        }

        public override PropertyAttributes Attributes
        {
            get { return _Property.Attributes; }
        }

        public override bool CanRead
        {
            get { return _ObjectProperty.CanRead; }
        }

        public override bool CanWrite
        {
            get { return _ObjectProperty.CanWrite; }
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            if (index != null && index.Length > 0)
            {
                return _Property.GetValue(obj, invokeAttr, binder, index, culture);
            }
            return _ObjectProperty.GetValue(obj);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            if (index != null && index.Length > 0)
            {
                _Property.SetValue(obj, value, invokeAttr, binder, index, culture);
            }
            else
            {
                _ObjectProperty.SetValue(obj, value);
            }
        }

        public override Type PropertyType
        {
            get { return _Property.PropertyType; }
        }

        #region override

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            return _Property.GetAccessors(nonPublic);
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return _Property.GetGetMethod(nonPublic);
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return _Property.GetIndexParameters();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return _Property.GetSetMethod(nonPublic);
        }


        public override Type DeclaringType
        {
            get { return _Property.DeclaringType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _Property.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _Property.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _Property.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _Property.Name; }
        }

        public override Type ReflectedType
        {
            get { return _Property.ReflectedType; }
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return _Property.CustomAttributes;
            }
        }

        public override object GetConstantValue()
        {
            return _Property.GetConstantValue();
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _Property.GetCustomAttributesData();
        }
        public override int GetHashCode()
        {
            return _Property.GetHashCode();
        }
        public override MethodInfo GetMethod
        {
            get
            {
                return _Property.GetMethod;
            }
        }
        public override Type[] GetOptionalCustomModifiers()
        {
            return _Property.GetOptionalCustomModifiers();
        }
        public override object GetRawConstantValue()
        {
            return _Property.GetRawConstantValue();
        }
        public override Type[] GetRequiredCustomModifiers()
        {
            return _Property.GetRequiredCustomModifiers();
        }
        public override MemberTypes MemberType
        {
            get
            {
                return _Property.MemberType;
            }
        }
        public override int MetadataToken
        {
            get
            {
                return _Property.MetadataToken;
            }
        }
        public override Module Module
        {
            get
            {
                return _Property.Module;
            }
        }
        public override MethodInfo SetMethod
        {
            get
            {
                return _Property.SetMethod;
            }
        }
        public override bool Equals(object obj)
        {
            var p = obj as PropertyInfoEx;
            if (p != null)
            {
                return _Property.Equals(p._Property);
            }
            return _Property.Equals(obj);
        }
        public override string ToString()
        {
            return Component.Converter.ToString(_Property);
        }

        #endregion


        #region IObjectReference 成员

        public object GetRealObject(StreamingContext context)
        {
            return _Property;
        }

        #endregion
    }
}

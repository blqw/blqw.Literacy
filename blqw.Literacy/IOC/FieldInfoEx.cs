using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.Reflection
{
    class FieldInfoEx : FieldInfo, IObjectReference
    {
        FieldInfo _Field;
        ObjectProperty _ObjectProperty;

        public FieldInfoEx(ObjectProperty objectProperty)
        {
            if (objectProperty == null)
            {
                throw new ArgumentNullException("objectProperty");
            }
            if (objectProperty.MemberInfo.MemberType != MemberTypes.Field)
            {
                throw new ArgumentException("不是字段(FieldInfo)", "objectProperty");
            }
            _Field = (FieldInfo)objectProperty.MemberInfo;
            _ObjectProperty = objectProperty;
        }

        public FieldInfoEx(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _Field = field;
            _ObjectProperty = ObjectProperty.Cache(field);
        }

        public override object GetValue(object obj)
        {
            return _ObjectProperty.GetValue(obj);
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, System.Globalization.CultureInfo culture)
        {
            _ObjectProperty.SetValue(obj, value);
        }

        public override bool Equals(object obj)
        {
            var p = obj as FieldInfoEx;
            if (p != null)
            {
                return _Field.Equals(p._Field);
            }
            return _Field.Equals(obj);
        }

        public override Type DeclaringType
        {
            get { return _Field.DeclaringType; }
        }

        public override string ToString()
        {
            return Component.Converter.ToString(_Field);
        }

        #region override

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _Field.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _Field.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _Field.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _Field.Name; }
        }

        public override Type ReflectedType
        {
            get { return _Field.ReflectedType; }
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return _Field.CustomAttributes;
            }
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _Field.GetCustomAttributesData();
        }
        public override int GetHashCode()
        {
            return _Field.GetHashCode();
        }
        public override Type[] GetOptionalCustomModifiers()
        {
            return _Field.GetOptionalCustomModifiers();
        }
        public override object GetRawConstantValue()
        {
            return _Field.GetRawConstantValue();
        }
        public override Type[] GetRequiredCustomModifiers()
        {
            return _Field.GetRequiredCustomModifiers();
        }
        public override MemberTypes MemberType
        {
            get
            {
                return _Field.MemberType;
            }
        }
        public override int MetadataToken
        {
            get
            {
                return _Field.MetadataToken;
            }
        }
        public override Module Module
        {
            get
            {
                return _Field.Module;
            }
        }
        public override RuntimeFieldHandle FieldHandle
        {
            get { return _Field.FieldHandle; }
        }

        public override Type FieldType
        {
            get { return _Field.FieldType; }
        }

        public override FieldAttributes Attributes
        {
            get { return _Field.Attributes; }
        }

        #endregion

        #region IObjectReference 成员

        public object GetRealObject(StreamingContext context)
        {
            return _Field;
        }

        #endregion
    }
}

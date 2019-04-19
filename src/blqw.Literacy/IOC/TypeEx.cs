using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace blqw.Reflection
{
    class TypeEx : Type, ICustomTypeProvider, IObjectReference
    {

        #region cache
        static readonly Dictionary<Type, TypeEx> _Cache = new Dictionary<Type, TypeEx>();

        /// <summary> 从缓存中获取对象,如果不存在则创建
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeEx Cache(Type type)
        {
            if (type == null)
            {
                return null;
            }
            var typex = type as TypeEx;
            if (typex != null)
            {
                return typex;
            }
            if (_Cache.TryGetValue(type, out typex))
            {
                return typex;
            }
            lock (_Cache)
            {
                if (_Cache.TryGetValue(type, out typex))
                {
                    return typex;
                }
                typex = new TypeEx(type);
                _Cache[type] = typex;
                return typex;
            }
        }

        #endregion

        Type _Type;
        private TypeEx(Type type)
        {
            if (type is TypeEx)
            {
                throw new ArgumentException("type");
            }
            _Type = type;
            _FullName = Component.Converter.ToString(_Type);
        }

        private string _FullName;
        public override string ToString()
        {
            return _FullName;
        }

        public override bool Equals(object obj)
        {
            var p = obj as TypeEx;
            if (p != null)
            {
                return _Type.Equals(p._Type);
            }
            return _Type.Equals(obj);
        }

        public override bool Equals(Type obj)
        {
            var p = obj as TypeEx;
            if (p != null)
            {
                return _Type.Equals(p._Type);
            }
            return _Type.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _Type.GetHashCode();
        }

        SortedList<BindingFlags, MemberInfo[]> _Members = new SortedList<BindingFlags, MemberInfo[]>();
        ReaderWriterLockSlim _Lock = new ReaderWriterLockSlim();

        private T[] GetMembersCache<T>(BindingFlags flags, Func<BindingFlags, T[]> get)
            where T : MemberInfo
        {
            if (_Lock.TryEnterReadLock(1000) == false)
            {
                throw new TimeoutException("尝试获取读锁[TypeEx]超时");
            }
            MemberInfo[] values;
            if (_Members.TryGetValue(flags, out values))
            {
                _Lock.ExitReadLock();
                return (T[])values;
            }
            _Lock.ExitReadLock();
            var arr = get((BindingFlags)((int)flags & 16777215));
            arr = Wrapper(arr);
            if (_Lock.TryEnterWriteLock(1000) == false)
            {
                throw new TimeoutException("尝试获取写锁[TypeEx]超时");
            }
            if (_Members.TryGetValue(flags, out values))
            {
                _Lock.ExitWriteLock();
                return (T[])values;
            }
            _Members.Add(flags, arr);
            _Lock.ExitWriteLock();
            return arr;
        }

        #region Flags
        const BindingFlags GetConstructorsFlags = (BindingFlags)(16777216 << 1);
        const BindingFlags GetFieldsFlags = (BindingFlags)(16777216 << 2);
        const BindingFlags GetMethodsFlags = (BindingFlags)(16777216 << 3);
        const BindingFlags GetNestedTypesFlags = (BindingFlags)(16777216 << 4);
        const BindingFlags GetPropertiesFlags = (BindingFlags)(16777216 << 5);
        const BindingFlags GetMembersFlags = (BindingFlags)(16777216 << 6);
        #endregion

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return GetMembersCache(bindingAttr | GetConstructorsFlags, _Type.GetConstructors);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return GetMembersCache(bindingAttr | GetFieldsFlags, _Type.GetFields);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return GetMembersCache(bindingAttr | GetMethodsFlags, _Type.GetMethods);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return GetMembersCache(bindingAttr | GetNestedTypesFlags, _Type.GetNestedTypes);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return GetMembersCache(bindingAttr | GetPropertiesFlags, _Type.GetProperties);
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return GetMembersCache(bindingAttr | GetMembersFlags, _Type.GetMembers);
        }

        private T[] Wrapper<T>(T[] members)
            where T : MemberInfo
        {
            if (members == null || members.Length == 0)
            {
                return members;
            }
            for (int i = members.Length - 1; i >= 0; i--)
            {
                members[i] = (T)ExportComponent.MemberInfoWrapper(members[i]);
            }
            return members;
        }

        private Type[] Wrapper(Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return types;
            }
            for (int i = types.Length - 1; i >= 0; i--)
            {
                types[i] = Cache(types[i]);
            }
            return types;
        }
        //======================以下是必须的============================================


        #region protected

        static TypeEx()
        {
            GetConstructorImplHandler = Literacy.CreateCaller(typeof(Type).GetMethod("GetConstructorImpl", BindingFlags.NonPublic | BindingFlags.Instance));
            GetMethodImplHandler = Literacy.CreateCaller(typeof(Type).GetMethod("GetMethodImpl", BindingFlags.NonPublic | BindingFlags.Instance));
            GetPropertyImplHandler = Literacy.CreateCaller(typeof(Type).GetMethod("GetPropertyImpl", BindingFlags.NonPublic | BindingFlags.Instance));
        }
        readonly static LiteracyCaller GetConstructorImplHandler;
        readonly static LiteracyCaller GetMethodImplHandler;
        readonly static LiteracyCaller GetPropertyImplHandler;


        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            var c = (ConstructorInfo)GetConstructorImplHandler(_Type, bindingAttr, binder, callConvention, types, modifiers);
            if (c == null) return null;
            return new ConstructorInfoEx(c);
        }


        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            var o = (MethodInfo)GetMethodImplHandler(_Type,name, bindingAttr, binder, callConvention, types, modifiers);
            if (o == null) return null;
            return new MethodInfoEx(o);
        }


        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            var o = (PropertyInfo)GetPropertyImplHandler(_Type, name, bindingAttr, binder, returnType, types, modifiers);
            if (o == null) return null;
            return new PropertyInfoEx(o);
        }


        #endregion




        public override Assembly Assembly
        {
            get { return _Type.Assembly; }
        }

        public override string AssemblyQualifiedName
        {
            get { return _Type.AssemblyQualifiedName; }
        }

        Type _BaseType;
        public override Type BaseType
        {
            get { return _BaseType ?? (_BaseType = Cache(_Type.BaseType)); }
        }

        public override string FullName
        {
            get { return _Type.FullName; }
        }

        public override Guid GUID
        {
            get { return _Type.GUID; }
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return _Type.Attributes;
        }

        Type _GetElementType;
        public override Type GetElementType()
        {
            return _GetElementType ?? (Cache(_Type.GetElementType()));
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return _Type.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return _Type.GetEvents();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            var o = _Type.GetField(name, bindingAttr);
            if (o == null) return null;
            return new FieldInfoEx(o);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return _Type.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return _Type.GetInterfaces();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return Cache(_Type.GetNestedType(name, bindingAttr));
        }

        protected override bool HasElementTypeImpl()
        {
            return _Type.HasElementType;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            return _Type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        protected override bool IsArrayImpl()
        {
            return _Type.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return _Type.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return _Type.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return _Type.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return _Type.IsPrimitive;
        }

        public override Module Module
        {
            get { return _Type.Module; }
        }

        public override string Namespace
        {
            get { return _Type.Namespace; }
        }

        public override Type UnderlyingSystemType
        {
            get { return _Type.UnderlyingSystemType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            return _Type.GetCustomAttributes(attributeType.UnderlyingSystemType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _Type.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            return _Type.IsDefined(attributeType.UnderlyingSystemType, inherit);
        }

        public override string Name
        {
            get { return _Type.Name; }
        }

        //========================以下是重写的

        #region ICustomTypeProvider 成员

        public Type GetCustomType()
        {
            return _Type;
        }

        #endregion

        #region IObjectReference 成员

        public object GetRealObject(StreamingContext context)
        {
            return _Type;
        }

        #endregion
        Type _MakePointerType;
        public override Type MakePointerType()
        {
            return _MakePointerType ?? (_MakePointerType = _Type.MakePointerType());
        }

        public override System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
        {
            get
            {
                return _Type.StructLayoutAttribute;
            }
        }
        Type _MakeByRefType;
        public override Type MakeByRefType()
        {
            return _MakeByRefType ?? (_MakeByRefType = Cache(_Type.MakeByRefType()));
        }
        Type _MakeArrayType;
        public override Type MakeArrayType()
        {
            return _MakeArrayType ?? (_MakeArrayType = Cache(_Type.MakeArrayType()));
        }

        public override Type MakeArrayType(int rank)
        {
            if (rank == 1)
            {
                return MakeArrayType();
            }
            return Cache(_Type.MakeArrayType(rank));
        }

        public override RuntimeTypeHandle TypeHandle
        {
            get
            {
                return _Type.TypeHandle;
            }
        }

        public override GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                return _Type.GenericParameterAttributes;
            }
        }
        MemberInfo[] _GetDefaultMembers;
        public override MemberInfo[] GetDefaultMembers()
        {
            return _GetDefaultMembers ?? (_GetDefaultMembers = Wrapper(_Type.GetDefaultMembers()));
        }

        public override bool IsConstructedGenericType
        {
            get
            {
                return _Type.IsConstructedGenericType;
            }
        }
        public override Array GetEnumValues()
        {
            return _Type.GetEnumValues();
        }
        public override bool IsSecurityCritical
        {
            get
            {
                return _Type.IsSecurityCritical;
            }
        }
        public override bool IsSecuritySafeCritical
        {
            get
            {
                return _Type.IsSecuritySafeCritical;
            }
        }
        public override bool IsSecurityTransparent
        {
            get
            {
                return _Type.IsSecurityTransparent;
            }
        }

        //=========================



    }
}

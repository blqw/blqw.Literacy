using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace blqw.Reflection
{
    /// <summary> 表示一个可以快速获取或者设置其对象属性/字段值的对象
    /// </summary>
    public sealed class ObjectProperty
    {
        static readonly Dictionary<MemberInfo, ObjectProperty> _Cache = new Dictionary<MemberInfo, ObjectProperty>();

        /// <summary> 从缓存中获取对象,如果不存在则创建
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static ObjectProperty Cache(MemberInfo member)
        {
            ObjectProperty op;
            if (_Cache.TryGetValue(member, out op))
            {
                return op;
            }
            if (member.MemberType != MemberTypes.Field &&
                member.MemberType != MemberTypes.Property)
            {
                return null;
            }
            lock (_Cache)
            {
                if (_Cache.TryGetValue(member, out op))
                {
                    return op;
                }
                if (member.MemberType == MemberTypes.Field)
                {
                    op = new ObjectProperty((FieldInfo)member);
                }
                else
                {
                    op = new ObjectProperty((PropertyInfo)member);
                }
                _Cache[member] = op;
                return op;
            }
        }


        /// <summary> 表示一个可以获取或者设置其内容的对象属性
        /// </summary>
        /// <param name="property">属性信息</param>
        private ObjectProperty(PropertyInfo property)
        {
            Field = false;
            MemberInfo = property; //属性信息
            OriginalType = property.PropertyType;
            var get = property.GetGetMethod(true); //获取属性get方法,不论是否公开
            var set = property.GetSetMethod(true); //获取属性set方法,不论是否公开
            if (set != null  //set方法不为空
                && property.GetIndexParameters().Length ==0)                    
            {
                CanWrite = true; //属性可写
                Static = set.IsStatic; //属性是否为静态属性
                IsPublic = set.IsPublic;
            }
            else if (property.DeclaringType.Name.StartsWith("<>f__AnonymousType")) //匿名类
            {
                CanWrite = true;
                IsPublic = false;
            }
            if (get != null) //get方法不为空
            {
                CanRead = true; //属性可读
                Static = get.IsStatic; //get.set只要有一个静态就是静态
                IsPublic = IsPublic || get.IsPublic;
            }
            ID = System.Threading.Interlocked.Increment(ref Literacy.Sequence);
            UID = Guid.NewGuid();
            Init();
            if (set == null && CanWrite) //匿名类的属性设置特殊处理
            {
                Setter = (o, v) =>
                {
                    var field = ClassType.GetField("<" + Name + ">i__Field", (BindingFlags)(-1));
                    Setter = Literacy.CreateSetter(field, ClassType);
                    Setter(o, v);
                };
            }
            Attributes = new AttributeCollection(MemberInfo);
            var mapping = Attributes.First<IMemberMappingAttribute>();
            if (mapping != null)
            {
                MappingName = mapping.Name;
            }
        }

        /// <summary> 表示一个可以获取或者设置其内容的对象字段
        /// </summary>
        /// <param name="field">字段信息</param>
        private ObjectProperty(FieldInfo field)
        {
            Field = true; //是一个字段
            MemberInfo = field; //字段信息
            OriginalType = field.FieldType; //字段值类型
            Static = field.IsStatic; //字段是否是静态的
            IsPublic = field.IsPublic; //字段是否是公开的
            CanWrite = !field.IsLiteral; // !field.IsInitOnly; //是否可写取决于是否静态
            CanRead = true; //字段一定可以读
            Init();
            ID = System.Threading.Interlocked.Increment(ref Literacy.Sequence);
            UID = Guid.NewGuid();
            AutoField = (field.Name[0] == '<') || field.Name.Contains("<");
            Attributes = new AttributeCollection(MemberInfo);
            var mapping = Attributes.First<IMemberMappingAttribute>();
            if (mapping != null)
            {
                MappingName = mapping.Name;
            }
        }

        #region 只读属性

        /// <summary> 属性/字段信息
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary> 是否为可空值类型
        /// </summary>
        public bool Nullable { get; private set; }

        /// <summary> 属性/字段的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary> 属性/字段的类型
        /// </summary>
        public Type MemberType { get; private set; }

        /// <summary> 原始类型
        /// </summary>
        public Type OriginalType { get; private set; }

        /// <summary> 属性/字段所属对象的类型
        /// </summary>
        public Type ClassType { get; private set; }

        /// <summary> 属性/字段是否是静态
        /// </summary>
        public bool Static { get; private set; }

        /// <summary> 属性/字段是否可读
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary> 属性/字段是否可写
        /// </summary>
        public bool CanWrite { get; private set; }

        /// <summary> 当前对象是否是字段
        /// </summary>
        public bool Field { get; private set; }

        /// <summary> 当前对象是否是公开的属性/字段
        /// </summary>
        public bool IsPublic { get; private set; }

        #endregion

        #region 属性/字段访问,设置委托

        /// <summary> 用于读取对象当前属性/字段的委托
        /// </summary>
        public LiteracyGetter Getter { get; private set; }

        /// <summary> 用于设置对象当前属性/字段的委托
        /// </summary>
        public LiteracySetter Setter { get; private set; }

        #endregion

        #region 延迟编译

        private object PreGetter(object instance)
        {
            LoadGetter();
            return Getter(instance);
        }

        private void PreSetter(object instance, object value)
        {
            LoadSetter();
            Setter(instance, value);
        }

        #endregion

        #region 编译

        /// <summary> 加载Getter
        /// </summary> 
        public void LoadGetter()
        {
            if (Getter != PreGetter)
            {
                return;
            }

            lock (this)
            {
                if (Getter == PreGetter) //Getter未编译
                {
                    if (!CanRead) //当前对象不可读
                    {
                        Getter = ErrorGetter;
                    }
                    else if (Field)
                    {
                        Getter = Literacy.CreateGetter((FieldInfo)MemberInfo);
                    }
                    else
                    {
                        Getter = Literacy.CreateGetter((PropertyInfo)MemberInfo);
                    }
                }
            }
        }

        /// <summary> 加载Setter
        /// </summary> 
        public void LoadSetter()
        {
            if (Setter != PreSetter)
            {
                return;
            }
            lock (this)
            {
                if (Setter == PreSetter) //Setter未编译
                {
                    if (!CanWrite) //当前成员可写
                    {
                        Setter = ErrorSetter;
                    }
                    else if (Field)
                    {
                        Setter = Literacy.CreateSetter((FieldInfo)MemberInfo);
                    }
                    else
                    {
                        Setter = Literacy.CreateSetter((PropertyInfo)MemberInfo);
                    }
                }
            }
        }

        #endregion

        #region 异常

        private object ErrorGetter(object instance)
        {
            if (((PropertyInfo)MemberInfo).GetIndexParameters().Length > 0)
            {
                throw new MethodAccessException("索引器无法取值");
            }
            throw new MethodAccessException("属性没有公开的Get访问器");
        }

        private void ErrorSetter(object instance, object value)
        {
            if (Field) //如果当前成员不可写,则绑定异常方法
            {
                throw new FieldAccessException("无法设置ReadOnly字段");
            }
            else if (((PropertyInfo)MemberInfo).GetIndexParameters().Length > 0)
            {
                throw new MethodAccessException("索引器无法赋值");
            }
            throw new MethodAccessException("属性没有公开的Set访问器");
        }

        #endregion

        /// <summary> 初始化
        /// </summary>
        private void Init()
        {
            Name = MemberInfo.Name;
            ClassType = MemberInfo.DeclaringType;
            var nullable = System.Nullable.GetUnderlyingType(OriginalType);
            if (nullable != null)
            {
                Nullable = true;
                MemberType = nullable;
            }
            else
            {
                MemberType = OriginalType;
            }
            Getter = PreGetter;
            Setter = PreSetter;
        }

        /// <summary> 获取对象的属性/字段值
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <exception cref="ArgumentNullException">实例属性instance对象不能为null</exception>
        /// <exception cref="ArgumentException">对象无法获取属性/字段的值</exception>
        public object GetValue(object instance)
        {
            if (!CanRead)
            {
                ErrorGetter(null);
            }
            else if (instance == null)
            {
                if (Static == false)
                {
                    throw new ArgumentNullException("instance", "实例属性对象不能为null");
                }
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                throw new ArgumentException("对象[" + instance + "]无法获取[" + MemberInfo + "]的值");
            }

            try
            {
                return Getter(instance);
            }
            catch (Exception ex)
            {
                var message = $"{MemberInfo.ReflectedType.ToString()}.{Name}属性取值失败";
                Trace.WriteLine(ex, message);
                throw new TargetInvocationException(message + ",原因见内部异常", ex);
            }
        }

        /// <summary> 尝试获取对象的属性/字段值,失败返回false
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <param name="value">成功将值保存在value,失败返回null</param>
        public bool TryGetValue(object instance, out object value)
        {
            if (!CanRead)
            {
                value = null;
                return false;
            }
            if (instance == null)
            {
                if (Static == false)
                {
                    value = null;
                    return false;
                }
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                value = null;
                return false;
            }
            try
            {
                value = Getter(instance);
            }
            catch (Exception ex)
            {
                var message = $"{MemberInfo.ReflectedType.ToString()}.{Name}属性取值失败";
                Trace.WriteLine(ex, message);
                value = null;
                return false;
            }
            return true;
        }

        /// <summary> 设置对象的属性/字段值
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的实例对象</param>
        /// <param name="value">将要设置的值</param>
        /// <exception cref="ArgumentNullException">实例属性instance对象不能为null</exception>
        public void SetValue(object instance, object value)
        {
            if (!CanWrite)
            {
                ErrorSetter(null, null);
            }
            else if (instance == null)
            {
                if (Static == false)
                {
                    throw new ArgumentNullException("instance", "实例属性对象不能为null");
                }
            }
            else if ((value == null || value is DBNull) && (OriginalType.IsClass || Nullable))
            {
                Setter(instance, null);
                return;
            }
            if (MemberType.IsEnum)
            {
                var str = value as string;
                if (str != null)
                {
                    value = Enum.Parse(MemberType, str, true);
                }
                else
                {
                    value = Enum.ToObject(MemberType, value);
                }
            }
            else if (MemberType.IsInstanceOfType(value) == false)
            {
                value = Convert.ChangeType(value, MemberType);
            }
            try
            {
                Setter(instance, value);
            }
            catch (Exception ex)
            {
                var message = $"{MemberInfo.ReflectedType.ToString()}.{Name}赋值失败";
                Trace.WriteLine(ex, message);
                throw new TargetInvocationException(message + ",原因见内部异常", ex);
            }
        }

        /// <summary> 尝试设置对象的属性/字段值,失败返回false
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <param name="value">成功将值保存在value,失败返回null</param>
        public bool TrySetValue(object instance, object value)
        {
            if (!CanWrite)
            {
                return false;
            }
            if (instance == null)
            {
                if (Static == false)
                {
                    return false;
                }
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                return false;
            }
            else if ((value == null || value is DBNull) && (OriginalType.IsClass || Nullable))
            {
                Setter(instance, null);
                return true;
            }

            try
            {
                if (MemberType.IsEnum)
                {
                    var str = value as string;
                    if (str != null)
                    {
                        value = Enum.Parse(MemberType, str, true);
                    }
                }
                else if (MemberType.IsInstanceOfType(value) == false)
                {
                    value = Convert.ChangeType(value, MemberType);
                }
                Setter(instance, value);
                return true;
            }
            catch(Exception ex)
            {
                var message = $"{MemberInfo.ReflectedType.ToString()}.{Name}属性取值失败";
                Trace.WriteLine(ex, message);
                return false;
            }
        }

        /// <summary> 成员特性
        /// </summary>
        public readonly AttributeCollection Attributes;

        /// <summary> 自增id 与Literacy共享序列
        /// </summary>
        public readonly int ID;

        /// <summary> Guid
        /// </summary>
        public readonly Guid UID;

        /// <summary> 是自动属性
        /// </summary>
        public readonly bool AutoField;

        /// <summary> 实体的映射名称
        /// </summary>
        public readonly string MappingName;
    }
}
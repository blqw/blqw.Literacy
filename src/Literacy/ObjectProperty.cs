using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace blqw.Reflection
{
    /// <summary> 
    /// 表示一个可以快速获取或者设置其对象属性/字段值的对象
    /// </summary>
    public sealed class ObjectProperty
    {
        private static readonly ConcurrentDictionary<MemberInfo, ObjectProperty> _cache = new ConcurrentDictionary<MemberInfo, ObjectProperty>();

        /// <summary> 
        /// 从缓存中获取对象,如果不存在则创建
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static ObjectProperty Cache(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property)
            {
                return null;
            }
            return _cache.GetOrAdd(member, x =>
            {
                if (x.MemberType == MemberTypes.Field)
                {
                    return new ObjectProperty((FieldInfo)x);
                }
                else
                {
                    return new ObjectProperty((PropertyInfo)x);
                }
            });
        }


        /// <summary> 
        /// 表示一个可以获取或者设置其内容的对象属性
        /// </summary>
        /// <param name="property">属性信息</param>
        private ObjectProperty(MemberInfo member)
        {
            ID = Sequence.Next();
            UID = Guid.NewGuid();
            Name = member.Name;
            ClassType = member.DeclaringType;
            Getter = LazyGetter;
            Setter = LazySetter;
            MemberInfo = member; //属性信息
            Attributes = new ReadOnlyCollection<Attribute>(Attribute.GetCustomAttributes(member));
            MappingName = Attributes.OfType<IMemberMappingAttribute>().FirstOrDefault()?.Name
                ?? Attributes.OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName;

            if (member is FieldInfo field)
            {
                Field = true;
                MemberOriginalType = field.FieldType;
                Static = field.IsStatic;
                IsPublic = field.IsPublic;
                CanWrite = true;
                CanRead = !field.IsLiteral;
                AutoField = field.Name.FirstOrDefault() == '<' || field.Name.Contains("<");
            }
            else if (member is PropertyInfo property)
            {
                MemberOriginalType = property.PropertyType;
                if (member.DeclaringType.Name.StartsWith("<>f__AnonymousType"))
                {
                    IsAnonymousType = true;
                    field = ClassType.GetField("<" + Name + ">i__Field", (BindingFlags)(-1));
                    Static = field.IsStatic;
                    IsPublic = field.IsPublic;
                    CanWrite = true;
                    CanRead = true;
                }
                else
                {
                    var set = property.GetSetMethod(true); //获取属性set方法,不论是否公开
                    if (set != null && property.GetIndexParameters().Length == 0)
                    {
                        CanWrite = true; //属性可写
                        Static = set.IsStatic; //属性是否为静态属性
                        IsPublic = set.IsPublic;
                    }

                    var get = property.GetGetMethod(true); //获取属性get方法,不论是否公开
                    if (get != null) //get方法不为空
                    {
                        CanRead = true; //属性可读
                        Static = get.IsStatic; //get.set只要有一个静态就是静态
                        IsPublic = IsPublic || get.IsPublic;
                    }
                }
            }
            else
            {
                throw new NotSupportedException(nameof(member) + " 必须是属性或字段");
            }

            MemberType = System.Nullable.GetUnderlyingType(MemberOriginalType) ?? MemberOriginalType;
            Nullable = (MemberType == MemberOriginalType);
        }

        #region 只读属性

        public MemberInfo MemberInfo { get; }

        /// <summary> 
        /// 是否为可空值类型
        /// </summary>
        public bool Nullable { get; }

        /// <summary> 
        /// 属性/字段的名称
        /// </summary>
        public string Name { get; }

        /// <summary> 
        /// 属性/字段的类型
        /// </summary>
        public Type MemberType { get; }

        /// <summary> 
        /// 属性/字段的原始类型
        /// </summary>
        public Type MemberOriginalType { get; }

        /// <summary> 
        /// 属性/字段所属对象的类型
        /// </summary>
        public Type ClassType { get; }

        /// <summary> 
        /// 属性/字段是否是静态
        /// </summary>
        public bool Static { get; }

        /// <summary> 
        /// 属性/字段是否可读
        /// </summary>
        public bool CanRead { get; }

        /// <summary> 
        /// 属性/字段是否可写
        /// </summary>
        public bool CanWrite { get; }

        /// <summary> 
        /// 当前对象是否是字段
        /// </summary>
        public bool Field { get; }

        /// <summary> 
        /// 当前对象是否是公开的属性/字段
        /// </summary>
        public bool IsPublic { get; }

        /// <summary>
        /// 是否是匿名类
        /// </summary>
        public bool IsAnonymousType { get; }

        /// <summary> 
        /// 成员特性
        /// </summary>
        public IReadOnlyList<Attribute> Attributes { get; }

        /// <summary> 
        /// 自增id 与Literacy共享序列
        /// </summary>
        public int ID { get; } = Sequence.Next();

        /// <summary> 
        /// Guid
        /// </summary>
        public Guid UID { get; } = Guid.NewGuid();

        /// <summary> 
        /// 自动属性对应的字段
        /// </summary>
        public bool AutoField { get; }

        /// <summary> 
        /// 实体的映射名称
        /// </summary>
        public string MappingName { get; }

        #endregion

        #region 属性/字段访问,设置委托

        /// <summary> 
        /// 用于读取对象当前属性/字段的委托
        /// </summary>
        public MemberGetter Getter { get; private set; }

        /// <summary> 
        /// 用于设置对象当前属性/字段的委托
        /// </summary>
        public MamberSetter Setter { get; private set; }

        #endregion

        #region 延迟编译

        private object LazyGetter(object instance)
        {
            if (Getter == LazyGetter)
            {
                BuildGetter();
            }
            return Getter(instance);
        }

        private void LazySetter(object instance, object value)
        {
            if (Setter == LazySetter)
            {
                BuildSetter();
            }
            Setter(instance, value);
        }

        #endregion

        #region 编译

        /// <summary> 
        /// 编译 Getter
        /// </summary> 
        public void BuildGetter()
        {
            lock (this)
            {
                if (Getter == LazyGetter) //Getter未编译
                {
                    if (!CanRead) //当前对象不可读
                    {
                        Getter = ErrorGetter;
                    }
                    else if (IsAnonymousType)
                    {
                        var field = ClassType.GetField("<" + Name + ">i__Field", (BindingFlags)(-1));
                        Getter = Literacy.CreateGetter(field);
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

        /// <summary> 
        /// 编译 Setter
        /// </summary> 
        public void BuildSetter()
        {
            lock (this)
            {
                if (Setter == LazySetter) //Setter未编译
                {
                    if (!CanWrite) //当前成员不可写
                    {
                        Setter = ErrorSetter;
                    }
                    else if (IsAnonymousType)
                    {
                        var field = ClassType.GetField("<" + Name + ">i__Field", (BindingFlags)(-1));
                        Setter = Literacy.CreateSetter(field);
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

        /// <summary> 
        /// 获取对象的属性/字段值
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
                    throw new ArgumentNullException(nameof(instance), "实例属性对象不能为null");
                }
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                throw new ArgumentException($"对象[{instance}]无法获取[{MemberInfo.ReflectedType.ToString()}.{Name}]的值");
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

        /// <summary> 
        /// 设置对象的属性/字段值
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
                    throw new ArgumentNullException(nameof(instance), "实例属性对象不能为null");
                }
            }
            else if ((value == null || value is DBNull) && (MemberType.IsClass || Nullable))
            {
                Setter(instance, MemberType == typeof(DBNull) ? DBNull.Value : null);
                return;
            }
            if (MemberType.IsEnum)
            {
                if (value is string str)
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

        /// <summary> 
        /// 尝试设置对象的属性/字段值,失败返回false
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
            else if ((value == null || value is DBNull) && (MemberOriginalType.IsClass || Nullable))
            {
                Setter(instance, MemberType == typeof(DBNull) ? DBNull.Value : null);
                return true;
            }

            try
            {
                if (MemberType.IsEnum)
                {
                    if (value is string str)
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
                Setter(instance, value);
                return true;
            }
            catch (Exception ex)
            {
                var message = $"{MemberInfo.ReflectedType.ToString()}.{Name}属性取值失败";
                Trace.WriteLine(ex, message);
                return false;
            }
        }
    }
}

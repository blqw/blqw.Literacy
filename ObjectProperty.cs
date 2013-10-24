using System;
using System.Reflection;

namespace blqw
{
    /// <summary> 表示一个可以快速获取或者设置其对象属性/字段值的对象
    /// </summary>
    public sealed class ObjectProperty
    {
        #region 私有字段
        //Getter委托声明的返回类型
        static readonly Type GetReturnType = typeof(Object);
        //Getter委托声明的参数类型
        static readonly Type[] GetParameterTypes = new[] { GetReturnType };
        //Setter委托声明的返回类型
        static readonly Type SetReturnType = typeof(Boolean);
        //Setter委托声明的参数类型
        static readonly Type[] SetParameterTypes = new[] { GetReturnType, GetReturnType };
        //InvalidCastException异常类型
        static readonly Type CastExceptionType = typeof(InvalidCastException);
        //当前对象保存的成员,可以是字段和属性
        MemberInfo _Member;
        #endregion

        private void Init()
        {
            Name = _Member.Name;
            ClassType = _Member.DeclaringType;
            if (OriginalType.IsValueType)
            {
                MemberType = System.Nullable.GetUnderlyingType(OriginalType);
                Nullable = MemberType != null;
            }
            MemberType = MemberType ?? OriginalType;
            Getter = PreGetter;
            Setter = PreSetter;
        }

        /// <summary> 表示一个可以获取或者设置其内容的对象属性
        /// </summary>
        /// <param name="property">属性信息</param>
        public ObjectProperty(PropertyInfo property)
        {
            Field = false;
            _Member = property;                     //属性信息
            OriginalType = property.PropertyType;
            var get = property.GetGetMethod(true);  //获取属性get方法,不论是否公开
            var set = property.GetSetMethod(true);  //获取属性set方法,不论是否公开
            if (set != null)                        //set方法不为空                    
            {
                CanWrite = true;                    //属性可写
                Static = set.IsStatic;              //属性是否为静态属性
                IsPublic = set.IsPublic;
            }
            if (get != null)                        //get方法不为空
            {
                CanRead = true;                     //属性可读
                Static = get.IsStatic;              //get.set只要有一个静态就是静态
                IsPublic = IsPublic || get.IsPublic;
            }
            Init();
        }

        /// <summary> 表示一个可以获取或者设置其内容的对象字段
        /// </summary>
        /// <param name="field">字段信息</param>
        public ObjectProperty(FieldInfo field)
        {
            Field = true;                           //是一个字段
            _Member = field;                        //字段信息
            OriginalType = field.FieldType;           //字段值类型
            Static = field.IsStatic;                //字段是否是静态的
            IsPublic = field.IsPublic;              //字段是否是公开的
            CanWrite = !field.IsInitOnly;           //是否可写取决于ReadOnly
            CanRead = true;                         //字段一定可以读
            Init();
        }

        #region 只读属性
        /// <summary> 属性/字段成员元数据
        /// </summary>
        public MemberInfo MemberInfo
        {
            get { return _Member; }
        }
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
        object PreGetter(object instance)
        {
            LoadGetter();
            return Getter(instance);
        }

        void PreSetter(object instance, object value)
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
                if (Getter == PreGetter)//Getter未编译
                {
                    if (!CanRead)    //当前对象不可读
                    {
                        Getter = ErrorGetter;
                    }
                    else if (Field)
                    {
                        Getter = Literacy.CreateGetter((FieldInfo)_Member);
                    }
                    else
                    {
                        Getter = Literacy.CreateGetter((PropertyInfo)_Member);
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
                if (Setter == PreSetter)//Setter未编译
                {
                    if (!CanWrite)   //当前成员可读
                    {
                        Setter = ErrorSetter;
                    }
                    else if (Field)
                    {
                        Setter = Literacy.CreateSetter((FieldInfo)_Member);
                    }
                    else
                    {
                        Setter = Literacy.CreateSetter((PropertyInfo)_Member);
                    }
                }
            }
        }
        #endregion

        #region 异常
        object ErrorGetter(object instance)
        {
            throw new Exception("属性没有公开的Get访问器");
        }

        void ErrorSetter(object instance, object value)
        {
            if (Field)  //如果当前成员不可写,则绑定异常方法
            {
                throw new Exception("无法设置ReadOnly字段");
            }
            else
            {
                throw new Exception("属性没有公开的Set访问器");
            }
        }
        #endregion



        /// <summary> 获取对象的属性/字段值
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        public object GetValue(object instance)
        {
            if (!CanRead)
            {
                ErrorGetter(null);
            }
            else if (instance == null && Static == false)
            {
                throw new Exception("实例对象不能为null");
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                throw new Exception("无法使用对象[" + instance.ToString() + "]获取[" + _Member.ToString() + "]的值");
            }
            return Getter(instance);
        }
        /// <summary> 尝试获取对象的属性/字段值,失败返回false
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的对象</param>
        /// <param name="value">成功将值保存在value,失败返回null</param>
        public bool TryGetValue(object instance, out object value)
        {
            if (!CanWrite)
            {
                value = null;
                return false;
            }
            else if (instance == null && Static == false)
            {
                value = null;
                return false;
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                value = null;
                return false;
            }
            value = Getter(instance);
            return true;
        }

        /// <summary> 设置对象的属性/字段值
        /// </summary>
        /// <param name="instance">将要获取其属性/字段值的实例对象</param>
        /// <param name="value">将要设置的值</param>
        public void SetValue(object instance, object value)
        {
            if (!CanRead)
            {
                ErrorSetter(null, null);
            }
            else if (instance == null && Static == false)
            {
                throw new Exception("实例对象不能为null");
            }
            else if (this.Nullable && (value == null || value is DBNull))
            {
                Setter(instance, null);
                return;
            }
            if (MemberType.IsInstanceOfType(value) == false)
            {
                value = Convert.ChangeType(value, MemberType);
            }
            Setter(instance, value);
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
            else if (instance == null && Static == false)
            {
                return false;
            }
            else if (ClassType.IsInstanceOfType(instance) == false)
            {
                return false;
            }
            else if (MemberType.IsInstanceOfType(value) == false)
            {
                return false;
            }
            else if (this.Nullable && (value == null || value is DBNull))
            {
                Setter(instance, null);
                return true;
            }
            try
            {
                Setter(instance, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }


}
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;

namespace blqw.Reflection
{
    /// <summary> 对象属性,字段访问组件
    /// </summary>
    public class Literacy : ILoadMember
    {
        #region Cache

        /// <summary>
        /// 方法缓存
        /// </summary>
        private static readonly ConcurrentDictionary<MethodInfo, MethodInvoker> _invokers = new ConcurrentDictionary<MethodInfo, MethodInvoker>(2, 255);

        /// <summary> 
        /// Literacy 缓存
        /// </summary>
        private static readonly TypeCache<Literacy> _literacies = new TypeCache<Literacy>();

        /// <summary> 
        /// 获取缓存
        /// </summary>
        /// <param name="type">反射对象类型</param>
        /// <param name="ignoreCase">属性/字段名称是否忽略大小写</param>
        /// <exception cref="ArgumentException">缓存中的对象类型与参数type不一致</exception>
        /// <exception cref="ArgumentNullException">参数type为null</exception>
        public static Literacy Cache(Type type, bool ignoreCase)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var lit = _literacies.GetOrCreate(type, x => new Literacy(x, false));
            return ignoreCase ? lit.GetIgnoreCaseInstance() : lit;
        }


        /// <summary> 
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">反射对象类型</typeparam>
        /// <param name="ignoreCase">属性/字段名称是否忽略大小写</param>
        /// <exception cref="ArgumentException">缓存中的对象类型与参数type不一致</exception>
        /// <exception cref="ArgumentNullException">参数type为null</exception>
        public static Literacy Cache<T>(bool ignoreCase)
        {
            var lit = _literacies.GetOrCreate<T>(x => new Literacy(x, false));
            return ignoreCase ? lit.GetIgnoreCaseInstance() : lit;
        }

        /// <summary> 
        /// 获取方法缓存
        /// </summary>
        /// <param name="method">调用方法</param>
        /// <exception cref="ArgumentNullException">参数method为null</exception>
        public static MethodInvoker Cache(MethodInfo method) =>
            _invokers.GetOrAdd(method ?? throw new ArgumentNullException(nameof(method)), x => CreateCaller(x));

        #endregion

        /// <summary> 
        /// 对象类型
        /// </summary>
        public readonly Type Type;

        /// <summary> 
        /// 对象属性集合
        /// </summary>
        public ObjectPropertyCollection Property { get; }

        /// <summary> 
        /// 对象字段集合
        /// </summary>
        public ObjectPropertyCollection Field { get; private set; }

        /// <summary> 
        /// 自增id 与ObjectProperty共享序列
        /// </summary>
        public int ID { get; } = Sequence.Next();

        /// <summary> 
        /// Guid
        /// </summary>
        public Guid UID { get; } = Guid.NewGuid();

        #region 私有的

        //private ObjectConstructor _defaultConstructor;

        //private object LazyBuildConstructor(params object[] args)
        //{
        //    var call = BuildConstructor(Type);
        //    if (call != null)
        //    {
        //        _defaultConstructor = call;
        //        return call();
        //    }
        //    if (Type.Name.StartsWith("<>f__AnonymousType")) //匿名类
        //    {
        //        var ctor = Type.GetConstructors()[0];
        //        if (ctor != null)
        //        {
        //            var ps = ctor.GetParameters();
        //            args = new object[ps.Length];
        //            for (var i = 0; i < ps.Length; i++)
        //            {
        //                var type = ps[i].ParameterType;
        //                if (type.IsValueType)
        //                {
        //                    args[i] = Activator.CreateInstance(type);
        //                }
        //            }
        //            call = CreateNewObject(ctor);
        //            return call(args);
        //        }
        //    }
        //    _defaultConstructor = ErrorNewObject;
        //    return _defaultConstructor();
        //}

        private object ErrorNewObject(params object[] args)
        {
            throw new Exception("没有无参的构造函数");
        }

        private Literacy _ignoreCaseInstance;

        private Literacy GetIgnoreCaseInstance()
        {
            if (_ignoreCaseInstance != null)
            {
                return _ignoreCaseInstance;
            }
            lock (this)
            {
                return _ignoreCaseInstance ?? (_ignoreCaseInstance = new Literacy(Type, true));
            }
        }
        #endregion

        #region 构造函数

        /// <summary> 
        /// 初始化对象属性,字段访问组件,建立大小写敏感的访问实例
        /// </summary>
        /// <param name="type">需快速访问的类型</param>
        public Literacy(Type type)
            : this(type, false)
        {
        }

        /// <summary> 
        /// 初始化对象属性,字段访问组件,ignoreCase参数指示是否需要区分大小写
        /// </summary>
        /// <param name="type">需快速访问的类型</param>
        /// <param name="ignoreCase">是否区分大小写(不区分大小写时应保证类中没有同名的(仅大小写不同的)属性或字段)</param>
        private Literacy(Type type, bool ignoreCase)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            //_defaultConstructor = LazyBuildConstructor;
            _ignoreCaseInstance = ignoreCase ? this : null;
            Property = new ObjectPropertyCollection(ignoreCase);
            Property.AddRange(Type.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        #endregion

        ///// <summary>
        ///// 调用对象的无参构造函数,新建对象
        ///// </summary>
        public object NewObject() => Activator.CreateInstance(Type);
        public object NewObject(params object[] args) => Activator.CreateInstance(Type, args);

        public object UninitializedNewObject() => FormatterServices.GetUninitializedObject(Type);
        #region ILoadMember

        /// <summary> 加载标识 
        /// <para>1  公共实例字段</para>
        /// <para>2  非公共实例字段</para>
        /// <para>4  静态字段</para>
        /// <para>8  非公共实例属性</para>
        /// <para>16 静态属性</para>
        /// </summary>
        private int _LoadFlag;

        /// <summary> 加载更多的属性或字段
        /// </summary>
        public ILoadMember Load
        {
            get
            {
                return this;
            }
        }

        #region ILoadMember
        /// <summary> 加载公开的实例字段
        /// </summary>
        void ILoadMember.PublicField()
        {
            if (Loaded(1))
            {
                return;
            }
            if (Field == null)
            {
                Field = new ObjectPropertyCollection(Property.IgnoreCase);
            }
            const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance;
            foreach (var f in Type.GetFields(bf))
            {
                Field.Add(ObjectProperty.Cache(f));
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载非公开的实例字段
        /// </summary>
        void ILoadMember.NonPublicField()
        {
            if (Loaded(2))
            {
                return;
            }
            if (Field == null)
            {
                Field = new ObjectPropertyCollection(Property.IgnoreCase);
            }
            const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var f in Type.GetFields(bf))
            {
                Field.Add(ObjectProperty.Cache(f));
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载公开的静态的字段,参数hasNonPublic为true,则非公开的静态字段也一起加载
        /// </summary>
        /// <param name="hasNonPublic">是否一起加载非公开的静态字段</param>
        void ILoadMember.StaticField(bool hasNonPublic)
        {
            if (Loaded(hasNonPublic ? 3 : 4))
            {
                return;
            }
            if (Field == null)
            {
                Field = new ObjectPropertyCollection(Property.IgnoreCase);
            }
            var bf = BindingFlags.Public | BindingFlags.Static;
            if (hasNonPublic)
            {
                bf |= BindingFlags.NonPublic;
            }
            foreach (var f in Type.GetFields(bf))
            {
                if (Field.ContainsKey(f.Name) == false)
                {
                    Field.Add(ObjectProperty.Cache(f));
                }
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载非公开的实例属性
        /// </summary>
        void ILoadMember.NonPublicProperty()
        {
            if (Loaded(5))
            {
                return;
            }
            const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var p in Type.GetProperties(bf))
            {
                if (p.GetIndexParameters().Length == 0)
                {
                    Property.Add(ObjectProperty.Cache(p));
                }
            }
            Monitor.Exit(this);
        }

        /// <summary> 加载公开的静态的属性,参数hasNonPublic为true,则非公开的静态属性也一起加载
        /// </summary>
        /// <param name="hasNonPublic">是否一起加载非公开的静态字段</param>
        void ILoadMember.StaticProperty(bool hasNonPublic)
        {
            if (Loaded(hasNonPublic ? 6 : 7))
            {
                return;
            }
            var bf = BindingFlags.Public | BindingFlags.Static;
            if (hasNonPublic)
            {
                bf |= BindingFlags.NonPublic;
            }
            foreach (var p in Type.GetProperties(bf))
            {
                if (p.GetIndexParameters().Length == 0 && Property.ContainsKey(p.Name) == false)
                {
                    Property.Add(ObjectProperty.Cache(p));
                }
            }
            Monitor.Exit(this);
        }
        #endregion

        /// <summary> 判断是否已加载
        /// </summary>
        /// <param name="flag">加载标识</param>
        private bool Loaded(int flag)
        {
            flag = 1 << flag;
            if ((_LoadFlag & flag) == 0)
            {
                Monitor.Enter(this);
                if ((_LoadFlag & flag) == 0)
                {
                    _LoadFlag |= flag;
                    return false;
                }
                Monitor.Exit(this);
            }
            return true;
        }

        #endregion

        private IReadOnlyList<Attribute> _attributes;

        public IReadOnlyList<Attribute> Attributes
        {
            get { return _attributes ?? (_attributes = new ReadOnlyCollection<Attribute>(Attribute.GetCustomAttributes(Type))); }
        }

        #region 静态的

        private static ConstructorInfo FindConstructor(Type instanceType, Type[] argumentTypes, out int?[] parameterMap)
        {
            foreach (var constructor in instanceType.GetTypeInfo().DeclaredConstructors)
            {
                if (constructor.IsStatic)
                {
                    continue;
                }
                if (TryCreateParameterMap(constructor.GetParameters(), argumentTypes, out parameterMap))
                {
                    return constructor;
                }
            }
            parameterMap = null;
            return null;
        }


        private static bool TryCreateParameterMap(ParameterInfo[] constructorParameters, Type[] argumentTypes, out int?[] parameterMap)
        {
            parameterMap = new int?[argumentTypes.Length];
            foreach (var p in constructorParameters)
            {
                var foundMatch = false;

                for (var i = 0; i < argumentTypes.Length; i++)
                {
                    if (parameterMap[i] != null)
                    {
                        continue;
                    }
                    var argType = argumentTypes[i].GetTypeInfo();
                    if (p.ParameterType.GetTypeInfo().IsAssignableFrom(argType))
                    {
                        foundMatch = true;
                        parameterMap[i] = p.Position;
                        break;
                    }
                }

                if (!foundMatch && !p.HasDefaultValue)
                {
                    return false;
                }
            }
            return true;
        }

        ///// <summary> 
        ///// IL构造一个用于调用对象构造函数的委托
        ///// </summary>
        ///// <param name="type">获取构造函数的对象</param>
        ///// <param name="argTypes">构造函数的参数,默认null</param>
        ///// <exception cref="ArgumentNullException">参数type为null</exception>
        //public static ObjectConstructor BuildConstructor(Type type, Type[] argTypes = null)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException("type");
        //    }
        //    if (argTypes == null)
        //    {
        //        argTypes = Type.EmptyTypes;
        //    }
        //    // 无参值类型构造方法
        //    if (type.IsValueType && argTypes.Length == 0)
        //    {
        //        var dm = new DynamicMethod("", typeof(object), ConstTypes.Objects, type, true);
        //        var il = dm.GetILGenerator();
        //        il.Emit(OpCodes.Ldloca_S, il.DeclareLocal(type));
        //        il.Emit(OpCodes.Initobj, type);
        //        il.Emit(OpCodes.Ldloc_0);
        //        il.Emit(OpCodes.Box, type);
        //        il.Emit(OpCodes.Ret);
        //        return (ObjectConstructor)dm.CreateDelegate(typeof(ObjectConstructor));
        //    }

        //    const BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        //    int?[] parameterMap = null;
        //    var ctor = argTypes.Length == 0 ? type.GetConstructor(bf, null, argTypes, null) : FindConstructor(type, argTypes, out parameterMap);
        //    if (ctor == null)
        //    {
        //        return null;
        //    }
        //    return CreateNewObject(ctor, parameterMap);
        //}

        ///// <summary> IL构造一个用于调用对象构造函数的委托
        ///// </summary>
        ///// <param name="ctor">构造函数</param>
        ///// <exception cref="NotImplementedException">不支持结构的有参构造函数</exception>
        //public static ObjectConstructor CreateNewObject(ConstructorInfo ctor, int?[] parameterMap = null)
        //{
        //    if (ctor == null)
        //    {
        //        return null;
        //    }
        //    var type = ctor.DeclaringType;
        //    var dm = new DynamicMethod("", typeof(object), ConstTypes.Objects, typeof(object), true);
        //    var ps = ctor.GetParameters();
        //    var il = dm.GetILGenerator();

        //    for (var i = 0; i < ps.Length; i++)
        //    {
        //        il.Emit(OpCodes.Ldarg_0);
        //        if (parameterMap == null)
        //        {
        //            il.Emit(OpCodes.Ldc_I4, i);
        //        }
        //        else if (parameterMap[i] == null)
        //        {
        //            il.Emit(OpCodes.Ldobj, ps[i].DefaultValue);
        //        }
        //        else
        //        {
        //            il.Emit(OpCodes.Ldc_I4, parameterMap[i].Value);
        //        }
        //        il.Emit(OpCodes.Ldelem_Ref);
        //        EmitCast(il, ps[i].ParameterType, false);
        //    }
        //    il.Emit(OpCodes.Newobj, ctor);
        //    if (type.IsValueType)
        //    {
        //        il.Emit(OpCodes.Box, type);
        //    }
        //    il.Emit(OpCodes.Ret);
        //    return (ObjectConstructor)dm.CreateDelegate(typeof(ObjectConstructor));
        //}

        /// <summary> IL构造一个用于获取对象属性值的委托
        /// </summary>
        public static MemberGetter CreateGetter(PropertyInfo prop, Type owner = null)
        {
            if (prop == null)
            {
                return null;
            }
            var dm = new DynamicMethod("", typeof(object), ConstTypes.Object, owner ?? GetOwnerType(prop), true);
            var il = dm.GetILGenerator();
            var met = prop.GetGetMethod(true);
            if (met == null)
            {
                return null;
            }
            if (met.IsStatic)
            {
                il.Emit(OpCodes.Call, met);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, prop.DeclaringType);
                if (prop.DeclaringType.IsValueType)
                {
                    il.Emit(OpCodes.Call, met);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, met);
                }
            }
            if (prop.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, prop.PropertyType);
            }
            il.Emit(OpCodes.Ret);
            return (MemberGetter)dm.CreateDelegate(typeof(MemberGetter));
        }

        /// <summary> IL构造一个用于获取对象字段值的委托
        /// </summary>
        public static MemberGetter CreateGetter(FieldInfo field, Type owner = null)
        {
            if (field == null)
            {
                return null;
            }
            var dm = new DynamicMethod("", typeof(object), ConstTypes.Object, owner ?? GetOwnerType(field), true);
            var il = dm.GetILGenerator();
            if (field.IsStatic)
            {
                il.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, field.DeclaringType);
                il.Emit(OpCodes.Ldfld, field);
            }
            if (field.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Box, field.FieldType);
            }
            il.Emit(OpCodes.Ret);
            return (MemberGetter)dm.CreateDelegate(typeof(MemberGetter));
        }

        /// <summary> IL构造一个用于设置对象属性值的委托
        /// </summary>
        public static MamberSetter CreateSetter(PropertyInfo prop, Type owner = null)
        {
            if (prop == null)
            {
                return null;
            }
            if (prop.DeclaringType.IsValueType)
            {
                throw new NotSupportedException("值类型无法通过方法给其属性或字段赋值");
            }
            var dm = new DynamicMethod("", null, ConstTypes.ObjectObject, owner ?? GetOwnerType(prop), true);
            var set = prop.GetSetMethod(true);
            if (set == null)
            {
                return null;
            }
            var il = dm.GetILGenerator();

            if (set.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, prop.PropertyType, false);
                il.Emit(OpCodes.Call, set);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, prop.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, prop.PropertyType, false);
                if (prop.DeclaringType.IsValueType)
                {
                    il.Emit(OpCodes.Call, set);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, set);
                }
            }
            il.Emit(OpCodes.Ret);

            return (MamberSetter)dm.CreateDelegate(typeof(MamberSetter));
        }

        /// <summary> IL构造一个用于设置对象字段值的委托
        /// </summary>
        public static MamberSetter CreateSetter(FieldInfo field, Type owner = null)
        {
            if (field == null || field.IsLiteral)
            {
                return null;
            }
            if (field.DeclaringType.IsValueType) //
            {
                throw new ArgumentException("值类型无法通过方法给其属性或字段赋值");
            }
            var dm = new DynamicMethod("", null, ConstTypes.ObjectObject, owner ?? GetOwnerType(field), true);
            var il = dm.GetILGenerator();

            if (field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, field.FieldType, false);
                il.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, field.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, field.FieldType, false);
                il.Emit(OpCodes.Stfld, field);
            }
            il.Emit(OpCodes.Ret);
            return (MamberSetter)dm.CreateDelegate(typeof(MamberSetter));
        }

        /// <summary> IL构造一个用于执行方法的委托
        /// </summary>
        /// <param name="method">方法</param>
        public static MethodInvoker CreateCaller(MethodInfo method, Type owner = null)
        {
            if (method == null)
            {
                return null;
            }

            var dm = new DynamicMethod("", typeof(object), ConstTypes.ObjectObjects, owner ?? GetOwnerType(method), true);

            var il = dm.GetILGenerator();

            var isRef = false;

            var ps = method.GetParameters();
            LocalBuilder[] loc = new LocalBuilder[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                Type pt = p.ParameterType;
                if (pt.IsByRef) //ref,out获取他的实际类型
                {
                    isRef = true;
                    pt = pt.GetElementType();
                }

                loc[i] = il.DeclareLocal(pt);
                if (p.IsOut == false)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    EmitCast(il, pt, false);
                    il.Emit(OpCodes.Stloc, loc[i]); //保存到本地变量
                }
            }

            if (method.IsStatic == false)
            {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, method.DeclaringType);
            }
            //将参数加载到参数堆栈
            foreach (var pa in method.GetParameters())
            {
                if (pa.ParameterType.IsByRef) //out或ref
                {
                    il.Emit(OpCodes.Ldloca_S, loc[pa.Position]);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, loc[pa.Position]);
                    loc[pa.Position] = null;
                }
            }
            LocalBuilder ret = null;
            if (method.ReturnType != typeof(void))
            {
                ret = il.DeclareLocal(method.ReturnType);
            }

            if (method.IsStatic || method.DeclaringType.IsValueType)
            {
                il.Emit(OpCodes.Call, method);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, method);
            }

            //设置参数
            if (isRef)
            {
                for (int i = 0; i < loc.Length; i++)
                {
                    var l = loc[i];
                    if (l == null)
                    {
                        continue;
                    }
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldloc, l);
                    if (l.LocalType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, l.LocalType);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            if (ret == null)
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if (method.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, method.ReturnType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, typeof(object));
            }

            il.Emit(OpCodes.Ret);

            return (MethodInvoker)dm.CreateDelegate(typeof(MethodInvoker));
        }

        /// <summary> IL类型转换指令
        /// </summary>
        private static void EmitCast(ILGenerator il, Type type, bool check = true)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
                if (check && Nullable.GetUnderlyingType(type) == null)
                {
                    var t = il.DeclareLocal(type);
                    il.Emit(OpCodes.Stloc, t);
                    il.Emit(OpCodes.Ldloca_S, t);
                }
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary> 如果是数组,则获取数组中元素的类型
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private static Type GetOwnerType(MemberInfo member)
        {
            //owner 是一个接口、一个数组、一个开放式泛型类型或一个泛型类型或方法的类型参数。
            Type type = member.ReflectedType;
            while (true)
            {
                if (type.IsArray)
                {
                    type = member.ReflectedType.GetElementType();
                }
                else if (
                    type.IsGenericParameter ||
                    type.IsInterface)
                {
                    return typeof(object);
                }
                else
                {
                    return type;
                }
            }
        }
        #endregion
    }
}

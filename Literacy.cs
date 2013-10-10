using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace blqw
{
    /// <summary> 对象属性,字段访问组件
    /// </summary>
    public class Literacy : ILoadMember
    {
        #region Cache

        static Dictionary<Type, Literacy> _Items = new Dictionary<Type, Literacy>(255);
        static Dictionary<Type, Literacy> _IgnoreCaseItems = new Dictionary<Type, Literacy>(255);
        static Dictionary<MethodInfo, LiteracyCaller> _Callers = new Dictionary<MethodInfo, LiteracyCaller>(255);
        /// <summary> 获取缓存
        /// </summary>
        /// <param name="type">反射对象类型</param>
        /// <param name="ignoreCase">属性/字段名称是否忽略大小写</param>
        /// <returns></returns>
        public static Literacy Cache(Type type, bool ignoreCase)
        {
            Literacy lit;
            Dictionary<Type, Literacy> item = ignoreCase ? _IgnoreCaseItems : _Items;
            if (item.TryGetValue(type, out lit))
            {
                if (lit._Type != type)
                {
                    throw new Exception("缓存中的对象反射类型与参数type不一致!");
                }
            }
            else
            {
                lock (item)
                {
                    if (item.TryGetValue(type, out lit) == false)
                    {
                        lit = new Literacy(type);
                        item.Add(type, lit);
                    }
                }
            }
            return lit;
        }

        /// <summary> 获取缓存
        /// </summary>
        /// <param name="type">反射对象类型</param>
        /// <param name="ignoreCase">属性/字段名称是否忽略大小写</param>
        /// <returns></returns>
        public static LiteracyCaller Cache(MethodInfo method)
        {
            LiteracyCaller caller;
            if (_Callers.TryGetValue(method, out caller))
            {
                return caller;
            }
            else
            {
                lock (_Callers)
                {
                    if (_Callers.TryGetValue(method, out caller) == false)
                    {
                        caller = Literacy.CreateCaller(method);
                        _Callers.Add(method, caller);
                    }
                }
            }
            return caller;
        }

        #endregion


        public Type Type
        {
            get { return _Type; }
        }
        #region 私有的
        Type _Type;
        LiteracyNewObject _CallNewObject;
        //Getter委托声明的返回类型
        static readonly Type Type_Object = typeof(Object);
        //Getter委托声明的参数类型
        static readonly Type[] Types_Object = new[] { Type_Object };
        //Setter委托声明的参数类型
        static readonly Type[] Types_2Object = new[] { Type_Object, Type_Object };

        static readonly Type[] Types_Objects = new[] { typeof(object[]) };

        static readonly Type[] Types_Object_Objects = new[] { Type_Object, typeof(object[]) };

        object PreNewObject(params object[] args)
        {
            _CallNewObject = CreateNewObject(_Type);
            if (_CallNewObject == null)
            {
                _CallNewObject = ErrorNewObject;
            }
            return _CallNewObject();
        }

        object ErrorNewObject(params object[] args)
        {
            throw new Exception("没有无参的构造函数");
        }
        #endregion

        #region 构造函数
        /// <summary> 初始化对象属性,字段访问组件,建立大小写敏感的访问实例
        /// </summary>
        /// <param name="type">需快速访问的类型</param>
        public Literacy(Type type) : this(type, false) { }

        /// <summary> 初始化对象属性,字段访问组件,ignoreCase参数指示是否需要区分大小写
        /// </summary>
        /// <param name="type">需快速访问的类型</param>
        /// <param name="ignoreCase">是否忽略大小写(不区分大小写时应保证类中没有同名的(仅大小写不同的)属性或字段)</param>
        public Literacy(Type type, bool ignoreCase)
        {
            _Type = type;
            _CallNewObject = PreNewObject;
            Property = new ObjectPropertyCollection(ignoreCase);
            foreach (var p in type.GetProperties())
            {
                if (p.GetIndexParameters().Length == 0)//排除索引器
                {
                    if (!Property.ContainsKey(p.Name))
                    {
                        var a = new ObjectProperty(p);
                        Property.Add(a);
                    }
                }
            }

        }
        #endregion

        /// <summary> 属性集合
        /// </summary>
        public ObjectPropertyCollection Property { get; private set; }

        /// <summary> 字段集合
        /// </summary>
        public ObjectPropertyCollection Field { get; private set; }

        /// <summary> 调用对象的无参构造函数,新建对象
        /// </summary>
        /// <returns></returns>
        public object NewObject()
        {
            return _CallNewObject();
        }

        #region ILoadMember
        /// <summary>
        /// 加载更多的属性或字段
        /// </summary>
        public ILoadMember Load
        {
            get
            {
                return this;
            }
        }
        int _LoadFlag = 0;

        //判断是否已经加载过了
        bool Loaded(int flag)
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
            var bf = BindingFlags.Public | BindingFlags.Instance;
            foreach (var f in _Type.GetFields(bf))
            {
                if (f.Name.Contains("<") == false)
                {
                    Field.Add(new ObjectProperty(f));
                }
            }
            Monitor.Exit(this);
        }

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
            var bf = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var f in _Type.GetFields(bf))
            {
                if (f.Name.Contains("<") == false)
                {
                    Field.Add(new ObjectProperty(f));
                }
            }
            Monitor.Exit(this);
        }

        void ILoadMember.StaticField(bool hasNonPublic)
        {
            if (Loaded(3))
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
            foreach (var f in _Type.GetFields(bf))
            {
                if (f.Name.Contains("<") == false &&
                    Field.ContainsKey(f.Name) == false)
                {
                    Field.Add(new ObjectProperty(f));
                }
            }
            Monitor.Exit(this);
        }

        void ILoadMember.NonPublicProperty()
        {
            if (Loaded(4))
            {
                return;
            }
            var bf = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var p in _Type.GetProperties(bf))
            {
                if (p.GetIndexParameters().Length == 0)
                {
                    Property.Add(new ObjectProperty(p));
                }
            }
            Monitor.Exit(this);
        }

        void ILoadMember.StaticProperty(bool hasNonPublic)
        {
            if (Loaded(5))
            {
                return;
            }
            var bf = BindingFlags.Public | BindingFlags.Static;
            if (hasNonPublic)
            {
                bf |= BindingFlags.NonPublic;
            }
            foreach (var p in _Type.GetProperties(bf))
            {
                if (p.GetIndexParameters().Length == 0)
                {
                    Property.Add(new ObjectProperty(p));
                }
            }
            Monitor.Exit(this);
        }
        #endregion

        #region 静态的
        /// <summary> IL构造一个用于调用对象构造函数的委托
        /// </summary>
        /// <param name="type">获取构造函数的对象</param>
        /// <param name="argTypes">构造函数的参数,默认null</param>
        /// <returns></returns>
        public static LiteracyNewObject CreateNewObject(Type type, Type[] argTypes = null)
        {
            var dm = new DynamicMethod("", Type_Object, Types_Objects, type.IsArray ? typeof(Array):type);

            if (argTypes == null)
            {
                argTypes = Type.EmptyTypes;
            }

            if (type.IsValueType && argTypes == Type.EmptyTypes)
            {
                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldloca_S, il.DeclareLocal(type));
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
                return (LiteracyNewObject)dm.CreateDelegate(typeof(LiteracyNewObject));
            }
            else
            {
                var ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, argTypes, null) ?? type.TypeInitializer;
                if (ctor == null)
                {
                    return null;
                }
                else
                {
                    var il = dm.GetILGenerator();
                    for (int i = 0; i < argTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldc_I4, i);
                        il.Emit(OpCodes.Ldelem_Ref);
                        EmitCast(il, argTypes[i]);
                    }
                    il.Emit(OpCodes.Newobj, ctor);
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Box, type);
                    }
                    il.Emit(OpCodes.Ret);
                    return (LiteracyNewObject)dm.CreateDelegate(typeof(LiteracyNewObject));
                }
            }

        }
        /// <summary> IL构造一个用于获取对象属性值的委托
        /// </summary>
        public static LiteracyGetter CreateGetter(PropertyInfo prop)
        {
            if (prop == null)
            {
                return null;
            }
            var dm = new DynamicMethod("", Type_Object, Types_Object, prop.DeclaringType);
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
                il.Emit(OpCodes.Callvirt, met);
            }
            if (prop.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, prop.PropertyType);
            }
            il.Emit(OpCodes.Ret);
            return (LiteracyGetter)dm.CreateDelegate(typeof(LiteracyGetter));
        }
        /// <summary> IL构造一个用于获取对象字段值的委托
        /// </summary>
        public static LiteracyGetter CreateGetter(FieldInfo field)
        {
            if (field == null)
            {
                return null;
            }
            var dm = new DynamicMethod("", Type_Object, Types_Object, field.DeclaringType);
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
            return (LiteracyGetter)dm.CreateDelegate(typeof(LiteracyGetter));
        }
        /// <summary> IL构造一个用于设置对象属性值的委托
        /// </summary>
        public static LiteracySetter CreateSetter(PropertyInfo prop)
        {
            if (prop == null)
            {
                return null;
            }
            if (prop.DeclaringType.IsValueType)//值类型无法通过方法给其属性或字段赋值
            {
                return null;
            }
            var dm = new DynamicMethod("", null, Types_2Object, prop.DeclaringType);
            var set = prop.GetSetMethod(true);
            if (set == null)
            {
                return null;
            }
            var il = dm.GetILGenerator();

            if (set.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, prop.PropertyType);
                il.Emit(OpCodes.Call, set);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, prop.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, prop.PropertyType);
                il.Emit(OpCodes.Callvirt, set);

            }
            il.Emit(OpCodes.Ret);

            return (LiteracySetter)dm.CreateDelegate(typeof(LiteracySetter));
        }
        /// <summary> IL构造一个用于设置对象字段值的委托
        /// </summary>
        public static LiteracySetter CreateSetter(FieldInfo field)
        {
            if (field == null || field.IsInitOnly)
            {
                return null;
            }
            if (field.DeclaringType.IsValueType)//值类型无法通过方法给其属性或字段赋值
            {
                return null;
            }
            var dm = new DynamicMethod("", null, Types_2Object, field.DeclaringType);
            var il = dm.GetILGenerator();

            if (field.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, field.FieldType);
                il.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                EmitCast(il, field.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                EmitCast(il, field.FieldType);
                il.Emit(OpCodes.Stfld, field);
            }
            il.Emit(OpCodes.Ret);
            return (LiteracySetter)dm.CreateDelegate(typeof(LiteracySetter));
        }
        /// <summary> IL构造一个用于执行方法的委托
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        public static LiteracyCaller CreateCaller(MethodInfo method)
        {
            var dm = new DynamicMethod("", Type_Object, Types_Object_Objects, method.DeclaringType);

            var il = dm.GetILGenerator();

            var isRef = false;

            var ps = method.GetParameters();
            LocalBuilder[] loc = new LocalBuilder[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                Type pt = p.ParameterType;
                if (pt.Name[pt.Name.Length - 1] == '&')//ref,out获取他的实际类型
                {
                    isRef = true;
                    pt = Type.GetType(pt.FullName.Remove(pt.FullName.Length - 1));
                }

                loc[i] = il.DeclareLocal(pt);
                if (p.IsOut)
                {
                    //if (pt.IsValueType)
                    //{
                    //    il.Emit(OpCodes.Initobj, pt);
                    //}
                    //else
                    //{
                    //    il.Emit(OpCodes.Ldnull);
                    //}
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    EmitCast(il, pt);
                    il.Emit(OpCodes.Stloc, loc[i]);//保存到本地变量
                }
            }

            if (method.IsStatic == false)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, method.DeclaringType);
            }
            //将参数加载到参数堆栈
            foreach (var pa in method.GetParameters())
            {
                if (pa.IsOut || pa.ToString().Contains("&"))//out或ref
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

            if (method.IsStatic)
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
                if (ret != null)
                {
                    il.Emit(OpCodes.Starg, ret);
                }
                for (int i = 0; i < loc.Length; i++)
                {
                    var l = loc[i];
                    if (l != null)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldelem_I4, i);
                        il.Emit(OpCodes.Ldloc, l);
                        if (l.LocalType.IsValueType)
                        {
                            il.Emit(OpCodes.Box, l.LocalType);
                        }
                        else
                        {
                            il.Emit(OpCodes.Castclass, Type_Object);
                        }
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                }
                if (ret != null)
                {
                    il.Emit(OpCodes.Ldarg, ret);
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

            return (LiteracyCaller)dm.CreateDelegate(typeof(LiteracyCaller));
        }
        /// <summary> IL类型转换指令
        /// </summary>
        private static void EmitCast(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }
        #endregion
    }
}

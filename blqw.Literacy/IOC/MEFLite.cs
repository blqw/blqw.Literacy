using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace blqw.IOC
{
    /// <summary>
    /// 用于执行MEF相关操作
    /// </summary>
    static class MEFLite
    {
        /// <summary>
        /// 字符串锁
        /// </summary>
        const string _Lock = "O[ON}:z05i$*H75O[bJdnedei#('i_i^v2";


        /// <summary> 
        /// 是否已初始化完成
        /// </summary>
        public static bool IsInitialized { get { return Lazy?.IsValueCreated == true; } }

        /// <summary>
        /// 插件容器
        /// </summary>
        public static CompositionContainer Container { get { return Lazy.Value; } }

        private static readonly Lazy<CompositionContainer> Lazy = GetLazy();

        private static Lazy<CompositionContainer> GetLazy()
        {
            Lazy<CompositionContainer> lazy;
            lock (_Lock)
            {
                lazy = AppDomain.CurrentDomain.GetData(_Lock) as Lazy<CompositionContainer>;
                if (lazy == null)
                {
                    if (Debugger.IsAttached
                        && Debug.Listeners.OfType<ConsoleTraceListener>().Any() == false)
                    {
                        Debug.Listeners.Add(new ConsoleTraceListener(true));
                    }
                    lazy = new Lazy<CompositionContainer>(GetContainer, true);
                    AppDomain.CurrentDomain.SetData(_Lock, lazy);
                }
            }
            try { var x = lazy.Value; } catch { }
            return lazy;
        }

        /// <summary> 
        /// 初始化
        /// </summary>
        [Obsolete("自动初始化,不再需要调用该方法")]
        public static void Initializer()
        {

        }

        private static bool Add(this HashSet<string> loaded, Assembly assembly)
        {
            var key = assembly.ManifestModule.ModuleVersionId + "," + assembly.ManifestModule.MDStreamVersion;
            if (loaded.Contains(key))
            {
                return false;
            }
            loaded.Add(key);
            return true;
        }

        /// <summary> 
        /// 获取插件容器
        /// </summary>
        /// <returns></returns>
        private static CompositionContainer GetContainer()
        {
            var dir = new DirectoryCatalog(".").FullPath;
            var files = new HashSet<string>(
                Directory.EnumerateFiles(dir, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.EnumerateFiles(dir, "*.exe", SearchOption.AllDirectories))
                , StringComparer.OrdinalIgnoreCase);
            var logs = new AggregateCatalog();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var loaded = new HashSet<string>();
            foreach (var a in assemblies)
            {
                if (loaded.Add(a) == false)
                {
                    continue;
                }
                if (a.IsUseful())
                {
                    LoadAssembly(a).ForEach(logs.Catalogs.Add);
                }
                if (a.IsDynamic == false)
                {
                    files.Remove(a.Location);
                }
            }
            var domain = AppDomain.CreateDomain("mef");
            foreach (var file in files)
            {
                var bytes = File.ReadAllBytes(file);
                try
                {
                    var ass = domain.Load(bytes);
                    if (loaded.Add(ass) == false)
                    {
                        continue;
                    }
                    if (ass.IsUseful())
                    {
                        ass = Assembly.Load(bytes);
                        LoadAssembly(ass).ForEach(logs.Catalogs.Add);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex, "MEF部分插件加载失败1");
                }
            }
            AppDomain.Unload(domain);
            return new SelectionPriorityContainer(logs);
        }

        /// <summary>
        /// 过滤系统组件,动态组件,全局缓存组件
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static bool IsUseful(this Assembly assembly)
        {
            return assembly.IsDynamic == false
                && assembly.GlobalAssemblyCache == false
                && assembly.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) == false
                && assembly.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) == false;
        }


        private static List<ComposablePartCatalog> LoadAssembly(Assembly assembly)
        {
            var list = new List<ComposablePartCatalog>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "MEF部分插件加载失败2");
                return list;
            }
            foreach (var type in types)
            {
                try
                {
                    if (type != null)
                    {
                        if (Regex.IsMatch(type.FullName, "[^a-zA-Z_`0-9.+]"))
                            continue;
                        list.Add(new TypeCatalog(type));
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex, "MEF部分插件加载失败3");
                }
            }
            return list;
        }

        /// <summary>
        /// 导入插件
        /// </summary>
        /// <param name="instance"></param>
        public static void Import(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var type = instance as Type;
            if (type != null)
            {
                Import(type, null);
                return;
            }
            try
            {
                Container.ComposeParts(instance);
                return;
            }
            catch (CompositionException ex)
            {
                Trace.WriteLine(ex.ToString(), "MEF组合失败");
            }
            Import(instance.GetType(), instance);
        }

        /// <summary>
        /// 导入插件
        /// </summary>
        /// <param name="type"></param>
        public static void Import(Type type, object instance)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            if (instance == null)
            {
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }

            foreach (var f in type.GetFields(flags))
            {
                if (f.IsLiteral == true)
                {
                    continue;
                }
                var import = GetImportDefinition(f, f.FieldType);
                if (import == null)
                {
                    continue;
                }
                var value = GetExportedValue(import);
                if (value != null)
                    f.SetValue(instance, value);
            }
            var args = new object[1];
            foreach (var p in type.GetProperties(flags))
            {
                var set = p.GetSetMethod(true);
                if (set == null)
                {
                    continue;
                }
                var import = GetImportDefinition(p, p.PropertyType);
                if (import == null)
                {
                    continue;
                }
                var value = GetExportedValue(import);
                if (value != null)
                {
                    args[0] = value;
                    set.Invoke(instance, args);
                }
            }
        }

        class ImportDefinitionImpl : ImportDefinition
        {
            public ImportDefinitionImpl(Expression<Func<ExportDefinition, bool>> constraint, string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, IDictionary<string, object> metadata)
                : base(constraint, contractName, cardinality, isRecomposable, isPrerequisite, metadata)
            {

            }

            /// <summary>
            /// 导入插件的字段或属性的类型
            /// </summary>
            public Type MemberType { get; set; }

            /// <summary>
            /// 导出插件的类型
            /// </summary>
            public Type ExportedType { get; set; }
        }

        /// <summary>
        /// 根据属性或字段极其类型,返回导入插件的描述信息
        /// </summary>
        /// <param name="member">属性或字段</param>
        /// <param name="memberType">属性或字段的类型</param>
        /// <returns></returns>
        private static ImportDefinitionImpl GetImportDefinition(MemberInfo member, Type memberType)
        {
            var import = member.GetCustomAttribute<ImportAttribute>();
            if (import != null)
            {
                return new ImportDefinitionImpl(
                    GetExpression(import.ContractName, import.ContractType, memberType),
                    import.ContractName,
                    ImportCardinality.ZeroOrOne,
                    false,
                    true,
                    null)
                {
                    MemberType = memberType,
                    ExportedType = memberType,
                };
            }
            var importMany = member.GetCustomAttribute<ImportManyAttribute>();
            if (importMany != null)
            {
                //获取实际类型
                var actualType = GetActualType(memberType);
                if (actualType == null)
                {
                    return null;
                }
                return new ImportDefinitionImpl(
                    GetExpression(importMany.ContractName, importMany.ContractType, actualType),
                    importMany.ContractName,
                    ImportCardinality.ZeroOrMore,
                    false,
                    true,
                    null)
                {
                    MemberType = memberType,
                    ExportedType = actualType,
                };
            }
            return null;
        }

        /// <summary>
        /// 获取当前集合类型的实际元素类型
        /// </summary>
        /// <param name="resultType">集合类型</param>
        /// <returns></returns>
        private static Type GetActualType(Type resultType)
        {
            if (resultType == null)
            {
                return null;
            }
            if (resultType.IsArray)
            {
                return resultType.GetElementType();
            }

            Type actualType = null; //实际插件类型
            if (resultType.IsInterface)
            {
                actualType = GetInerfaceElementType(resultType);
            }
            foreach (var @interface in resultType.GetInterfaces())
            {
                var elementType = GetInerfaceElementType(@interface);
                if (elementType == null)
                {
                    continue;
                }
                if (actualType == elementType)
                {
                    continue;
                }
                if (actualType == typeof(object) || actualType == null)
                {
                    actualType = elementType;
                }
                else if (elementType != typeof(object))
                {
                    return null;
                }
            }
            return actualType;
        }

        private static Type GetInerfaceElementType(Type interfaceType)
        {
            Type elementType = null;
            if (interfaceType.IsGenericType)
            {
                var raw = interfaceType.GetGenericTypeDefinition();
                if (raw == typeof(ICollection<>) || raw == typeof(IEnumerable<>))
                {
                    elementType = interfaceType.GetGenericArguments()[0];
                }
            }
            else if (interfaceType == typeof(ICollection)
                || interfaceType == typeof(IEnumerable))
            {
                elementType = typeof(object);
            }
            return elementType;
        }

        /// <summary>
        /// 用于描述 <see cref="IDictionary<string, object>"/> 的 ContainsKey方法
        /// </summary>
        private static readonly MethodInfo _ContainsKey = typeof(IDictionary<string, object>).GetMethod("ContainsKey");

        /// <summary>
        /// 用于描述 <see cref="IDictionary<string, object>"/> 索引器的get方法
        /// </summary>
        private static readonly MethodInfo _getItem = typeof(IDictionary<string, object>).GetProperties().Where(it => it.GetIndexParameters()?.Length > 0).Select(it => it.GetGetMethod()).First();

        /// <summary>
        /// 获取根据插件导入名称约定和类型约束相匹配导出插件的筛选表达式
        /// </summary>
        /// <param name="contractName">约定名称</param>
        /// <param name="contractType">约定类型</param>
        /// <param name="actualType">实际类型</param>
        /// <returns></returns>
        private static Expression<Func<ExportDefinition, bool>> GetExpression(string contractName, Type contractType, Type actualType)
        {
            var p = Expression.Parameter(typeof(ExportDefinition), "p");
            Expression left = null;
            Expression right = null;
            Type validType;
            if (contractName != null)
            {
                var a = Expression.Property(p, "ContractName");
                left = Expression.Equal(a, Expression.Constant(contractName));
                validType = contractType ?? typeof(object);
            }
            else if (contractType == null || contractType == typeof(object))
            {
                validType = actualType;
            }
            else
            {
                validType = contractType;
            }

            if (validType != typeof(object))
            {
                var t = AttributedModelServices.GetTypeIdentity(validType);
                var metadata = Expression.Property(p, "Metadata");
                var typeIdentity = Expression.Constant("TypeIdentity");
                var containsKey = Expression.Call(metadata, _ContainsKey, typeIdentity);

                var getItem = Expression.Call(metadata, _getItem, typeIdentity);

                right = Expression.AndAlso(containsKey, Expression.Equal(getItem, Expression.Constant(t)));
            }

            if (left == null && right == null)
            {
                var @true = Expression.Constant(true);
                return Expression.Lambda<Func<ExportDefinition, bool>>(@true, p);
            }

            if (left == null)
            {
                return Expression.Lambda<Func<ExportDefinition, bool>>(right, p);
            }

            if (right == null)
            {
                return Expression.Lambda<Func<ExportDefinition, bool>>(left, p);
            }

            var c = Expression.AndAlso(left, right);
            return Expression.Lambda<Func<ExportDefinition, bool>>(c, p);
        }

        /// <summary>
        /// 根据导入描述获,和返回类型取导出插件的值
        /// </summary>
        /// <param name="import">导入描述</param>
        /// <returns></returns>
        private static object GetExportedValue(ImportDefinitionImpl import)
        {
            var exports = Container.GetExports(import);

            if (import.Cardinality == ImportCardinality.ZeroOrMore)
            {
                if (import.MemberType.IsArray || import.MemberType.IsInterface)
                {
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(import.ExportedType));
                    foreach (var export in exports)
                    {
                        object value = ConvertExportedValue(export.Value, import.ExportedType);
                        if (value != null)
                        {
                            list.Add(value);
                        }
                    }

                    if (import.MemberType.IsArray)
                    {
                        var array = Array.CreateInstance(import.ExportedType, list.Count);
                        list.CopyTo(array, 0);
                        return array;
                    }
                    return list;
                }
                else
                {
                    dynamic list = Activator.CreateInstance(import.MemberType);
                    foreach (var export in exports)
                    {
                        dynamic value = ConvertExportedValue(export.Value, import.ExportedType);
                        if (value != null)
                        {
                            list.Add(value);
                        }
                    }
                    return list;
                }
            }

            return ConvertExportedValue(exports.FirstOrDefault()?.Value, import.ExportedType);
        }

        private static object ConvertExportedValue(object value, Type exportedType)
        {
            if (value == null)
            {
                return null;
            }
            if (exportedType.IsInstanceOfType(value))
            {
                return value;
            }
            var handler = value as ExportedDelegate;
            if (handler != null && exportedType.IsSubclassOf(typeof(Delegate)))
            {
                return handler.CreateDelegate(exportedType);
            }
            return null;
        }

        class SelectionPriorityContainer : CompositionContainer
        {
            public SelectionPriorityContainer(ComposablePartCatalog catalog)
                : base(catalog)
            {

            }
            protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
            {
                //var exports = base.GetExportsCore(definition, atomicComposition);
                var exports = base.GetExportsCore(
                                new ImportDefinition(
                                    definition.Constraint,
                                    definition.ContractName,
                                    ImportCardinality.ZeroOrMore,
                                    definition.IsRecomposable,
                                    definition.IsPrerequisite,
                                    definition.Metadata
                                ), atomicComposition);

                if (definition.Cardinality == ImportCardinality.ZeroOrMore)
                {
                    return exports;
                }

                //返回优先级最高的一个或者没有
                return exports.OrderByDescending(it =>
                {
                    object priority;
                    if (it.Metadata.TryGetValue("Priority", out priority))
                    {
                        return priority;
                    }
                    return 0;
                }).Take(1).ToArray();
            }

        }

    }
}
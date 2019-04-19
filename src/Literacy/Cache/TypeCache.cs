using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace blqw.Reflection
{
    /// <summary> 
    /// 用于处于Type类型为Key时的缓存
    /// </summary>
    class TypeCache<TValue>
    {
        /// <summary> 
        /// 标准的字典缓存
        /// </summary>
        private readonly ConcurrentDictionary<Type, TValue> _cache = new ConcurrentDictionary<Type, TValue>();

        /// <summary> 
        /// 泛型缓存
        /// </summary>
        class GenericCache<Key>
        {
            public readonly static TValue Item;
            public readonly static bool IsReadied;
        }

        /// <summary> 
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="item">缓存值</param>
        public void Set(Type key, TValue item)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.IsConstructedGenericType)
            {
                var gc = typeof(GenericCache<>).MakeGenericType(typeof(TValue), key);
                var fieid1 = gc.GetField("Item", BindingFlags.Public | BindingFlags.Static);
                var fieid2 = gc.GetField("IsReadied", BindingFlags.Public | BindingFlags.Static);
                fieid1.SetValue(null, item);
                fieid2.SetValue(null, true);
            }
            _cache.AddOrUpdate(key, item, (a, b) => item);
        }

        /// <summary> 
        /// 获取缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        public TValue Get(Type key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.TryGetValue(key, out var item) ? item : (default);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="TKey">缓存键的类型</typeparam>
        public TValue Get<TKey>() => GenericCache<TKey>.Item;

        /// <summary> 
        /// 获取缓存值,如果指定缓存不存在则使用create参数获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="create">用于创建缓存项委托</param>
        public TValue GetOrCreate(Type key, Func<Type, TValue> create)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (create == null)
            {
                throw new ArgumentNullException(nameof(create));
            }

            return _cache.GetOrAdd(key, x =>
            {
                var item = create(x);
                var gc = typeof(GenericCache<>).MakeGenericType(typeof(TValue), x);
                var fieid1 = gc.GetField("Item", BindingFlags.Public | BindingFlags.Static);
                var fieid2 = gc.GetField("IsReadied", BindingFlags.Public | BindingFlags.Static);
                fieid1.SetValue(null, item);
                fieid2.SetValue(null, true);
                return item;
            });
        }

        /// <summary> 
        /// 获取缓存值,如果指定缓存不存在则使用create参数获取缓存
        /// </summary>
        /// <typeparam name="TKey">缓存键的类型</typeparam>
        /// <param name="create">用于创建缓存项委托</param>
        public TValue GetOrCreate<TKey>(Func<Type, TValue> create)
        {
            if (create == null)
            {
                throw new ArgumentNullException(nameof(create));
            }

            if (GenericCache<TKey>.IsReadied)
            {
                return GenericCache<TKey>.Item;
            }
            return GetOrCreate(typeof(TKey), create);
        }
    }
}

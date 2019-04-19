using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace blqw.Reflection
{
    /// <summary> 
    /// 对象属性/字段集合
    /// </summary>
    public class ObjectPropertyCollection : IEnumerable<ObjectProperty>
    {
        /// <summary> 
        /// 属性集合
        /// </summary>
        private readonly Dictionary<string, ObjectProperty> _items;
        /// <summary> 
        /// 对象属性/字段集合
        /// </summary>
        /// <param name="ignoreCase">是否忽略大小写</param>
        public ObjectPropertyCollection(bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
            _items = new Dictionary<string, ObjectProperty>(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        }
        /// <summary> 
        /// 是否忽略大小写
        /// </summary>
        public bool IgnoreCase { get; private set; }

        /// <summary> 
        /// 是否存在指定名称的属性
        /// </summary>
        public bool ContainsKey(string name) => _items.ContainsKey(name);

        /// <summary> 
        /// 属性名集合
        /// </summary>
        public ICollection<string> Names => _items.Keys;

        /// <summary> 
        /// 获取指定名称的属性,如果属性不存在,则返回null
        /// </summary>
        public ObjectProperty this[string name] => _items.TryGetValue(name, out var value) ? value : null;

        /// <summary>
        /// 使用映射名获取字段
        /// </summary>
        /// <param name="mappingName"></param>
        public ObjectProperty Mapping(string mappingName) => _items.TryGetValue("\0" + mappingName, out var value) ? value : null;

        /// <summary> 
        /// 属性个数
        /// </summary>
        public int Count { get; private set; }

        internal void Add(ObjectProperty value)
        {
            var name = value.Name;
            if (_items.TryGetValue(name, out var p))
            {
                if (IgnoreCase && p.Name != name)
                {
                    throw new ArgumentException("属性名称因忽略大小写而重复");
                }
                else
                {
                    throw new ArgumentException("属性不应重复添加");
                }
            }
            else
            {
                _items.Add(name, value);
                if (value.MappingName != null)
                {
                    _items.Add("\0" + value.MappingName, value);
                }
                Count++;
            }
        }

        internal void AddRange  (IEnumerable<PropertyInfo> properties)
        {
            foreach (var p in properties)
            {
                if (p.GetIndexParameters().Length == 0 && !ContainsKey(p.Name)) //排除索引器
                {
                    Add(ObjectProperty.Cache(p));
                }
            }
        }

        /// <summary> 支持在属性或字段集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ObjectProperty> GetEnumerator() => GetEnumerator(null, null);

        /// <summary> 
        /// 支持在属性或字段集合上进行简单迭代。
        /// </summary>
        /// <param name="canwirte">是否可写</param>
        /// <param name="canread">是否可读</param>
        public IEnumerator<ObjectProperty> GetEnumerator(bool? canwirte, bool? canread)
        {
            foreach (var item in _items)
            {
                var value = item.Value;
                if (item.Key[0] == '\0' || value.AutoField)
                {
                    continue; //跳过映射属性 和 自动属性生成的字段
                }
                if (canwirte != null && canwirte.Value != value.CanWrite)
                {
                    continue;
                }
                if (canread != null && canread.Value != value.CanWrite)
                {
                    continue;
                }
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => _items.Values.GetEnumerator();
    }
}

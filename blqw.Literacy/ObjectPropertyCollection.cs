using System;
using System.Collections;
using System.Collections.Generic;

namespace blqw
{
    /// <summary> 对象属性/字段集合
    /// </summary>
    public class ObjectPropertyCollection : IEnumerable<ObjectProperty>
    {
        /// <summary> 属性集合
        /// </summary>
        private readonly Dictionary<string, ObjectProperty> _Items;
        /// <summary> 对象属性/字段集合
        /// </summary>
        /// <param name="ignoreCase">是否忽略大小写</param>
        public ObjectPropertyCollection(bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
            if (ignoreCase)
            {
                _Items = new Dictionary<string, ObjectProperty>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _Items = new Dictionary<string, ObjectProperty>();
            }
        }
        /// <summary> 是否忽略大小写
        /// </summary>
        public bool IgnoreCase { get; private set; }

        /// <summary> 是否存在指定名称的属性
        /// </summary>
        public bool ContainsKey(string name)
        {
            return _Items.ContainsKey(name);
        }
        /// <summary> 属性名集合
        /// </summary>
        public ICollection<string> Names
        {
            get { return _Items.Keys; }
        }
        /// <summary> 获取指定名称的属性,如果属性不存在,则返回null
        /// </summary>
        public ObjectProperty this[string name]
        {
            get
            {
                ObjectProperty value;
                if (_Items.TryGetValue(name, out value))
                {
                    return value;
                }
                return null;
            }
        }

        /// <summary> 使用映射名获取字段
        /// </summary>
        /// <param name="mappingName"></param>
        public ObjectProperty Mapping(string mappingName)
        {
            ObjectProperty value;
            if (_Items.TryGetValue("\0" + mappingName, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary> 属性个数
        /// </summary>
        public int Count { get; private set; }

        internal void Add(ObjectProperty value)
        {
            var name = value.Name;
            ObjectProperty p;
            if (_Items.TryGetValue(name, out p))
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
                _Items.Add(name, value);
                if (value.MappingName != null)
                {
                    _Items.Add("\0" + value.MappingName, value);
                }
                Count++;
            }
        }

        /// <summary> 支持在属性或字段集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ObjectProperty> GetEnumerator()
        {
            foreach (var item in _Items)
            {
                if (item.Key[0] == '\0') continue; //跳过映射属性
                var value = item.Value;
                if (value.AutoField == false)
                {
                    yield return value;
                }
            }
        }
        /// <summary> 支持在属性或字段集合上进行简单迭代。
        /// </summary>
        /// <param name="canwirte">是否可写</param>
        /// <param name="canread">是否可读</param>
        public IEnumerator<ObjectProperty> GetEnumerator(bool? canwirte, bool? canread)
        {
            if (canwirte == null && canread == null)
            {
                foreach (var item in _Items)
                {
                    if (item.Key[0] == '\0') continue;
                    var value = item.Value;
                    if (value.AutoField == false)
                    {
                        yield return value;
                    }
                }
            }
            else if (canwirte == null)
            {
                var b = canread.Value;
                foreach (var item in _Items)
                {
                    if (item.Key[0] == '\0') continue;
                    var value = item.Value;
                    if (value.CanRead == b && value.AutoField == false)
                    {
                        yield return value;
                    }
                }
            }
            else if (canread == null)
            {
                var b = canwirte.Value;
                foreach (var item in _Items)
                {
                    if (item.Key[0] == '\0') continue;
                    var value = item.Value;
                    if (value.CanWrite == b && value.AutoField == false)
                    {
                        yield return value;
                    }
                }
            }
            else
            {
                var a = canread.Value;
                var b = canwirte.Value;
                foreach (var item in _Items)
                {
                    if (item.Key[0] == '\0') continue;
                    var value = item.Value;
                    if (value.CanWrite == b && value.CanRead == a && value.AutoField == false)
                    {
                        yield return value;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.Values.GetEnumerator();
        }
    }
}

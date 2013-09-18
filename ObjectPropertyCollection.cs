using System;
using System.Collections.Generic;

namespace blqw
{
    /// <summary> 对象属性/字段集合
    /// </summary>
    public class ObjectPropertyCollection : IEnumerable<ObjectProperty>
    {
        /// <summary> 
        /// </summary>
        Dictionary<string, ObjectProperty> Items;
        /// <summary> 对象属性/字段集合
        /// </summary>
        /// <param name="ignoreCase">是否忽略大小写</param>
        public ObjectPropertyCollection(bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
            if (ignoreCase)
            {
                Items = new Dictionary<string, ObjectProperty>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                Items = new Dictionary<string, ObjectProperty>();
            }
        }
        /// <summary> 是否忽略大小写
        /// </summary>
        public bool IgnoreCase { get; private set; }

        /// <summary> 是否存在指定名称的属性
        /// </summary>
        public bool ContainsKey(string name)
        {
            return Items.ContainsKey(name);
        }
        /// <summary> 属性名集合
        /// </summary>
        public ICollection<string> Names
        {
            get { return Items.Keys; }
        }
        /// <summary> 获取指定名称的属性,如果属性不存在,则返回null
        /// </summary>
        public ObjectProperty this[string name]
        {
            get
            {
                ObjectProperty value;
                if (Items.TryGetValue(name, out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary> 属性个数
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        internal void Add(ObjectProperty value)
        {
            if (IgnoreCase)
            {
                if (Items.ContainsKey(value.Name))
                {
                    if (Items[value.Name].Name != value.Name)
                    {
                        throw new Exception("在忽略大小写模式下出现仅大小写不同的重复名称");
                    }
                }
            }
            Items.Add(value.Name, value);
            //Items[value.Name] = value;
        }

        internal bool Remove(string name)
        {
            return Items.Remove(name);
        }

        /// <summary> 支持在属性或字段集合上进行简单迭代。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ObjectProperty> GetEnumerator()
        {
            foreach (var item in Items.Values)
            {
                yield return item;
            }
        }
        /// <summary> 支持在属性或字段集合上进行简单迭代。
        /// </summary>
        /// <param name="canwirte">是否可写</param>
        /// <param name="canread">是否可读</param>
        /// <returns></returns>
        public IEnumerator<ObjectProperty> GetEnumerator(bool? canwirte, bool? canread)
        {
            if (canwirte == null && canread == null)
            {
                foreach (var item in Items.Values)
                {
                    yield return item;
                }
            }
            else if (canwirte == null)
            {
                var b = canread.Value;
                foreach (var item in Items.Values)
                {
                    if (item.CanRead == b)
                    {
                        yield return item;
                    }
                }
            }
            else if (canread == null)
            {
                var b = canwirte.Value;
                foreach (var item in Items.Values)
                {
                    if (item.CanWrite == b)
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                var a = canread.Value;
                var b = canwirte.Value;
                foreach (var item in Items.Values)
                {
                    if (item.CanWrite == b && item.CanRead == a)
                    {
                        yield return item;
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }
    }
}

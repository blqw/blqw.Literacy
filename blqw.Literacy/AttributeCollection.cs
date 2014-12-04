using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary> 特性集合,用于方便快速的操作特性
    /// </summary>
    public class AttributeCollection
    {
        private Attribute[] _attributes;
        private int _count;

        /// <summary> 根据类型对象, 初始化特性集合
        /// </summary>
        public AttributeCollection(Type type)
        {
            _attributes = Attribute.GetCustomAttributes(type);
            _count = _attributes.Length;
        }

        /// <summary> 根据成员对象, 初始化特性集合
        /// </summary>
        /// <param name="member"></param>
        public AttributeCollection(MemberInfo member)
        {
            _attributes = Attribute.GetCustomAttributes(member);
            _count = _attributes.Length;
        }

        /// <summary> 当前对象特性总数
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary> 获取所有特性
        /// </summary>
        public Attribute[] GetAll()
        {
            return _attributes;
        }

        #region First

        /// <summary> 获取第一个指定类型的特性,没有找到返回null
        /// </summary>
        public Attribute First(Type attrType)
        {
            if (attrType == null)
            {
                throw new ArgumentNullException("attrType");
            }
            else if (!typeof(Attribute).IsAssignableFrom(attrType))
            {
                throw new ArgumentOutOfRangeException("attrType", "attrType必须是Attribute的子类型");
            }
            for (int i = 0; i < _count; i++)
            {
                if (attrType.IsInstanceOfType(_attributes[i]))
                {
                    return _attributes[i];
                }
            }
            return null;
        }

        /// <summary> 获取第一个指定类型的特性,没有找到返回null
        /// </summary>
        public T First<T>()
            where T : class
        {
            for (int i = 0; i < _count; i++)
            {
                var attr = _attributes[i] as T;
                if (attr != null)
                {
                    return attr;
                }
            }
            return null;
        }

        /// <summary> 获取第一个指定类型且符合条件的特性,,没有找到返回null
        /// </summary>
        public T First<T>(Converter<T, bool> func)
            where T : class
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            for (int i = 0; i < _count; i++)
            {
                var attr = _attributes[i] as T;
                if (attr != null && func(attr))
                {
                    return attr;
                }
            }
            return null;
        }

        #endregion

        #region Exists

        /// <summary> 判断是否存在指定类型的特性
        /// </summary>
        public bool Exists(Type attrType)
        {
            if (attrType == null)
            {
                throw new ArgumentNullException("attrType");
            }
            else if (!typeof(Attribute).IsAssignableFrom(attrType))
            {
                throw new ArgumentOutOfRangeException("attrType", "attrType必须是Attribute的子类型");
            }

            for (int i = 0; i < _count; i++)
            {
                if (attrType.IsInstanceOfType(_attributes[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> 判断是否存在指定类型的特性
        /// </summary>
        public bool Exists<T>()
            where T : Attribute
        {
            for (int i = 0; i < _count; i++)
            {
                if (_attributes[i] is T)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> 判断是否存在指定类型且符合条件的特性
        /// </summary>
        public bool Exists<T>(Converter<T, bool> func)
            where T : Attribute
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            for (int i = 0; i < _count; i++)
            {
                var attr = _attributes[i] as T;
                if (attr != null && func(attr))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Where

        /// <summary> 返回所有指定类型的特性的枚举迭代
        /// </summary>
        public IEnumerable<Attribute> Where(Type attrType)
        {
            if (attrType == null)
            {
                throw new ArgumentNullException("attrType");
            }
            else if (!typeof(Attribute).IsAssignableFrom(attrType))
            {
                throw new ArgumentOutOfRangeException("attrType", "attrType必须是Attribute的子类型");
            }

            for (int i = 0; i < _count; i++)
            {
                if (attrType.IsInstanceOfType(_attributes[i]))
                {
                    yield return _attributes[i];
                }
            }
        }

        /// <summary> 返回所有指定类型的特性的枚举迭代
        /// </summary>
        public IEnumerable<T> Where<T>()
            where T : Attribute
        {
            for (int i = 0; i < _count; i++)
            {
                var attr = _attributes[i] as T;
                if (attr != null)
                {
                    yield return attr;
                }
            }
        }

        /// <summary> 返回所有指定类型且符合条件的特性的枚举迭代
        /// </summary>
        public IEnumerable<T> Where<T>(Converter<T, bool> func)
            where T : Attribute
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            for (int i = 0; i < _count; i++)
            {
                var attr = _attributes[i] as T;
                if (attr != null && func(attr))
                {
                    yield return attr;
                }
            }
        }

        #endregion
    }
}

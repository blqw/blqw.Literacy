using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Reflection
{
    /// <summary> 控制对象成员名称的映射关系
    /// </summary>
    public interface IMemberMappingAttribute
    {
        /// <summary> 映射名
        /// </summary>
        string Name { get; }
    }
}

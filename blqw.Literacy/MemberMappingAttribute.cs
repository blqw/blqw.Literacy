using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary> 对象成员映射关系特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class MemberMappingAttribute : Attribute, IMemberMappingAttribute
    {
        public MemberMappingAttribute()
        {

        }
        public MemberMappingAttribute(string name)
        {
            Name = name;
        }
        /// <summary> 映射名称
        /// </summary>
        public string Name { get; set; }
    }
}

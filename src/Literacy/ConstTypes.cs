using System;

namespace blqw
{
    class ConstTypes
    {
        /// <summary> 
        /// [ typeof(Object) ]
        /// </summary>
        public static readonly Type[] Object = { typeof(object) };

        /// <summary> 
        /// [ typeof(Object),typeof(Object) ]
        /// </summary>
        public static readonly Type[] ObjectObject = { typeof(object), typeof(object) };

        /// <summary> 
        /// [ typeof(object[]) ]
        /// </summary>
        public static readonly Type[] Objects = { typeof(object[]) };

        /// <summary> 
        /// [ typeof(Object), typeof(object[])  ]
        /// </summary>
        public static readonly Type[] ObjectObjects = { typeof(object), typeof(object[]) };
    }
}

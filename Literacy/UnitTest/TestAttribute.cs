using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class TestAttribute : Attribute
    {
        public int ID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
    public class SingleModel
    {
        public string Name { get; set; }
    }
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Sex { get; set; }
    }
}

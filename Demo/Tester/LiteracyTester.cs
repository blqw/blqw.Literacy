using blqw.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class LiteracyTester : TesterBase
    {
        LiteracyGetter get = null;
        LiteracySetter set = null;

        protected override void Initialize()
        {
            var p = State.GetType().GetProperty("Name");
            get = Literacy.CreateGetter(p);
            set = Literacy.CreateSetter(p);
        }

        protected override void Testing()
        {
            var s = get(State);
            set(State, s);
        }
    }
}

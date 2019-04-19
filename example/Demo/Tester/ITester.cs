using blqw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public abstract class TesterBase
    {
        protected abstract void Initialize();

        protected abstract void Testing();

        public int TestCount { get; set; }

        public object State { get; set; }

        public virtual string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public void Start()
        {
            Console.WriteLine($" 准备测试[{Name}]");
            var watch = new Stopwatch();
            watch.Start();
            Initialize();
            watch.Stop();
            Console.WriteLine($"初始化用时:{watch.Elapsed.TotalMilliseconds} ms");
            CodeTimer.Initialize();
            CodeTimer.Time(Name, TestCount, Testing);
            Console.WriteLine();
        }
    }
}

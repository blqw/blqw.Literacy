using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace blqw
{
    /// <summary> 代码性能测试类,改编自'老赵'的同名类
    /// </summary>
    public static class CodeTimer
    {
        private static bool _initialize = false;
        public static void Initialize()
        {
            if (_initialize == false)
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                Time("", 1, () => { });
                _initialize = true;
            }
        }

        public static void Time(string name, int iteration, ThreadStart action)
        {
            if (String.IsNullOrEmpty(name)) return;

            // 1.
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 2.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            // 3.
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCycleCount();
            for (int i = 0; i < iteration; i++) action();
            ulong cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            string format = "    {0}\t  :  {1}";
            // 4.
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine(" 运行时间    CPU时钟周期    垃圾回收( 1代      2代      3代 )");
            format = " {0,-12}{1,-25}{2,-9}{3,-9}{4}";
            object[] args = new object[6];
            args[0] = watch.ElapsedMilliseconds.ToString("N0") + "ms";
            args[1] = cpuCycles.ToString("N0");
            if (GC.MaxGeneration >= 0) args[2] = GC.CollectionCount(0) - gcCounts[0];
            if (GC.MaxGeneration >= 1) args[3] = GC.CollectionCount(1) - gcCounts[1];
            if (GC.MaxGeneration >= 2) args[4] = GC.CollectionCount(2) - gcCounts[2];
            Console.WriteLine(format, args);
            Console.WriteLine();
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }
    }
}

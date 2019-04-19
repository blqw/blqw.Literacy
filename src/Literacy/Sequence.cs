namespace blqw
{
    static class Sequence
    {
        private static int _number = 0;
        public static int Current => _number;
        public static int Next() => System.Threading.Interlocked.Increment(ref _number);
    }
}

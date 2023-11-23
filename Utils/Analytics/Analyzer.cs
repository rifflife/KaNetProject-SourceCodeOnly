using System;
using System.Diagnostics;

namespace Utils.Analytics
{
    public static class Analyzer
    {
        public const int DEFAULT_REPEAT_TIME = 1000;

        /// <summary>해당 함수를 반복한 뒤 경과 시간을 반환합니다.</summary>
        public static long GetFunctionElapsed(Action function, int repeatTime = DEFAULT_REPEAT_TIME)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            stopwatch.Start();

            for (int i = 0; i < repeatTime; ++i)
            {
                function();
            }

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public static long GetFunctionElapsed(FunctionInfo func, int repeatTime = DEFAULT_REPEAT_TIME)
            => GetFunctionElapsed(func.Function, repeatTime);

        public static void Measure(IMeasurable measurable) => measurable.Measure();
    }
}

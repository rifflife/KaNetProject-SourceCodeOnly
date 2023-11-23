using System.Collections.Generic;
using Utils;

namespace Utils.Analytics
{
    public class FunctionMeasurer : IMeasurable
    {
        /// <summary>측정 타이틀입니다.</summary>
        public string TitleName { get; set; }
        /// <summary>함수의 반복 횟수입니다.</summary>
        public int RepeatTime { get; set; }

        private List<FunctionInfo> mFunctionSetList = new();

        public FunctionMeasurer(string titleName, int repeatTime = Analyzer.DEFAULT_REPEAT_TIME)
        {
            TitleName = titleName;
            RepeatTime = repeatTime;
        }

        public FunctionMeasurer(string titleName, FunctionInfo functionSet, int repeatTime = Analyzer.DEFAULT_REPEAT_TIME)
        {
            TitleName = titleName;
            mFunctionSetList.Add(functionSet);
            RepeatTime = repeatTime;
        }

        public FunctionMeasurer(string titleName, FunctionInfo first, FunctionInfo second, int repeatTime = Analyzer.DEFAULT_REPEAT_TIME)
        {
            TitleName = titleName;
            mFunctionSetList.Add(first);
            mFunctionSetList.Add(second);
            RepeatTime = repeatTime;
        }

        public void Add(FunctionInfo functionSet) => mFunctionSetList.Add(functionSet);

        public void Measure()
        {
            Ulog.Log(UlogType.Tester, $"\nStart measuring \"{TitleName}\"");
            Ulog.Log(UlogType.Tester, $"Repeat time : {RepeatTime}\n");

            foreach (var function in mFunctionSetList)
            {
                long elapsed = Analyzer.GetFunctionElapsed(function, RepeatTime);
                Ulog.Log(UlogType.Tester, $"[{function}] Elapsed : {elapsed}");
            }
        }
    }
}

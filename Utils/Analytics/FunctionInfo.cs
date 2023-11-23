using System;

namespace Utils.Analytics
{
    /// <summary>FunctionInfo 함수의 정보 집합입니다.</summary>
    public class FunctionInfo
    {
        public string Name { get; private set; } = "";
        public Action Function = null;

        public FunctionInfo(string name, Action function)
        {
            Name = name;
            Function = function;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.CodeGen.Core
{
	[Serializable]
	public class CodeGenOption
	{
		public List<MatchRoleGroup> MatchRoleGroupList = new();

		public CodeGenOption()
		{

		}

		public CodeGenOption(List<MatchRole> defaultMathRoles)
		{
			MatchRoleGroupList = new List<MatchRoleGroup>();
			MatchRoleGroupList.Add(new MatchRoleGroup(defaultMathRoles));
		}

		public string ApplyMatchRole(string sourceCode)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var group in MatchRoleGroupList)
			{
				sb.AppendLine(group.ApplyMatchRole(sourceCode));
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}
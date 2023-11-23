using System;
using System.Collections.Generic;

namespace Utils.CodeGen.Core
{
	[Serializable]
	public struct MatchRole
	{
		public string Pattern;
		public string Replace;

		public MatchRole(string pattern, string replace)
		{
			Pattern = pattern;
			Replace = replace;
		}

		public string ApplyMatchRole(string source)
		{
			return source.Replace(Pattern, Replace);
		}
	}

	[Serializable]
	public class MatchRoleGroup
	{
		public List<MatchRole> MatchRoleList = new();

		public MatchRoleGroup()
		{

		}

		public MatchRoleGroup(IList<MatchRole> matchRole)
		{
			MatchRoleList = new List<MatchRole>(matchRole);
		}

		public string ApplyMatchRole(string source)
		{
			string result = source;

			foreach (var role in MatchRoleList)
			{
				result = role.ApplyMatchRole(result);
			}

			return result;
		}
	}
}
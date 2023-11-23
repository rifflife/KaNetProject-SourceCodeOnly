using System.Collections.Generic;
using Utils.CodeGen.Core;

namespace Utils.CodeGen
{
	public class NetworkClassTypesCodeGenerator : CodeGenerator
	{
		public NetworkClassTypesCodeGenerator()
		{
			Title = "Synchronized value class";

			// Initialize Declarations
			mStartDeclaration =
				"// 이 코드는 자동생성된 코드입니다. 수정하지 마세요.\n" +
				"\n" +
				"using System;\n" +
				"using System.Text;\n" +
				"using KaNet.Utils;\n" +
				"\n" +
				"namespace KaNet.Synchronizers\n" +
				"{\n";

			mEndDeclaration =
				"}";

			// Target Code Path
			CodeFilePath = $@"\KaNet\Synchronizers\NetworkTypes\NetworkClassTypes.cs";

			// Option Pathy
			CodeTemplatePath  = $@"\NetworkClassTypesTemplate.txt";
			OptionFilePath  = $@"\NetworkClassTypesOption.txt";
		}

		public override void SetCodeGenOptionByDefault()
		{
			List<MatchRole> defaultRule = new List<MatchRole>();

			defaultRule.Add(new MatchRole("<data_type>", "int"));
			defaultRule.Add(new MatchRole("<represent>", "Int32"));
			defaultRule.Add(new MatchRole("<memory_size>", "4"));
			defaultRule.Add(new MatchRole("<default_value>", "0"));

			mCodeGenOption = new CodeGenOption(defaultRule);
		}
	}
}
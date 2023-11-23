using System.Text;

namespace Utils.CodeGen.Core
{
	public abstract class CodeGenerator
	{
		public string Title { get; protected set; } = "";

		protected CodeGenOption mCodeGenOption;
		protected string mStartDeclaration;
		protected string mEndDeclaration;
		protected string mCodeTemplate;

		// Target Code Path
		public string CodeFilePath { get; protected set; }

		// Option Pathy
		public string CodeTemplatePath { get; protected set; }
		public string OptionFilePath { get; protected set; }

		public void SetCodeTemplate(string template)
		{
			mCodeTemplate = template;
		}

		public void SetCodeGenOption(CodeGenOption codeGenOption)
		{
			mCodeGenOption = codeGenOption;
		}

		public bool TryGetCodeGenOption(out CodeGenOption codeGenOption)
		{
			codeGenOption = mCodeGenOption;
			return codeGenOption != null;
		}

		public abstract void SetCodeGenOptionByDefault();

		public bool TryGenerate(out string generatedCode)
		{
			if (mCodeGenOption == null)
			{
				generatedCode = null;
				return false;
			}

			StringBuilder sb = new StringBuilder();

			sb.AppendLine(mStartDeclaration);
			sb.AppendLine(mCodeGenOption.ApplyMatchRole(mCodeTemplate));
			sb.AppendLine(mEndDeclaration);

			generatedCode = sb.ToString();
			return true;
		}
	}
}
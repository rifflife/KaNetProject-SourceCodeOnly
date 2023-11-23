using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///
/// 코드 들여쓰기 수정 금지 
///

namespace Utils
{
	public enum TextType
	{
		None = 0,
		Text_Title_GameStart,
	}

	/// <summary>언어 타입입니다.</summary>
	public enum LanguageType
	{
		// Type Name			//		SteamAPI Name	//SteamWeb Code
		None,
		Arabic,                 //		arabic			ar
		Bulgarian,              //		bulgarian		bg
		Chinese_Simplified,		//		schinese		zh-CN
		Chinese_Traditional,    //		tchinese		zh-TW
		Czech,					//		czech			cs
		Danish,					//		danish			da
		Dutch,					//		dutch			nl
		English,				//		english			en
		Finnish,				//		finnish			fi
		French,					//		french			fr
		German,					//		german			de
		Greek,					//		greek			el
		Hungarian,				//		hungarian		hu
		Italian,				//		italian			it
		Japanese,				//		japanese		ja
		Korean,					//		koreana			ko
		Norwegian,				//		norwegian		no
		Polish,					//		polish			pl
		Portuguese,				//		portuguese		pt
		Portuguese_Brazil,		//		brazilian		pt-BR
		Romanian,				//		romanian		ro
		Russian,				//		russian			ru
		Spanish_Spain,			//		spanish			es
		Spanish_Latin_America,	//		latam			es-419
		Swedish,                //		swedish			sv
		Thai,					//		thai			th
		Turkish,				//		turkish			tr
		Ukrainian,				//		ukrainian		uk
		Vietnamese,				//		vietnamese		vn
	}

	public static partial class Localization
	{
		private static readonly string DefaultText = "TEXT ERROR";
		
		private static readonly Dictionary<TextType, string> mTextTable = new Dictionary<TextType, string>()
		{
			{ TextType.None, "TEXT_TYPE_ERROR"},
			{ TextType.Text_Title_GameStart, "게임 스타또"},
		};

		private static readonly BidirectionalMap<LanguageType, string> mLanguageCodeTable = new BidirectionalMap<LanguageType, string>()
		{
			{ LanguageType.None,					"ERROR"		},		//	Error
			{ LanguageType.Arabic,					"arabic"	},		//	ar
			{ LanguageType.Bulgarian,				"bulgarian"	},		//	bg
			{ LanguageType.Chinese_Simplified,		"schinese"	},		//	zh-CN
			{ LanguageType.Chinese_Traditional,		"tchinese"	},		//	zh-TW
			{ LanguageType.Czech,					"czech"		},		//	cs
			{ LanguageType.Danish,					"danish"	},		//	da
			{ LanguageType.Dutch,					"dutch"		},		//	nl
			{ LanguageType.English,					"english"	},		//	en
			{ LanguageType.Finnish,					"finnish"	},		//	fi
			{ LanguageType.French,					"french"	},		//	fr
			{ LanguageType.German,					"german"	},		//	de
			{ LanguageType.Greek,					"greek"		},		//	el
			{ LanguageType.Hungarian,				"hungarian"	},		//	hu
			{ LanguageType.Italian,					"italian"	},		//	it
			{ LanguageType.Japanese,				"japanese"	},		//	ja
			{ LanguageType.Korean,					"koreana"	},		//	ko
			{ LanguageType.Norwegian,				"norwegian"	},		//	no
			{ LanguageType.Polish,					"polish"	},		//	pl
			{ LanguageType.Portuguese,				"portuguese"},		//	pt
			{ LanguageType.Portuguese_Brazil,		"brazilian"	},		//	pt-BR
			{ LanguageType.Romanian,				"romanian"	},		//	ro
			{ LanguageType.Russian,					"russian"	},		//	ru
			{ LanguageType.Spanish_Spain,			"spanish"	},		//	es
			{ LanguageType.Spanish_Latin_America,	"latam"		},		//	es-419
			{ LanguageType.Swedish,					"swedish"	},		//	sv
			{ LanguageType.Thai,					"thai"		},		//	th
			{ LanguageType.Turkish,					"turkish"	},		//	tr
			{ LanguageType.Ukrainian,				"ukrainian"	},		//	uk
			{ LanguageType.Vietnamese,				"vietnamese"},		//	vn
		};
		
		/// <summary>언어가 변경되면 호출됩니다.</summary>
		public static event Action OnLanguageChanged;

		private static LanguageType mCurrentLanguage;

		/// <summary>현재 설정된 언어입니다.</summary>
		public static LanguageType CurrentLanguage
		{
			get => mCurrentLanguage;
			private set
			{
				mCurrentLanguage = value;
				OnLanguageChanged?.Invoke();
			}
		}

		/// <summary>스팀 API가 제공하는 언어 코드로 언어를 설정합니다.</summary>
		/// <param name="steamworksApiLanguageCode">스팀 API의 언어 코드 문자열</param>
		public static void SetLanguage(string steamworksApiLanguageCode)
		{
			CurrentLanguage = mLanguageCodeTable.TryGetValue(steamworksApiLanguageCode, out var code) ? code : LanguageType.English;
		}

		/// <summary>언어 타입으로 언어를 설정합니다.</summary>
		/// <param name="languageType">언어 타입</param>
		public static void SetLanguage(LanguageType languageType)
		{
			CurrentLanguage = (languageType == LanguageType.None) ? LanguageType.English : languageType;
		}
	}
}


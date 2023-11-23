using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Utils.ViewModel;

public class LocalizationText
{
	private LocalizeStringEvent mStringEvent;

	//���� ����ϴ� �༮�� ������ �ִ°� ���� �� ����.
	private string mReference = "LocalizationTable";

	private TextMeshProTextViewModel mTextViewModel;


	public LocalizationText(TextMeshProTextViewModel textViewModel, string entrykey)
	{
		mTextViewModel = textViewModel;
		mStringEvent = textViewModel.GetViewGameObject().AddComponent<LocalizeStringEvent>();
		mStringEvent.StringReference = new LocalizedString
		{
			TableReference = mReference,
			TableEntryReference = entrykey
		};

		mStringEvent.OnUpdateString.AddListener(onStringChange);
		mStringEvent.RefreshString();
	}

	public LocalizationText(TextMeshProTextViewModel textViewModel, string entrykey, Dictionary<string, object> arguments)
	{
		mTextViewModel = textViewModel;
		mStringEvent = textViewModel.GetViewGameObject().AddComponent<LocalizeStringEvent>();
		mStringEvent.StringReference = new LocalizedString
		{
			TableReference = mReference,
			TableEntryReference = entrykey,
			Arguments = new object[] { arguments }
		};

		mStringEvent.OnUpdateString.AddListener(onStringChange);
		mStringEvent.RefreshString();
	}

	private void onStringChange(string text)
	{
		mTextViewModel.Text = text;
	}

}



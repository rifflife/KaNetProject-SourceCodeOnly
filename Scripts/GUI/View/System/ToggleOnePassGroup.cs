using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.ViewModel;

/// <summary>
/// �ش� �׷쿡 ���� ��� �߿� ������ �ϳ��� ���� �� �� �ֽ��ϴ�.
/// </summary>
[Serializable]
public class ToggleOnePassGroup
{
	private List<ToggleViewModel> mGroupList = new List<ToggleViewModel>();

	public void Add(ToggleViewModel toggle)
	{
		toggle.AddAction((onValueChange) =>
		{
			if (!onValueChange)
				return;

			foreach (var item in mGroupList)
			{
				if (!item.Equals(toggle))
				{
					item.IsOn = false;
				}
			}
		});

		mGroupList.Add(toggle);
	}

	/// <summary>�׷� ���� �ִ� ����� ��� false �մϴ�.</summary>
	public void Reset()
	{
		foreach (var item in mGroupList)
		{
			item.IsOn = false;
		}
	}

}

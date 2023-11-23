using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Utils.ViewModel
{
	[Serializable]
	public abstract class ViewModel
	{
		[field : SerializeField] public string ViewModelName { get; protected set; }
		public bool IsAvailable { get; protected set; } = false;

		public ViewModel()
		{
			ViewModelName = "";
		}

		public ViewModel(string viewModelName)
		{
			ViewModelName = viewModelName;
		}

		public abstract void Initialize(MonoBehaviour mono);

		public abstract GameObject GetViewGameObject();
	}
}

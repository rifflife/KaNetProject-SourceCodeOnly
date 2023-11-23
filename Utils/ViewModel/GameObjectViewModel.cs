using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.ViewModel
{
	[Serializable]
	public class GameObjectViewModel : ViewModel
	{
		public GameObject GameObject { get; private set; }

		public GameObjectViewModel() : base() {}

		public GameObjectViewModel(string viewModelName) : base(viewModelName) {}

		public override void Initialize(MonoBehaviour mono)
		{
			var findTransform = mono.GetComponentsInChildren<Transform>(true);

			bool isFinded = false;

			foreach (var t in findTransform)
			{
				if (t.gameObject.name == ViewModelName)
				{
					GameObject = t.gameObject;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{GameObject.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			return;
		}

		public override GameObject GetViewGameObject()
		{
			return GameObject.gameObject;
		}
	}
}

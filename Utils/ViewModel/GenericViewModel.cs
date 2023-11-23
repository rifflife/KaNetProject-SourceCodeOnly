using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Utils.ViewModel
{
	[Serializable]
	public class GenericViewModel<T> : ViewModel where T : Component
	{
		public T Model { get; private set; }

		public GenericViewModel()
		{
			ViewModelName = "";
		}

		public GenericViewModel(string viewModelName) : base(viewModelName) { }

		public override void Initialize(MonoBehaviour mono)
		{
			var findTransform = mono.GetComponentsInChildren<T>(true);

			bool isFinded = false;

			foreach (var t in findTransform)
			{
				if (t.gameObject.name == ViewModelName)
				{
					Model = t;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{Model.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			return;
		}

		public override GameObject GetViewGameObject()
		{
			return Model.gameObject;
		}
	}
}

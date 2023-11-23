using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.ViewModel
{
	[Serializable]
	public class TransformViewModel : ViewModel
	{
		public Transform Transform { get; private set; }

		public TransformViewModel() : base() {}

		public TransformViewModel(string viewModelName) : base(viewModelName) {}

		public override void Initialize(MonoBehaviour mono)
		{
			var findTransform = mono.GetComponentsInChildren<Transform>(true);

			bool isFinded = false;

			foreach (var t in findTransform)
			{
				if (t.gameObject.name == ViewModelName)
				{
					Transform = t;
					isFinded = true;
					break;
				}
			}

			if (!isFinded)
			{
				Ulog.Log(UlogType.UI, $"Mono Initialize failed! There is no \"{Transform.GetType().Name}\" component in \"{ViewModelName}\"!");
				return;
			}

			IsAvailable = true;

			return;
		}

		public override GameObject GetViewGameObject()
		{
			return Transform.gameObject;
		}
	}
}

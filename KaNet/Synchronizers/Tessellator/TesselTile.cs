using System.Collections.Generic;
using KaNet.Utils;
using UnityEngine;

namespace KaNet.Synchronizers
{
	public class TesselTile
	{
		public TessellateCoord TesselCoord { get; private set; }

		public List<NetworkObject> ObjectList { get; private set; } = new();

		public TesselTile(TessellateCoord tesselCoord)
		{
			TesselCoord = tesselCoord;
		}

		public bool TryAddObject(NetworkObject networkObject)
		{
			if (ObjectList.Contains(networkObject))
			{
				return false;
			}

			ObjectList.Add(networkObject);

			return true;
		}

		public void RemoveObject(NetworkObject networkObject)
		{
			ObjectList.Remove(networkObject);
		}
	}
}

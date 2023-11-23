using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.KaNet.Session
{
	public readonly struct UserID
	{
		public readonly byte ID;

		public UserID(byte id)
		{
			ID = id;
		}

		public UserID(int id)
		{
			ID = (byte)id;
		}
	}
}

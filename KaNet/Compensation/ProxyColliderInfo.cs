using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Synchronizers;
using UnityEngine;

namespace KaNet.Compensation
{
	//public readonly struct HitscanInfo
	//{
	//	public readonly NetVector3 HitPosition;
	//	public readonly NetInt32 Damage;
	//}

	public class ProxyCollider : MonoBehaviour
	{
		public event Action<NetObjectID>  OnCollisionHit;

		public NetObjectID ProxyNetObjectID { get; set; }

		public void SetByInfo(ProxyColliderInfo info)
		{
			ProxyNetObjectID = info.RepresentObjectID;
			transform.position = info.Position;
			transform.rotation = info.Rotation;
		}
	}

	public struct ProxyColliderInfo
	{
		public NetObjectID RepresentObjectID;
		public Vector3 Position;
		public Quaternion Rotation;

		public ProxyColliderInfo
		(
			NetObjectID representObjectID,
			Vector3 position,
			Quaternion rotation
		)
		{
			RepresentObjectID = representObjectID;
			Position = position;
			Rotation = rotation;
		}
	}
}

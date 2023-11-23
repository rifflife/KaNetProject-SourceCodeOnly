using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaNet.Core;
using KaNet.Session;
using KaNet.Synchronizers;
using KaNet.Utils;
using UnityEngine;
using Utils;

namespace KaNet.Synchronizers
{
	/// <summary>네트워크 오브젝트를 공간분할하여 관리하는 클래스입니다. 서버에서만 동작합니다.</summary>
	public class Tessellator
	{
		/// <summary>공간 분할된 영역의 개별 정보 테이블입니다.</summary>
		//private BidirectionalMap<TessellateCoord, TesselTile> mTileTable = new();
		/// <summary>네트워크 오브젝트와 공간분할 영역 테이블입니다.</summary>
		//private Dictionary<NetworkObject, TesselTile> mObjectTable = new();

		private TesselTile mSingleTessel = new TesselTile(new TessellateCoord(0, 0, 0));

		/// <summary>모든 네트워크 게임 오브젝트입니다.</summary>
		private Dictionary<NetObjectID, NetworkObject> mObjectByID = new();

		// Divide Cell Size
		public const float CellX = 32;
		public const float CellY = 32;
		public const float CellZ = 32;

		private SessionHandlerService mSessionHandler;

		/// <summary>반드시 서버만 초기화해야합니다.</summary>
		public Tessellator(SessionHandlerService sessionHandler)
		{
			mSessionHandler = sessionHandler;
		}

		private List<NetLifeStreamToken> mLifeStream = new();

		/// <summary>객체들의 공간 분할 좌표를 계산합니다.</summary>
		public void CalculateCoord()
		{
			//foreach (var no in mObjectByID.Values)
			//{
			//	calulateCoordByNetObj(no);
			//}
		}

		private void calulateCoordByNetObj(NetworkObject netObj)
		{
			//var force = new TessellateCoord(0, 0, 0);

			//if (mTileTable.TryGetValue(force, out var tessel))
			//{
			//	tessel.TryAddObject(netObj);
			//}
			//else
			//{
			//	mTileTable.TryAdd(force, new TesselTile(force));
			//}

			//var coord = new TessellateCoord(netObj.transform.position, CellX, CellY, CellZ);

			//if (mObjectTable.TryGetValue(netObj, out var checkTile))
			//{
			//	if (checkTile.TesselCoord == coord)
			//	{
			//		return;
			//	}

			//	checkTile.RemoveObject(netObj);

			//	if (checkTile.ObjectList.IsEmpty())
			//	{
			//		mTileTable.TryRemove(coord);
			//	}

			//	if (!mTileTable.TryGetValue(coord, out var tesselTile))
			//	{
			//		tesselTile = new TesselTile(coord);
			//		mTileTable.Add(coord, tesselTile);
			//	}

			//		tesselTile.AddObject(netObj);
			//}
			//else
			//{
			//	if (!mTileTable.TryGetValue(coord, out var tesselTile))
			//	{
			//		tesselTile = new TesselTile(coord);
			//		mTileTable.Add(coord, tesselTile);
			//	}

			//	tesselTile.AddObject(netObj);
			//}
		}

		public void Clear()
		{
			//mTileTable.Clear();
			//mObjectTable.Clear();
			mObjectByID.Clear();
			mSingleTessel.ObjectList.Clear();
		}

		#region Getter

		public List<TesselTile> GetAllTesselTile()
		{
			return new List<TesselTile>() { mSingleTessel };
			//return mTileTable.ForwardValues.ToList();
		}

		public List<NetLifeStreamToken> GetLifeStream()
		{
			var tokens = new List<NetLifeStreamToken>(mLifeStream);
			mLifeStream.Clear();
			return tokens;
		}

		#endregion

		#region Events

		public void OnAddNetworkObject(NetworkObject no)
		{
			Debug.Assert(!mObjectByID.ContainsKey(no.ID));
			mLifeStream.Add(new NetLifeStreamToken(no, true));
			mObjectByID.Add(no.ID, no);
			mSingleTessel.ObjectList.Add(no);
			//calulateCoordByNetObj(no);
		}

		public void OnRemoveNetworkObject(NetworkObject no)
		{
			mLifeStream.Add(new NetLifeStreamToken(no, false));
			mObjectByID.Remove(no.ID);
			mSingleTessel.ObjectList.Remove(no);
		}

		#endregion
	}
}

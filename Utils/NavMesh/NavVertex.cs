using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.NavMesh
{
	//[StructLayout(LayoutKind.Sequential)]
	//public struct NavNode
	//{
	//	public int Index;
	//	public int Cost;

	//	public NavNode(int index, int cost)
	//	{
	//		Index = index;
	//		Cost = cost;
	//	}
	//}

	//public class NavVertex
	//{
	//	public Vector3 Vertex3 { get; private set; }
	//	public int Index { get; private set; }
	//	public readonly List<int> LinkedIndex = new List<int>();

	//	public NavVertex(float x, float y)
	//	{
	//		Vertex3 = new Vector3(x, 0, y);
	//	}

	//	public NavVertex(int x, int y)
	//	{
	//		Vertex3 = new Vector3(x, 0, y);
	//	}

	//	public NavVertex(TileCoord tileCoord)
	//	{
	//		Vertex3 = new Vector3(tileCoord.X, 0, tileCoord.Y);
	//	}

	//	public void Add(int index)
	//	{
	//		if (index == this.Index)
	//		{
	//			return;
	//		}

	//		LinkedIndex.TryAddUnique(index);
	//	}

	//	public void Remove(int index)
	//	{
	//		LinkedIndex.Remove(index);
	//	}
	//}
}

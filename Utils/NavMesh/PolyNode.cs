using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.NavMesh
{
	public class PolyNode
	{
		public Vector3[] Vertices { get; private set; }
		public Vector3 Center => (Vertices[0] + Vertices[1] + Vertices[2]) * 0.33333F;
		public int Index;
		public List<(PolyNode Node, int Cost)> NodeList;

		public PolyNode(int index, Vector3 point1, Vector3 point2, Vector3 point3)
		{
			Index = index;

			Vertices = new Vector3[3];
			Vertices[0] = point1;
			Vertices[1] = point2;
			Vertices[2] = point3;
		}

		public void AddNodeIndices(PolyNode polyNode)
		{
			if (!IsNearbyPolyNode(polyNode))
			{
				return;
			}

			int cost = GetCost(polyNode);
			NodeList.Add((polyNode, cost));
		}

		public bool IsNearbyPolyNode(PolyNode other)
		{
			List<Vector3> originVertices = new List<Vector3>(Vertices);
			List<Vector3> otherVertices = new List<Vector3>(other.Vertices);

			int matchCount = 0;

			for (int i = 2; i >= 0; i--)
			{
				for (int k = otherVertices.Count - 1; k >= 0; k--)
				{
					if (originVertices[i] == otherVertices[k])
					{
						originVertices.RemoveAt(i);
						otherVertices.RemoveAt(i);
						matchCount++;
						break;
					}
				}
			}

			return matchCount == 2;
		}

		public int GetCost(PolyNode other) => (int)(this.Center - other.Center).sqrMagnitude;
	}
}

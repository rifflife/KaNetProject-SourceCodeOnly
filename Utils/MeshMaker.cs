using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
	public static class MeshMaker
	{
		/// <summary>원점을 기준으로 XZ 평면에 위치하는 Quad Mesh를 생성합니다.</summary>
		/// <returns></returns>
		public static Mesh MakeQuadFromOrigin(float size)
		{
			Vector3 v0 = Vector3.zero;
			Vector3 v1 = Vector3.forward * size;
			Vector3 v2 = Vector3.right * size;
			Vector3 v3 = v1 + v2;

			Vector3 n = Vector3.up;

			Mesh quad = new Mesh();

			Vector3[] vertices = new Vector3[4] { v0, v1, v2, v3 };
			Vector3[] normals = new Vector3[4] { n, n, n, n };
			int[] indices = new int[6] { 0, 1, 2, 2, 1, 3 };

			quad.SetVertices(vertices);
			quad.SetNormals(normals);
			quad.SetIndices(indices, MeshTopology.Triangles, 0);

			quad.RecalculateBounds();

			return quad;
		}
	}
}

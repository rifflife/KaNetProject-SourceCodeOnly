using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils.NavMesh;

namespace Utils
{
	public static class Debugger
	{
		[Conditional("UNITY_EDITOR")]
		public static void DrawLine(Vector3 start, Vector3 end) => UnityEngine.Debug.DrawLine(start, end);

		[Conditional("UNITY_EDITOR")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color) => UnityEngine.Debug.DrawLine(start, end, color);

		[Conditional("UNITY_EDITOR")]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) => UnityEngine.Debug.DrawLine(start, end, color, duration);

		[Conditional("UNITY_EDITOR")]
		public static void DrawRay(Vector3 start, Vector3 dir) => UnityEngine.Debug.DrawRay(start, dir);

		[Conditional("UNITY_EDITOR")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color) => UnityEngine.Debug.DrawRay(start, dir, color);

		[Conditional("UNITY_EDITOR")]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration) => UnityEngine.Debug.DrawRay(start, dir, color, duration);

		[Conditional("UNITY_EDITOR")]
		public static void DrawTile(in TileCoord coord, float unitSize, in Vector3 offset, in Color outline, in Color fill)
		{
			//Gizmos.color = fill;

			//Vector3 cv = coord.ToVectorXZ();

			//Vector3 v0 = offset + cv;
			//Vector3 v1 = offset + cv + new Vector3(unitSize, 0, 0);
			//Vector3 v2 = offset + cv + new Vector3(0, 0, unitSize);
			//Vector3 v3 = offset + cv + new Vector3(unitSize, 0, unitSize);

			//Vector3 n = Vector3.up;

			//Mesh mesh = new Mesh();

			//Vector3[] vertices = new Vector3[4] { v0, v1, v2, v3 };
			//int[] indices = new int[6] { 0, 2, 1, 2, 3, 1 };
			//Vector3[] normals = new Vector3[4] { n, n, n, n };

			//mesh.SetVertices(vertices);
			//mesh.SetIndices(indices, MeshTopology.Triangles, 0);
			//mesh.SetNormals(normals);

			//mesh.RecalculateBounds();

			//Gizmos.DrawMesh(mesh);
			//Gizmos.color = outline;
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Color fillColor, Color outColor)
		{
			Gizmos.color = fillColor;
			Gizmos.DrawMesh(mesh, position);
			Gizmos.color = outColor;
			Gizmos.DrawWireMesh(mesh, position);
		}

		public static void DrawBitMask2D(BitmaskVector mask, Mesh quad, float offset, Color color)
		{
			Gizmos.color = color;

			for (int y = 0; y < mask.SizeY; y++)
			{
				for (int x = 0; x < mask.SizeX; x++)
				{
					if (mask[y, x])
					{
						Gizmos.DrawMesh(quad, new Vector3(x * offset, 0, y * offset));
					}
				}
			}
		}

		public static void DrawNavSquareNode(NavSquareNode node, Mesh quad, Color color)
		{
			Gizmos.color = color;

			TileCoord endPoint = node.OppositePoint;

			for (int y = node.Position.Y; y < endPoint.Y; y++)
			{
				for (int x = node.Position.X; x < endPoint.X; x++)
				{
					Gizmos.DrawMesh(quad, new Vector3(x, 0, y));
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils.NavMesh
{
	public class NavTile
	{
	}



	public class NavTileMap
	{
		public bool this[int y, int x]
		{
			get => mCollisionMap[y, x];
			set => mCollisionMap[y, x] = value;
		}

		public int SizeX => mCollisionMap.SizeX;
		public int SizeY => mCollisionMap.SizeY;

		private List<Vector3> mVertices = new List<Vector3>();
		private List<int> mIndices = new List<int>();

		public const int UNIT_SIZE = 3;
		private BitmaskVector mCollisionMap;

		public NavTileMap()
		{
			mCollisionMap = BitmaskVector.Create(32, 10, false);
			SetMapByIntField(TestResources.TestMap2);
		}

		public NavTileMap(int width, int height)
		{
			mCollisionMap = BitmaskVector.Create(width, height, false);
		}

		/// <summary>특정 유닛이 돌아다닐 수 있는 영역을 반환받습니다.</summary>
		/// <param name="unitSize">유닛의 크기입니다.</param>
		/// <returns>돌아다닐 수 있는 영역 맵 입니다.</returns>
		/// <exception cref="ArgumentException">잘못된 인자</exception>
		public BitmaskVector GetWalkableArea(int unitSize)
		{
			if (unitSize <= 0 || unitSize > 20)
			{
				throw new ArgumentException("잘못된 Unit Size 입니다.");
			}

			if (unitSize == 1)
			{
				var copied = mCollisionMap.Clone();
				copied.Flip();
				return copied;
			}

			BitmaskVector walkableMap = BitmaskVector.Create(mCollisionMap.SizeX, mCollisionMap.SizeY, false);

			int unitOffset = unitSize - 1;

			int checkAreaX = walkableMap.SizeX - unitOffset;
			int checkAreaY = walkableMap.SizeY - unitOffset;

			for (int y = 0; y < checkAreaY; y++)
			{
				for (int x = 0; x < checkAreaX; x++)
				{
					if (mCollisionMap[y, x])
					{
						continue;
					}

					bool hasWall = false;

					for (int uy = 0; uy < unitSize; uy++)
					{
						for (int ux = 0; ux < unitSize; ux++)
						{
							hasWall |= mCollisionMap[y + uy, x + ux];

							if (hasWall) break;
						}

						if (hasWall) break;
					}

					if (!hasWall)
					{
						for (int uy = 0; uy < unitSize; uy++)
						{
							for (int ux = 0; ux < unitSize; ux++)
							{
								walkableMap[y + uy, x + ux] = true;
							}
						}
					}
				}
			}

			return walkableMap;
		}

		public List<NavSquareNode> GetSquareNodeList(BitmaskVector walkableArea, int unitSize)
		{
			BitmaskVector checkMap = walkableArea.Clone();

			List<NavSquareNode> nodeList = new List<NavSquareNode>();

			for (int nodeIndex = 0; nodeIndex < 100; nodeIndex++)
			{
				bool hasArea = false;

				TileCoord startPos = TileCoord.Zero;

				for (int y = 0; y < checkMap.SizeY; y++)
				{
					for (int x = 0; x < checkMap.SizeX; x++)
					{
						if (checkMap[y, x])
						{
							startPos = new TileCoord(x, y);
							hasArea = true;
							break;
						}
					}

					if (hasArea) break;
				}

				if (!hasArea) break;

				int cSizeX = 1;
				int cSizeY = 1;

				for (int i = 0; i < 1000; i++)
				{
					// X 방향 검사
					bool expandX;

					if (startPos.X + cSizeX < checkMap.SizeX)
					{
						expandX = true;

						for (int uy = 0; uy < cSizeY; uy++)
						{
							if (checkMap[startPos.Y + uy, startPos.X + cSizeX] == false)
							{
								expandX = false;
								break;
							}
						}

						if (expandX)
						{
							cSizeX++;
						}
					}
					else
					{
						expandX = false;
					}

					// Y 방향 검사
					bool expandY;

					if (startPos.Y + cSizeY < checkMap.SizeY)
					{
						expandY = true;

						for (int ux = 0; ux < cSizeX; ux++)
						{
							if (checkMap[startPos.Y + cSizeY, startPos.X + ux] == false)
							{
								expandY = false;
								break;
							}
						}

						if (expandY)
						{
							cSizeY++;
						}
					}
					else
					{
						expandY = false;
					}

					if (!expandX && !expandY)
					{
						break;
					}
				}

				for (int ry = 0; ry < cSizeY; ry++)
				{
					for (int rx = 0; rx < cSizeX; rx++)
					{
						checkMap[startPos.Y + ry, startPos.X + rx] = false;
					}
				}

				nodeList.Add(new NavSquareNode(nodeIndex, unitSize, startPos, new TileCoord(cSizeX, cSizeY)));
			}

			return nodeList;
		}

		public List<NavSquareNode> GetLinkedNode(List<NavSquareNode> navSquareNodes)
		{
			// Find and bind nearby nodes
			for (int i = 0; i < navSquareNodes.Count; i++)
			{
				var curNode = navSquareNodes[i];

				for (int k = 0; k < navSquareNodes.Count; k++)
				{
					if (k == i)
					{
						continue;
					}

					var checkNode = navSquareNodes[k];

					curNode.TryAddNearNode(checkNode);
					if (curNode.IsCollideWith(checkNode))
					{
					}
				}
			}

			return navSquareNodes;
		}

		public void Generate()
		{
			
		}

		public void SetMapByIntField(in int[,] field)
		{
			int strideY = field.GetLength(0);
			int strideX = field.GetLength(1);

			for (int y = 0; y < strideY; y++)
			{
				for (int x = 0; x < strideX; x++)
				{
					mCollisionMap[y, x] = field[y, x] == 0 ? false : true;
				}
			}
		}
	}
}

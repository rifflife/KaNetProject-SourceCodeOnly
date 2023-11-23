using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.NavMesh
{
	public class NavSquareNode
	{
		public int Index { get; private set;}
		public int UnitSize { get; private set;}
		public TileCoord Position { get; private set; }
		public TileCoord Size { get; private set; }
		public TileCoord OppositePoint { get; private set; }
		private List<NavSquareNode> mNearNode = new List<NavSquareNode>();
		private List<int> mIndices = new List<int>();

		public NavSquareNode(int index, int unitSize, in TileCoord position, in TileCoord size)
		{
			Index = index;
			UnitSize = unitSize;
			Position = position;
			Size = size;
			OppositePoint = Position + Size;
		}

		public bool TryAddNearNode(NavSquareNode node)
		{
			if (this.UnitSize != node.UnitSize)
			{
				throw new ArgumentException("서로 다른 Unit Size를 가진 노드를 추가하려고 했습니다.");
			}

			if (!IsCollideWith(node))
			{
				return false;
			}

			if (!IsThroughableWith(node)) 
			{
				return false;
			}

			mNearNode.TryAddUnique(node);
			mIndices.TryAddUnique(node.Index);

			return true;
		}

		public bool IsCollideWith(NavSquareNode target)
		{
			return !(this.Position.X > target.OppositePoint.X || this.OppositePoint.X < target.Position.X ||
				this.Position.Y > target.OppositePoint.Y || this.OppositePoint.Y < target.Position.Y);
		}

		public bool IsThroughableWith(NavSquareNode target)
		{
			return true;
		}

		public bool IsInclude(TileCoord pivot)
		{
			return (pivot.X < this.Position.X || pivot.X >= this.OppositePoint.X ||
				pivot.Y < this.Position.Y || pivot.Y >= this.OppositePoint.Y);
		}

		public void GeneratePolyNode()
		{

		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < mIndices.Count; i++)
			{
				sb.Append(mIndices[Index]);

				if (i != mIndices.Count - 1)
				{
					sb.Append(", ");
				}
			}

			return $"({Index}:{sb})";
		}
	}
}

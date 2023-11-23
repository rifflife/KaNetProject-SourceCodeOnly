using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Utils;
using Utils.NavMesh;

public class Tester_NavMesh : MonoBehaviour
{
	[Test]
	public void Test_TileCoord()
	{
		TileCoord coord_1 = new TileCoord(10, 20);
		TileCoord coord_2 = new TileCoord(5, 7);

		Assert.AreEqual(new TileCoord(15, 27), coord_1 + coord_2);
		Assert.AreEqual(new TileCoord(5, 13), coord_1 - coord_2);
		
		Assert.AreEqual(new TileCoord(30, 60), coord_1 * 3);
		Assert.AreEqual(new TileCoord(5, 10), coord_1 / 2);

		TileCoord e1 = new TileCoord(10, 20);
		TileCoord e2 = new TileCoord(10, 20);

		Assert.IsTrue(e1.Equals(e2));
		Assert.AreEqual(e1, e2);
	}

	[Test]
	public void Test_TileMap()
	{
		NavTileMap navTileMap = new NavTileMap();
	}
}

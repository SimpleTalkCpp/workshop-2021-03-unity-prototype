using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMap : MonoBehaviour
{
	public int mapWidth = 14;
	public int mapHeight = 30;

	int[,] tiles;

	private void Awake()
	{
		tiles = new int[mapWidth, mapHeight];
		tiles[0,0] = 1;
		tiles[3,4] = 1;
	}

	public bool IsOverlapped(MyPiece.Shape shape, Vector2Int pos) {

		var s = shape;
		for (int y = 0; y < 4; y++) {
			for (int x = 0; x < 4; x++) {
				if (s.cells[x, y] == 0) continue;

				int px = pos.x + x;
				int py = pos.y + y;

				if (px < 0 || px >= mapWidth ) return true;
				if (py < 0 || py >= mapHeight) return true;
				if (tiles[px, py] != 0) return true;
			}
		}

		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		var drawSize = MyPiece.drawSize;
		{
			var mapSize = Vector3.Scale(new Vector3(mapWidth, mapHeight, 1), drawSize);
			Gizmos.DrawWireCube(mapSize * 0.5f - drawSize * 0.5f, mapSize);
		}

		if (tiles == null) return;

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				if (tiles[x, y] == 0) continue;

				var pos = Vector3.Scale(new Vector3(x,y,0), drawSize);
				Gizmos.DrawWireCube(pos, drawSize);
			}
		}
	}

}

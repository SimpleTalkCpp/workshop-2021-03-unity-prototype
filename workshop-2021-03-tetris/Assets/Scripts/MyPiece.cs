using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class MyUtil
{
	public static int EnumCount<T>() {
		return System.Enum.GetValues(typeof(T)).Length;
	}
}

public class MyPiece : MonoBehaviour
{
	public static Vector3 drawSize => Vector3.one * 1;

	public class Shape {
		public enum Type {I, J, L, O, T, S, Z }
		public enum Dir  {N, E, S, W }
		public int[,] cells;

		public Shape(int [,] cells_) {
			cells = cells_;
		}

		public static int typeCount => MyUtil.EnumCount<Shape.Type>();
		public static int dirCount  => MyUtil.EnumCount<Shape.Dir>();
	}

	public class ShapeTable {
		Shape[,] _shapes;

		public ShapeTable() {
			var typeCount = Shape.typeCount;
			var dirCount  = Shape.dirCount;

			_shapes = new Shape[typeCount, dirCount];

			_shapes[(int)Shape.Type.I, 0] = new Shape(new int[4,4] {
				{0,1,0,0},
				{0,1,0,0},
				{0,1,0,0},
				{0,1,0,0},
			});

			_shapes[(int)Shape.Type.J, 0] = new Shape(new int[4,4] {
				{0,0,1,0},
				{0,0,1,0},
				{0,1,1,0},
				{0,0,0,0},
			});

			_shapes[(int)Shape.Type.L, 0] = new Shape(new int[4,4] {
				{0,1,0,0},
				{0,1,0,0},
				{0,1,1,0},
				{0,0,0,0},
			});

			_shapes[(int)Shape.Type.O, 0] = new Shape(new int[4,4] {
				{0,0,0,0},
				{0,1,1,0},
				{0,1,1,0},
				{0,0,0,0},
			});

			_shapes[(int)Shape.Type.T,0] = new Shape(new int[4,4] {
				{0,0,0,0},
				{0,1,0,0},
				{1,1,1,0},
				{0,0,0,0},
			});

			_shapes[(int)Shape.Type.S,0] = new Shape(new int[4,4] {
				{0,0,0,0},
				{0,1,0,0},
				{0,1,1,0},
				{0,0,1,0},
			});

			_shapes[(int)Shape.Type.Z,0] = new Shape(new int[4,4] {
				{0,0,0,0},
				{0,1,0,0},
				{0,1,1,0},
				{0,0,1,0},
			});

			for (int t = 0; t < typeCount; t++) {
				for (int d = 1; d < dirCount; d++) {
					_shapes[t, d] = RotateShape(_shapes[t, d-1]);
				}
			}
		}

		Shape RotateShape(Shape s) {
			var c = s.cells;
			var o = new Shape(
				new int[4,4] {
					{ c[3,0], c[2,0], c[1,0], c[0,0]},
					{ c[3,1], c[2,1], c[1,1], c[0,1]},
					{ c[3,2], c[2,2], c[1,2], c[0,2]},
					{ c[3,3], c[2,3], c[1,3], c[0,3]},
				}
			);
			return o;
		}

		public Shape GetShape(Shape.Type type, Shape.Dir dir) {
			return _shapes[(int)type, (int)dir];
		}
	};

	public static ShapeTable shapeTable = new ShapeTable();

	public Vector2Int pos;
	public Shape.Type type;
	public Shape.Dir  dir;

	public Shape shape => shapeTable.GetShape(type, dir);

	public Shape.Dir rotateDir(int r) {
		var newDir = ((int)dir + Shape.dirCount + r) % Shape.dirCount;
		return (Shape.Dir)newDir;
	}

	public void RandomShape() {
		type = (Shape.Type)UnityEngine.Random.Range(0, Shape.typeCount);
		dir  = (Shape.Dir )UnityEngine.Random.Range(0, Shape.dirCount);
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.red;

		var s = shape;
		for (int y = 0; y < 4; y++) {
			for (int x = 0; x < 4; x++) {
				if (s.cells[x, y] == 0) continue;

				var p = Vector3.Scale(new Vector3(pos.x + x, pos.y + y,0), drawSize);
				Gizmos.DrawWireCube(p, drawSize);
			}
		}
	}
}
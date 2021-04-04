using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayer : MonoBehaviour
{
    MyMap map;
    MyPiece piece;

    public float moveCooldown = 0.05f;
    float _moveCooldownRemain = 0;

    public float fallCooldown = 0.1f;
    float _fallCooldownRemain = 0;

	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
    {
        map = GetComponent<MyMap>();
        NewPiece();
    }

	private void Update()
	{
        _fallCooldownRemain -= Time.deltaTime;
        if (_fallCooldownRemain < 0) {
            _fallCooldownRemain = fallCooldown;
            MovePiece(0, -1, 0);
        }

        _moveCooldownRemain -= Time.deltaTime;

        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb.sKey.isPressed) {
            MovePiece(0, -1, 0);
        } else if (kb.wKey.isPressed) {
            MovePiece(0, 1, 0);
        } else if (kb.aKey.isPressed) {
            MovePiece(-1, 0, 0);
        } else if (kb.dKey.isPressed) {
            MovePiece(1, 0, 0);
        } else if (kb.qKey.wasPressedThisFrame) {
            MovePiece(0, 0, -1);
        } else if (kb.eKey.wasPressedThisFrame) {
            MovePiece(0, 0, 1);
        }


        if (kb.nKey.wasPressedThisFrame) {
            NewPiece();
        }
	}

	void NewPiece() {
        if (!piece) {
            var pieceObj = new GameObject("Piece");
            pieceObj.transform.SetParent(transform, false);
            piece = pieceObj.AddComponent<MyPiece>();
        }

        piece.RandomShape();
        piece.pos.x = map.mapWidth / 2 - 2;
        piece.pos.y = map.mapHeight - 4;
    }

	void MovePiece(int x, int y, int rotate) { MovePiece(new Vector2Int(x, y), rotate); }

    void MovePiece(Vector2Int offset, int rotate) {
        if (_moveCooldownRemain > 0) {
            return;
        }
        _moveCooldownRemain = moveCooldown;

        var pos = piece.pos + offset;

        var newDir = piece.rotateDir(rotate);

        var shape = MyPiece.shapeTable.GetShape(piece.type, newDir);

        if (map.IsOverlapped(shape, pos)) {
            return;
        }

        piece.pos += offset;
        piece.dir = newDir;
    }
}

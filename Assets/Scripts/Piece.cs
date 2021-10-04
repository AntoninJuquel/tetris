using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float stepDelay = 1f, lockDelay = .5f;
    private float _lockTime, _stepTime;
    public Board Board { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    public TetrominoData Data { get; private set; }
    public int RotationIndex { get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        Board = board;
        Position = position;
        Data = data;
        RotationIndex = 0;
        _stepTime = Time.time - stepDelay;
        _lockTime = 0f;
        Cells = new Vector3Int[data.Cells.Length];
        for (var i = 0; i < data.Cells.Length; i++)
        {
            Cells[i] = (Vector3Int) data.Cells[i];
        }
    }

    private void Update()
    {
        Board.Clear(this);

        _lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A))
            Rotate(-1);
        if (Input.GetKeyDown(KeyCode.E))
            Rotate(1);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Move(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        if (Time.time >= _stepTime) Step();

        Board.Set(this);
    }

    private void Step()
    {
        _stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (_lockTime >= lockDelay) Lock();
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        _lockTime = lockDelay;
    }

    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        var newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        var canMove = Board.IsValidPosition(this, newPosition);
        if (canMove)
        {
            Position = newPosition;
            _lockTime = 0;
        }

        return canMove;
    }

    private void Rotate(int direction)
    {
        var originalRotation = RotationIndex;
        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);
        if (TestWallKicks(RotationIndex, direction)) return;
        RotationIndex = originalRotation;
        ApplyRotationMatrix(-direction);
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (var i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];
            int x, y;
            switch (Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= .5f;
                    cell.y -= .5f;
                    x = Mathf.CeilToInt((cell.x * global::Data.RotationMatrix[0] * direction) + (cell.y * global::Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * global::Data.RotationMatrix[2] * direction) + (cell.y * global::Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * global::Data.RotationMatrix[0] * direction) + (cell.y * global::Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * global::Data.RotationMatrix[2] * direction) + (cell.y * global::Data.RotationMatrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (var i = 0; i < Data.WallKicks.GetLength(1); i++)
        {
            var translation = Data.WallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0)
            wallKickIndex--;
        return Wrap(wallKickIndex, 0, Data.WallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max) => input < min ? max - (min - input) % (max - min) : min + (input - min) % (max - min);
}
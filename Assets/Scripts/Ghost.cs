using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Tile tile;
    [SerializeField] private Board board;
    [SerializeField] private Piece piece;

    public Tilemap Tilemap { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (var i = 0; i < Cells.Length; i++)
        {
            var tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (var i = 0; i < Cells.Length; i++)
            Cells[i] = piece.Cells[i];
    }

    private void Drop()
    {
        var position = piece.Position;

        var current = position.y;
        var bottom = -board.boardSize.y / 2 - 1;

        board.Clear(piece);

        for (var row = current; row >= bottom; row--)
        {
            position.y = row;
            if (board.IsValidPosition(piece, position))
                Position = position;
            else
                break;
        }

        board.Set(piece);
    }

    private void Set()
    {
        for (var i = 0; i < Cells.Length; i++)
        {
            var tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, tile);
        }
    }
}
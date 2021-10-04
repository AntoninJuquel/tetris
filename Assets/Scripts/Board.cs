using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public Vector2Int boardSize = new Vector2Int(10, 20);
    [SerializeField] private TetrominoData[] tetrominoes;
    [SerializeField] private Vector3Int spawnPosition, previewPosition;
    [SerializeField] private TextMeshProUGUI scoreText, bestScoreText;
    private TetrominoData _currentPiece, _nextPiece;
    private int _score;

    public RectInt Bounds
    {
        get
        {
            var position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    public Tilemap Tilemap { get; private set; }
    public Piece Piece { get; private set; }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Piece = GetComponent<Piece>();
        for (var i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }

        bestScoreText.text = "BEST\n" + PlayerPrefs.GetInt("BEST", 0).ToString("00000");
    }

    private void Start()
    {
        _nextPiece = tetrominoes[Random.Range(0, tetrominoes.Length)];
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        _currentPiece = _nextPiece;

        ClearCurrentPreview();

        Piece.Initialize(this, spawnPosition, _currentPiece);
        _nextPiece = tetrominoes[Random.Range(0, tetrominoes.Length)];

        DrawNextPreview();

        if (IsValidPosition(Piece, spawnPosition))
            Set(Piece);
        else
            GameOver();
    }

    private void ClearCurrentPreview()
    {
        for (var i = 0; i < _currentPiece.Cells.Length; i++)
        {
            var tilePosition = (Vector3Int) _currentPiece.Cells[i] + previewPosition;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    private void DrawNextPreview()
    {
        for (var i = 0; i < _nextPiece.Cells.Length; i++)
        {
            var tilePosition = (Vector3Int) _nextPiece.Cells[i] + previewPosition;
            Tilemap.SetTile(tilePosition, _nextPiece.tile);
        }
    }

    private void GameOver()
    {
        if (_score > PlayerPrefs.GetInt("BEST", 0))
            PlayerPrefs.SetInt("BEST", _score);
        SceneManager.LoadScene("Tetris");
    }

    public void Set(Piece piece)
    {
        for (var i = 0; i < piece.Cells.Length; i++)
        {
            var tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (var i = 0; i < piece.Cells.Length; i++)
        {
            var tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        for (var i = 0; i < piece.Cells.Length; i++)
        {
            var tilePosition = piece.Cells[i] + position;
            if (Tilemap.HasTile(tilePosition) || !Bounds.Contains((Vector2Int) tilePosition)) return false;
        }

        return true;
    }

    public void ClearLines()
    {
        var row = Bounds.yMin;

        while (row < Bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        for (var col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            var position = new Vector3Int(col, row, 0);
            if (!Tilemap.HasTile(position))
                return false;
        }

        return true;
    }

    private void LineClear(int row)
    {
        for (var col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            var position = new Vector3Int(col, row, 0);
            Tilemap.SetTile(position, null);
            _score += 10;
            scoreText.text = "SCORE\n" + _score.ToString("00000");
        }

        while (row < Bounds.yMax)
        {
            for (var col = Bounds.xMin; col < Bounds.xMax; col++)
            {
                var position = new Vector3Int(col, row + 1, 0);
                var above = Tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                Tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
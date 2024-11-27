using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject[] PiecePrefabs;
    public GameObject EffectPrefab;

    private Transform TileParent;
    private Transform PieceParent;
    private Transform EffectParent;
    private MovementManager movementManager;
    private UIManager uiManager;
    
    public int CurrentTurn = 1;
    public Tile[,] Tiles = new Tile[Utils.FieldWidth, Utils.FieldHeight];
    public Piece[,] Pieces = new Piece[Utils.FieldWidth, Utils.FieldHeight];

    void Awake()
    {
        TileParent = GameObject.Find("TileParent").transform;
        PieceParent = GameObject.Find("PieceParent").transform;
        EffectParent = GameObject.Find("EffectParent").transform;
        
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        movementManager = gameObject.AddComponent<MovementManager>();
        movementManager.Initialize(this, EffectPrefab, EffectParent);
        
        InitializeBoard();
    }

    void InitializeBoard()
    {
        // 8x8로 타일들을 배치
        // --- TODO ---
        for (int i = 0; i < Utils.FieldWidth; i++) {
            for (int j = 0; j < Utils.FieldHeight; j++) {
                Tile tile = Instantiate(TilePrefab).GetComponent<Tile>();
                tile.Set((i,j));
                Tiles[i,j] = tile;
            }
        }
        // ------

        PlacePieces(1);
        PlacePieces(-1);
    }

    void PlacePieces(int direction)
    {
        // 체스 말들을 배치
        // --- TODO ---
        int j;
        j = (direction > 0) ? 0 : 8;
        for (int i = 0; i < Utils.FieldWidth; i++) {
            PlacePiece(1,(i,j),direction); //
        }

        j = (direction > 0) ? 1 : 7;
        for (int i = 0; i < Utils.FieldWidth; i++) {
            PlacePiece(0,(i,j),direction); //
        }
        // ------
    }

    Piece PlacePiece(int pieceType, (int, int) pos, int direction)
    {
        // 체스 말 하나를 배치 후 initialize
        // --- TODO ---
        GameObject PiecePrefab = PiecePrefabs[pieceType];
        Piece piece = Instantiate(PiecePrefab).GetComponent<Piece>();
        piece.initialize(pos,direction);
        return piece;
        // ------
    }

    public bool IsValidMove(Piece piece, (int, int) targetPos)
    {
        return movementManager.IsValidMove(piece, targetPos);
    }

    public void ShowPossibleMoves(Piece piece)
    {
        movementManager.ShowPossibleMoves(piece);
    }

    public void ClearEffects()
    {
        movementManager.ClearEffects();
    }


    public void Move(Piece piece, (int, int) targetPos)
    {
        if (!IsValidMove(piece, targetPos)) return;
        
        // 체스 말을 이동하고, 만약 해당 자리에 상대 말이 있다면 삭제
        // --- TODO ---
        
        // ------
    }

    void ChangeTurn()
    {
        // 턴을 변경하고, UI에 표시
        // --- TODO ---
        CurrentTurn = -CurrentTurn; //?
        uiManager.UpdateTurn(CurrentTurn);
        // ------
    }
}

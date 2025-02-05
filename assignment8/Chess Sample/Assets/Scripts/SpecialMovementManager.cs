using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpecialMovementManager : MonoBehaviour
{
    private GameManager gameManager;
    private Piece[,] EnPassantPieces = new Piece[Utils.FieldWidth, Utils.FieldHeight];    // Piece들
    (int, int) LastPos = (0,0);

    public void Initialize(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void UpdateEnPassantPrey(Piece targetPiece, (int, int) prevPos) {
        EnPassantPieces[LastPos.Item1, LastPos.Item2] = null; // 매 턴마다 초기화
        if (targetPiece is Pawn) {
            (int prevPosX, int prevPosY) = prevPos;
            (int posX, int posY) = targetPiece.MyPos;
            int incX = posX - prevPosX;
            int incY = posY - prevPosY;

            if (incX == 0 && (incY == 2 || incY == -2)) {
                incY = targetPiece.PlayerDirection;
                EnPassantPieces[posX, prevPosY + incY] = targetPiece;
                LastPos = (posX, prevPosY + incY);
            }
        }
    }

    public bool IsInEnPassant((int, int) targetPos) {
        if (!Utils.IsInBoard(targetPos)) return false;

        if (EnPassantPieces[targetPos.Item1, targetPos.Item2] != null) return true;
        return false;
    }

    /*
    public bool TryEnPassant(Piece pPiece, (int, int) targetPos, MoveInfo moveInfo) {
        if (pPiece is not Pawn) return false;
        if (!IsInEnPassant(targetPos)) return false;

        (int posX, int posY) = pPiece.MyPos;
        (int incX, int incY) = (moveInfo.dirX, moveInfo.dirY);

        if (!Utils.IsInBoard((posX,posY))) return false;
        if (incX == 0) return false;


        posX += incX;
        posY += incY;


        if (targetPos == (posX, posY)) return true;
        
        return false;
    }
    */

    public void DestroyEnPassantPrey((int, int) targetPos) {
        if (IsInEnPassant(targetPos)) {
            Destroy(EnPassantPieces[targetPos.Item1, targetPos.Item2].gameObject);
        }
    }

    public bool IsValidCastlingWithoutCheck(Piece piece, (int, int) targetPos) {
        if (!Utils.IsInBoard(targetPos) || targetPos == piece.MyPos) return false;

        if (piece is not King) return false;
        if (!(targetPos.Item1 - piece.MyPos.Item1 == 2 || targetPos.Item1 - piece.MyPos.Item1 == -2)) return false;
        if (piece.MyPos.Item2 != targetPos.Item2) return false;
        
        // Debug.Log($"valid1, {piece.MyPos}");
        King king = (King)piece;
        int kingPosX = king.MyPos.Item1;
        // int posY = king.MyPos.Item2;// int posY = (king.PlayerDirection == 1) ? 0 : Utils.FieldHeight - 1;

        if (targetPos.Item1 - kingPosX > 0 && king.kCastling) return TryCastling(king, 1);
        else if (targetPos.Item1 - kingPosX < 0 && king.qCastling) return TryCastling(king, -1);
        
        return false;
    }

    public bool TryCastling(King king, int CastlingDirection) {
        int kingPosX = king.MyPos.Item1;
        int posY = king.MyPos.Item2;
        (int, int) posXRange;

        // Debug.Log("king should be moved");
        if (CastlingDirection > 0 && king.kCastling) {
            Piece kRook = gameManager.Pieces[Utils.FieldWidth - 1, posY];
            if (kRook) posXRange = (kingPosX, kRook.MyPos.Item1);
            else return false;
        }
        else if (CastlingDirection < 0 && king.qCastling) {
            Piece qRook = gameManager.Pieces[0, posY];
            if (qRook) posXRange = (qRook.MyPos.Item1, kingPosX);
            else return false;
        }
        else {
            return false;
        }

        for (int i = posXRange.Item1 + 1; i < posXRange.Item2; i++) {
            Piece targetPiece = gameManager.Pieces[i,posY];
            if (targetPiece) {
                return false;
            }
            // Debug.Log((i,posY), targetPiece);
        }

        // Debug.Log((posXRange, posY));
        // Debug.Log("true by default");
        return true;
    }

    public void UpdateCastling(Piece piece, (int, int) prevPos) {
        if (piece is King) {
            King kPiece = (King)piece;
            kPiece.kCastling = false;
            kPiece.qCastling = false;
        }
        else if (piece is Rook) {
            int playerDirection = piece.PlayerDirection;
            Piece temp = null;

            foreach (Piece p in gameManager.Pieces) {
                if (p is King && piece.PlayerDirection == playerDirection) {
                    temp = p;
                }
            }
            
            if (temp) {
                King kPiece = (King)temp;
                if (prevPos.Item1 == 0) kPiece.qCastling = false;
                else kPiece.qCastling = false;
            }
        }
    }
}

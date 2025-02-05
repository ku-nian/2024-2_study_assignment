using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private GameManager gameManager;
    private SpecialMovementManager specialMovementManager;
    private GameObject effectPrefab;
    private Transform effectParent;
    private List<GameObject> currentEffects = new List<GameObject>();   // 현재 effect들을 저장할 리스트
    
    public void Initialize(GameManager gameManager, GameObject effectPrefab, Transform effectParent)
    {
        this.gameManager = gameManager;
        this.effectPrefab = effectPrefab;
        this.effectParent = effectParent;
        specialMovementManager = gameObject.AddComponent<SpecialMovementManager>();
        specialMovementManager.Initialize(gameManager);
    }

    public void UpdateEnPassantPrey(Piece Piece, (int, int) prevPos) {
        specialMovementManager.UpdateEnPassantPrey(Piece, prevPos);
    }

    public bool IsInEnPassant((int, int) targetPos) {
        return specialMovementManager.IsInEnPassant(targetPos);
    }

    public void DestroyEnPassantPrey((int, int) targetPos) {
        specialMovementManager.DestroyEnPassantPrey(targetPos);
    }

    public bool IsValidCastlingWithoutCheck(Piece piece, (int, int) targetPos) {
        return specialMovementManager.IsValidCastlingWithoutCheck(piece, targetPos);
    }

    public void UpdateCastling(Piece piece, (int, int) prevPos) {
        specialMovementManager.UpdateCastling(piece, prevPos);
    }
    private bool TryMove(Piece piece, (int, int) targetPos, MoveInfo moveInfo)
    {
        // moveInfo의 distance만큼 direction을 이동시키며 이동이 가능한지를 체크
        // 보드에 있는지, 다른 piece에 의해 막히는지 등을 체크
        // 폰에 대한 예외 처리를 적용
        // --- TODO ---
        bool wasItPiece = false;
        (int posX, int posY) = piece.MyPos; // position
        (int incX, int incY) = (moveInfo.dirX,moveInfo.dirY); // increment

        if (targetPos != (posX + incX * moveInfo.distance, posY + incY * moveInfo.distance)) return false;
        
        if (piece is Pawn) {
            if (incX == 0) {
                for (int i = 0; i < moveInfo.distance; i++) {
                    posX += incX;
                    posY += incY;
                    if (!Utils.IsInBoard((posX, posY))) return false;
                    
                    Piece targetPiece = gameManager.Pieces[posX,posY];
                    if (targetPiece) return false;
                }
            }
            else {
                posX += incX;
                posY += incY;
                if (!Utils.IsInBoard((posX,posY))) return false;
                Piece targetPiece = gameManager.Pieces[posX,posY];

                //target 존재하고, 아군이면 false
                //target 없고, 앙파상 불가능 false
                if (targetPiece != null && targetPiece.PlayerDirection == piece.PlayerDirection) return false;
                else if (targetPiece == null && !IsInEnPassant((posX, posY))) return false;
            }

            return true;
        }
        else if (piece is King) {
            if ((incX == 2 || incX == -2) && incY == 0) { // 수정 필요??
                return false;
                // int castlingDirection = (incX > 0) ? 1 : -1;
                // if (specialMovementManager.TryCastling((King)piece, castlingDirection)) {
                //     return true;
                // }
            }
        }

        for (int i = 0; i < moveInfo.distance; i++) {
            posX += incX;
            posY += incY;

            // 상대방 기물을 만났던 경우, 상대방 기물이 있는 자리까지만 움직일 수 있고, 현재 자리까지는 못 옴
            if (!Utils.IsInBoard((posX, posY)) || wasItPiece) return false;
            

            Piece targetPiece = gameManager.Pieces[posX,posY];
            if (targetPiece) {
                if (targetPiece.PlayerDirection == piece.PlayerDirection) { // 자신 기물인 경우, 자신 기물은 먹을 수 없으므로 return false
                    return false;
                }
                wasItPiece = true;
            }
        }

        return true;
        // ------
    }

    // 체크를 제외한 상황에서 가능한 움직임인지를 검증
    private bool IsValidNormalMoveWithoutCheck(Piece piece, (int, int) targetPos)
    {
        if (!Utils.IsInBoard(targetPos) || targetPos == piece.MyPos) return false;

        foreach (var moveInfo in piece.GetMoves())
        {
            if (TryMove(piece, targetPos, moveInfo))
                return true;
        }

        return false;
    }

    private bool IsValidMoveWithoutCheck(Piece piece, (int, int) targetPos) {
        if (IsValidNormalMoveWithoutCheck(piece, targetPos)) {
            return true;
        }
        else if (IsValidCastlingWithoutCheck(piece, targetPos)) {
            return true;
        }
        
        return false;
    }

    // 체크를 포함한 상황에서 가능한 움직임인지를 검증
    public bool IsValidMove(Piece piece, (int, int) targetPos)
    {
        //Debug.Log($"{piece} {targetPos} called");
        if (IsValidCastlingWithoutCheck(piece, targetPos)) {
            var originalKingPos = piece.MyPos;

            var rook = gameManager.Pieces[0, targetPos.Item2];
            var targetRookPos = (originalKingPos.Item1 - 1, originalKingPos.Item2);
            // Debug.Log((0, targetPos.Item2));

            if (targetPos.Item1 >= piece.MyPos.Item1) {
                // Debug.Log((Utils.FieldWidth - 1, targetPos.Item2));
                rook = gameManager.Pieces[Utils.FieldWidth - 1, targetPos.Item2];
                targetRookPos = (originalKingPos.Item1 + 1, originalKingPos.Item2); // king의 과거 위치를 기준으로 rook 위치 지정정
            }
            // Debug.Log((originalKingPos, rook, targetRookPos));

            // foreach (Piece p in gameManager.Pieces) {
            //     if (p) {
            //         Debug.Log((p.MyPos, p));
            //     }
            // }

            var originalRookPos = rook.MyPos;

            
            gameManager.Pieces[targetRookPos.Item1, targetRookPos.Item2] = rook;
            gameManager.Pieces[originalRookPos.Item1, originalRookPos.Item2] = null;
            rook.MyPos = targetRookPos;

            gameManager.Pieces[targetPos.Item1, targetPos.Item2] = piece;
            gameManager.Pieces[originalKingPos.Item1, originalKingPos.Item2] = null;
            piece.MyPos = targetPos;


            bool isValid = !IsInCheck(piece.PlayerDirection);

            // 원상 복구
            gameManager.Pieces[originalKingPos.Item1, originalKingPos.Item2] = piece;
            gameManager.Pieces[targetPos.Item1, targetPos.Item2] = null;
            piece.MyPos = originalKingPos;

            gameManager.Pieces[originalRookPos.Item1, originalRookPos.Item2] = rook;
            gameManager.Pieces[targetRookPos.Item1, targetRookPos.Item2] = null;
            rook.MyPos = originalRookPos;

            // Debug.Log($"{piece.MyPos} to {targetPos}, castling, {isValid}");
            return isValid;
        }
        else if (IsValidNormalMoveWithoutCheck(piece, targetPos)) { // 수정 필요??
            // 체크 상태 검증을 위한 임시 이동
            var originalPiece = gameManager.Pieces[targetPos.Item1, targetPos.Item2];
            var originalPos = piece.MyPos;

            gameManager.Pieces[targetPos.Item1, targetPos.Item2] = piece;
            gameManager.Pieces[originalPos.Item1, originalPos.Item2] = null;
            piece.MyPos = targetPos;

            bool isValid = !IsInCheck(piece.PlayerDirection);

            // 원상 복구
            gameManager.Pieces[originalPos.Item1, originalPos.Item2] = piece;
            gameManager.Pieces[targetPos.Item1, targetPos.Item2] = originalPiece;
            piece.MyPos = originalPos;
            
            // Debug.Log($"{piece.MyPos} to {targetPos}, normal move, {isValid}");
            return isValid;
        }
        // Debug.Log($"{piece.MyPos} to {targetPos}");
        return false;    
    }

    // 체크인지를 확인
    public bool IsInCheck(int playerDirection) //
    {
        (int, int) kingPos = (-1, -1); // 왕의 위치
        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {
                var piece = gameManager.Pieces[x, y];
                if (piece is King && piece.PlayerDirection == playerDirection)
                {
                    kingPos = (x, y);
                    break;
                }
            }
            if (kingPos.Item1 != -1 && kingPos.Item2 != -1) break;
        }

        // 왕이 지금 체크 상태인지를 리턴
        // gameManager.Pieces에서 Piece들을 참조하여 움직임을 확인
        // --- TODO ---
        foreach (Piece piece in gameManager.Pieces) {
            if (piece != null && piece.PlayerDirection != playerDirection) {
                if (IsValidMoveWithoutCheck(piece, kingPos)) { //
                    return true;
                }
            }
        }
        return false;
        // ------
    }

    public bool IsInMate(int playerDirection) { // 움직이면 check인 경우(stalemate, checkmate)
        foreach (Piece piece in gameManager.Pieces) {
            if (piece != null && piece.PlayerDirection == playerDirection) { //
                foreach (MoveInfo move in piece.GetMoves()) {
                    (int x, int y) = piece.MyPos;
                    (int, int) targetPos = (x + move.dirX * move.distance, y + move.dirY * move.distance);
                    if (IsValidMove(piece, targetPos)) {
                        return false;
                    }
                }
            }
        }
        return true;
    }



    public void ShowPossibleMoves(Piece piece)
    {
        ClearEffects();

        // 가능한 움직임을 표시
        // IsValidMove를 사용
        // effectPrefab을 effectParent의 자식으로 생성하고 위치를 적절히 설정
        // currentEffects에 effectPrefab을 추가
        // --- TODO ---
        (int x, int y) = piece.MyPos;
        foreach (MoveInfo move in piece.GetMoves()) {
            (int, int) targetPos = (x + move.dirX * move.distance, y + move.dirY * move.distance);
            if (IsValidMove(piece, targetPos)) {
                GameObject effect = Instantiate(effectPrefab, effectParent);
                effect.transform.localPosition = Utils.ToRealPos(targetPos);
                currentEffects.Add(effect);
            }
        }
        // ------
    }

    // 효과 비우기
    public void ClearEffects()
    {
        foreach (var effect in currentEffects)
        {
            if (effect != null) Destroy(effect);
        }
        currentEffects.Clear();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// King.cs
public class King : Piece
{
    public bool kCastling = true;
    public bool qCastling = true;
    private (bool, bool) state = (true, true);
    MoveInfo[] moveTable;
    public override MoveInfo[] GetMoves()
    {
        // --- TODO ---
        if ((kCastling, qCastling) == state && moveTable != null) return moveTable;

        switch ((kCastling, qCastling)) {
            case (true,true):
                moveTable = new MoveInfo[]
                {
                    new MoveInfo(1,1,1),
                    new MoveInfo(1,0,1),
                    new MoveInfo(1,-1,1),
                    new MoveInfo(0,1,1),
                    new MoveInfo(0,-1,1),
                    new MoveInfo(-1,1,1),
                    new MoveInfo(-1,0,1),
                    new MoveInfo(-1,-1,1),
                    new MoveInfo(2,0,1),
                    new MoveInfo(-2,0,1)
                };
                break;
            case (true,false):
                moveTable = new MoveInfo[]
                {
                    new MoveInfo(1,1,1),
                    new MoveInfo(1,0,1),
                    new MoveInfo(1,-1,1),
                    new MoveInfo(0,1,1),
                    new MoveInfo(0,-1,1),
                    new MoveInfo(-1,1,1),
                    new MoveInfo(-1,0,1),
                    new MoveInfo(-1,-1,1),
                    new MoveInfo(2,0,1)
                };
                break;
            case (false,true):
                moveTable = new MoveInfo[]
                {
                    new MoveInfo(1,1,1),
                    new MoveInfo(1,0,1),
                    new MoveInfo(1,-1,1),
                    new MoveInfo(0,1,1),
                    new MoveInfo(0,-1,1),
                    new MoveInfo(-1,1,1),
                    new MoveInfo(-1,0,1),
                    new MoveInfo(-1,-1,1),
                    new MoveInfo(-2,0,1)
                };
                break;
            case (false,false):
                moveTable = new MoveInfo[]
                {
                    new MoveInfo(1,1,1),
                    new MoveInfo(1,0,1),
                    new MoveInfo(1,-1,1),
                    new MoveInfo(0,1,1),
                    new MoveInfo(0,-1,1),
                    new MoveInfo(-1,1,1),
                    new MoveInfo(-1,0,1),
                    new MoveInfo(-1,-1,1)
                };
                break;
        }
        return moveTable;

        // ------
    }
}

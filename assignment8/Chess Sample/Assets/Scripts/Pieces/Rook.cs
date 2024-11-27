using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rook.cs
public class Rook : Piece
{
    public override MoveInfo[] GetMoves()
    {
        // --- TODO ---
        int limit = (Utils.FieldWidth > Utils.FieldHeight) ? Utils.FieldHeight : Utils.FieldWidth;
        MoveInfo[] ret = new MoveInfo[4*limit];
        for (int i = 0; i < 4; i++) {
            int a = i/2 * 2 - 1;
            int b = i%2 * 2 - 1;
            for (int j = 0; j < limit; j++) {
                ret[i*limit+j] = new MoveInfo(a,b,j);
            }
        }
        return ret;
        // ------
    }
}

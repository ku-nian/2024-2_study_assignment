using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override MoveInfo[] GetMoves()
    {
        // --- TODO ---
        
        // (int x, int y) = MyPos;
        
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
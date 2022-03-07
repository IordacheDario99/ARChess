using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPice
{
    public override bool[,] LegalMove()
    {
        bool[,] target = new bool[8, 8];

        //Up left
        Moves(CurrentXPosition - 1, CurrentYPosition + 2, ref target);

        //Up right
        Moves(CurrentXPosition + 1, CurrentYPosition + 2, ref target);

        //Left up
        Moves(CurrentXPosition - 2, CurrentYPosition + 1, ref target);

        //Right up
        Moves(CurrentXPosition + 2, CurrentYPosition + 1, ref target);

        //Down left
        Moves(CurrentXPosition - 1, CurrentYPosition - 2, ref target);

        //Down right
        Moves(CurrentXPosition + 1, CurrentYPosition - 2, ref target);

        //Down left
        Moves(CurrentXPosition - 2, CurrentYPosition - 1, ref target);

        //Down right
        Moves(CurrentXPosition + 2, CurrentYPosition - 1, ref target);

        return target;

    }

    public void Moves(int x, int y, ref bool[,] target)
    {
        ChessPice cp1;
        if (x < 8 && x >= 0 && y < 8 && y >= 0)
        {
            cp1 = BoardManager.BoardManagerInstance.ChessPices[x, y];
            if (cp1 == null)
            {
                target[x, y] = true;
            }
            else if (isWhite != cp1.isWhite)
            {
                target[x, y] = true;
            }
        }
    }
}

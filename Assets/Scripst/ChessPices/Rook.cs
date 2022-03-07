using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPice
{
    public override bool[,] LegalMove()
    {
        bool[,] target = new bool[8, 8];


        ChessPice cp1;
        int tale;

        //Right
        tale = CurrentXPosition;
        while (true)
        {
            tale++;
            if (tale >= 8)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[tale, CurrentYPosition];
            if (cp1 == null)
            {
                target[tale, CurrentYPosition] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[tale, CurrentYPosition] = true;
                }
                break;
            }


        }

        //Left
        tale = CurrentXPosition;
        while (true)
        {
            tale--;
            if (tale < 0)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[tale, CurrentYPosition];
            if (cp1 == null)
            {
                target[tale, CurrentYPosition] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[tale, CurrentYPosition] = true;
                }
                break;
            }


        }
        //Up

        tale = CurrentYPosition;
        while (true)
        {
            tale++;
            if (tale >= 8)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, tale];
            if (cp1 == null)
            {
                target[CurrentXPosition, tale] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[CurrentXPosition, tale] = true;
                }
                break;
            }


        }
        //Down
        tale = CurrentYPosition;
        while (true)
        {
            tale--;
            if (tale < 0)
            {
                break;
            }
            cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, tale];
            if (cp1 == null)
            {
                target[CurrentXPosition, tale] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[CurrentXPosition, tale] = true;
                }
                break;
            }
        }



        return target;
    }
}

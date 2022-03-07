using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPice
{
    public override bool[,] LegalMove()
    {
        bool[,] target = new bool[8, 8];

        ChessPice cp1;
        int x, y;

        //Right
        x = CurrentXPosition;
        while (true)
        {
            x++;
            if (x >= 8)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[x, CurrentYPosition];
            if (cp1 == null)
            {
                target[x, CurrentYPosition] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[x, CurrentYPosition] = true;
                }
                break;
            }


        }

        //Left
        x = CurrentXPosition;
        while (true)
        {
            x--;
            if (x < 0)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[x, CurrentYPosition];
            if (cp1 == null)
            {
                target[x, CurrentYPosition] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[x, CurrentYPosition] = true;
                }
                break;
            }


        }
        //Up

        x = CurrentYPosition;
        while (true)
        {
            x++;
            if (x >= 8)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, x];
            if (cp1 == null)
            {
                target[CurrentXPosition, x] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[CurrentXPosition, x] = true;
                }
                break;
            }


        }
        //Down
        x = CurrentYPosition;
        while (true)
        {
            x--;
            if (x < 0)
            {
                break;
            }


            cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, x];
            if (cp1 == null)
            {
                target[CurrentXPosition, x] = true;
            }
            else
            {
                if (cp1.isWhite != isWhite)
                {
                    target[CurrentXPosition, x] = true;
                }
                break;
            }


        }


        //Top Left
        x = CurrentXPosition;
        y = CurrentYPosition;

        while (true)
        {
            x--;
            y++;
            if (x < 0 || y >= 8)
            {
                break;
            }
            else
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[x, y];
                if (cp1 == null)
                {
                    target[x, y] = true;
                }
                else
                {
                    if (isWhite != cp1.isWhite)
                    {
                        target[x, y] = true;
                    }
                    break;
                }
            }

        }

        //Top Right
        x = CurrentXPosition;
        y = CurrentYPosition;

        while (true)
        {
            x++;
            y++;
            if (x >= 8 || y >= 8)
            {
                break;
            }
            else
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[x, y];
                if (cp1 == null)
                {
                    target[x, y] = true;
                }
                else
                {
                    if (isWhite != cp1.isWhite)
                    {
                        target[x, y] = true;
                    }
                    break;
                }
            }

        }

        //Down Left
        x = CurrentXPosition;
        y = CurrentYPosition;

        while (true)
        {
            x--;
            y--;
            if (x < 0 || y < 0)
            {
                break;
            }
            else
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[x, y];
                if (cp1 == null)
                {
                    target[x, y] = true;
                }
                else
                {
                    if (isWhite != cp1.isWhite)
                    {
                        target[x, y] = true;
                    }
                    break;
                }
            }

        }

        //Down Right
        x = CurrentXPosition;
        y = CurrentYPosition;

        while (true)
        {
            x++;
            y--;
            if (x >= 8 || y < 0)
            {
                //Debug.LogError("I'm broken" + x + " " + y);
                break;
            }
            else
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[x, y];
                if (cp1 == null)
                {
                    target[x, y] = true;
                }
                else
                {
                    if (isWhite != cp1.isWhite)
                    {
                        target[x, y] = true;
                    }
                    break;
                }
            }

        }





        return target;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPice
{

    public override bool[,] LegalMove()
    {
        bool[,] target = new bool[8, 8];
        //knight
        ChessPice cp1 = BoardManager.BoardManagerInstance.ChessPices[1, CurrentYPosition];
        //bishop
        ChessPice cp2 = BoardManager.BoardManagerInstance.ChessPices[2, CurrentYPosition];
        //queen
        ChessPice cp3 = BoardManager.BoardManagerInstance.ChessPices[3, CurrentYPosition];
        //bishop
        ChessPice cp5 = BoardManager.BoardManagerInstance.ChessPices[5, CurrentYPosition];
        //knight
        ChessPice cp6 = BoardManager.BoardManagerInstance.ChessPices[6, CurrentYPosition];

        //tower
        ChessPice cp0;
        //ChessPice[] cp2 = new ChessPice[8];
        int x, y;
        //incepem din coltul din stanga
        x = CurrentXPosition - 1;
        //cu o pozitie +1 fata de cea initiala pe axa y (adica sus)
        y = CurrentYPosition + 1;
        //daca regele nu se afla pe ultima linie atunci intram in block
        if (CurrentYPosition != 7)
        {

            //Top
            //daca conditia de mai sus este validata, ii crestem valoarea lui x
            //astfel generand inca doua posibile mutari la dreapta
            //acest for are rol de a genera 3 pozitii valide pentru deplasarea regelui
            //de la dreapta la stanga (x++ => x0, x1, x2)
            for (int i = 0; i < 3; i++)
            {
                //daca valoarea lui x este cuprinsa in latimea tablei (x[0,8])
                //atunci trecem la urmatoarele verificari.
                if (x >= 0 || x < 8)
                {
                    //we get hold of a copy of the chess pice that already exists at the given position
                    //ChessPice[x,y] will return the chess pice that is already occupying the tale[x,y]  
                    cp0 = BoardManager.BoardManagerInstance.ChessPices[x, y];
                    //daca nu exista nici o instanta 
                    if (cp0 == null)
                    {
                        target[x, y] = true;
                    }
                    else if (isWhite != cp0.isWhite)
                    {

                        target[x, y] = true;

                    }
                }
                x++;
            }
        }


        x = CurrentXPosition - 1;
        y = CurrentYPosition - 1;
        if (CurrentYPosition != 0)
        {

            //Bottom
            //daca conditia de mai sus este validata, ii crestem valoarea lui x
            //astfel generand inca doua posibile mutari la dreapta
            //acest for are rol de a genera 3 pozitii valide pentru deplasarea regelui
            //de la dreapta la stanga (x++ => x0, x1, x2)
            for (int i = 0; i < 3; i++)
            {
                //daca valoarea lui x este cuprinsa in latimea tablei (x[0,8])
                //atunci trecem la urmatoarele verificari.
                if (x >= 0 || x < 8)
                {
                    //we get hold of a copy of the chess pice that already exists at the given position
                    //ChessPice[x,y] will return the chess pice that is already occupying the tale[x,y]  
                    cp0 = BoardManager.BoardManagerInstance.ChessPices[x, y];
                    //daca nu exista nici o instanta 
                    if (cp0 == null)
                    {
                        target[x, y] = true;
                    }
                    else if (isWhite != cp0.isWhite)
                    {

                        target[x, y] = true;
                    }

                }
                x++;
            }
        }

        //Midle left
        if (CurrentXPosition > 0)
        {
            cp0 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition - 1, CurrentYPosition];
            if (cp0 == null)
            {
                target[CurrentXPosition - 1, CurrentYPosition] = true;
            }
            else if (isWhite != cp0.isWhite)
            {
                target[CurrentXPosition - 1, CurrentYPosition] = true;
            }
        }

        //Midle right
        if (CurrentXPosition < 7)
        {
            cp0 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition + 1, CurrentYPosition];
            if (cp0 == null)
            {
                target[CurrentXPosition + 1, CurrentYPosition] = true;
            }
            else if (isWhite != cp0.isWhite)
            {
                target[CurrentXPosition + 1, CurrentYPosition] = true;
            }
        }

        //Castling for white's
        if (BoardManager.BoardManagerInstance.hasSelectedWhiteKing == false)
        {
            if (isWhite)
            {
                for (int r = 0; r < 8; r++)
                {
                    cp0 = BoardManager.BoardManagerInstance.ChessPices[r, CurrentYPosition];

                    if (cp0 != null && cp0.GetType() == typeof(Rook))
                    {
                        //Castling to the left
                        //I know it's kinda uga buga but that's the fastest way
                        if (cp0.CurrentXPosition == 0 && cp1 == null & cp2 == null & cp3 == null)
                        {
                            target[2, CurrentYPosition] = true;
                            BoardManager.BoardManagerInstance.MoveTower[0] = true;


                        }
                        //Castling to the right
                        else if (cp0.CurrentXPosition == 7 && cp5 == null && cp6 == null)
                        {
                            target[6, CurrentYPosition] = true;
                            BoardManager.BoardManagerInstance.MoveTower[1] = true;
                        }

                    }

                }
            }
        }

        //Castling for black's
        if (BoardManager.BoardManagerInstance.hasSelectedBlackKing == false)
        {
            if (!isWhite)
            {
                for (int r = 0; r < 8; r++)
                {
                    cp0 = BoardManager.BoardManagerInstance.ChessPices[r, CurrentYPosition];

                    if (cp0 != null && cp0.GetType() == typeof(Rook))
                    {
                        //Castling to the left
                        //I know it's kinda uga buga but that's the fastest way
                        if (cp0.CurrentXPosition == 0 && cp1 == null & cp2 == null & cp3 == null)
                        {
                            target[2, CurrentYPosition] = true;
                            BoardManager.BoardManagerInstance.MoveTower[0] = true;


                        }
                        //Castling to the right
                        else if (cp0.CurrentXPosition == 7 && cp5 == null && cp6 == null)
                        {
                            target[6, CurrentYPosition] = true;
                            BoardManager.BoardManagerInstance.MoveTower[1] = true;

                        }

                    }

                }
            }

        }




        return target;
    }
}

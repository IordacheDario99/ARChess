using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPice
{
   
    public override bool[,] LegalMove()
    {
        //daca array-ul este mai mic de 8x8 atunci o sa ne dea o eroare
        //index out of boundries cand vrem sa mutam un pion  pe pozitia mai mare decat cea stabilita (sa zicem 6x6)
        //tale reprezinta patratica verde pe care putem muta piesa
        bool[,] target = new bool[8, 8];
        int[] enpassant = BoardManager.BoardManagerInstance.EnPassant;

        ChessPice cp1, cp2;
        //white team

        if (isWhite)
        {
            //move forward
            if (CurrentYPosition != 7)
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, CurrentYPosition + 1];

                if (cp1 == null)
                {
                    target[CurrentXPosition, CurrentYPosition + 1] = true;

                }
            }

            //move two tales forward
            if (CurrentYPosition == 1)
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, CurrentYPosition + 1];
                cp2 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, CurrentYPosition + 2];
                if (cp1 == null && cp2 == null)
                {
                    target[CurrentXPosition, CurrentYPosition + 2] = true;
                }

            }



            //diagonal left
            //daca pe diagonala se afla un adversar atunci instantiem MoveToPlane pe pozitia adversarului,
            //lucru care ne permite sa mutam pe acea poztie
            if (CurrentXPosition != 0 && CurrentYPosition != 7)
            {
                if (enpassant[0] == CurrentXPosition - 1 && enpassant[1] == CurrentYPosition + 1)
                {
                    cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition - 1, CurrentYPosition + 1];
                    target[CurrentXPosition - 1, CurrentYPosition + 1] = true;

                }
                else
                {
                    //luam piesa care se afla pe pozitia x-1 (stanga) y+1(inainte)
                    cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition - 1, CurrentYPosition + 1];

                    //daca avem o piesa adversar pe diagonala atunci putem muta pe acea pozitie
                    if (cp1 != null && !cp1.isWhite)
                    {
                        //daca pozitia din matricea noastra de bool este true atunci putem instatia acel prefab
                        //aici noi 
                        target[CurrentXPosition - 1, CurrentYPosition + 1] = true;
                    }
                }


            }


            //diagonal right

            if (CurrentXPosition != 7 && CurrentYPosition != 7)
            {
                if (enpassant[0] == CurrentXPosition + 1 && enpassant[1] == CurrentYPosition + 1)
                {
                    
                    Debug.Log("I'm here");
                    target[CurrentXPosition + 1, CurrentYPosition + 1] = true;
                }
                else
                {

                    cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition + 1, CurrentYPosition + 1];

                    if (cp1 != null && !cp1.isWhite)
                    {
                        target[CurrentXPosition + 1, CurrentYPosition + 1] = true;
                    }
                }

            }
          

        }


        //black team
        else
        {
            //move forward
            if (CurrentYPosition != 0)
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, CurrentYPosition - 1];

                if (cp1 == null)
                {
                    target[CurrentXPosition, CurrentYPosition - 1] = true;

                }
            }

            //move two tales forward
            if (CurrentYPosition == 6)
            {
                cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, CurrentYPosition - 1];
                cp2 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition, CurrentYPosition - 2];
                if (cp1 == null && cp2 == null)
                {
                    target[CurrentXPosition, CurrentYPosition - 2] = true;
                }

            }



            //diagonal left
            //daca pe diagonala se afla un adversar atunci instantiem MoveToPlane pe pozitia adversarului,
            //lucru care ne permite sa mutam pe acea poztie
            if (CurrentXPosition != 0 && CurrentYPosition != 0)
            {
                if (enpassant[0] == CurrentXPosition - 1 && enpassant[1] == CurrentYPosition - 1)
                {
                    target[enpassant[0], enpassant[1]] = true;
                }

                //luam piesa care se afla pe pozitia x-1 (stanga) y+1(inainte)
                cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition - 1, CurrentYPosition - 1];

                //daca avem o piesa adversar pe diagonala atunci putem muta pe acea pozitie
                if (cp1 != null && cp1.isWhite)
                {
                    //daca pozitia din matricea noastra de bool este true atunci putem instatia acel prefab
                    //aici noi 
                    target[CurrentXPosition - 1, CurrentYPosition - 1] = true;
                }
            }

            //diagonal right

            if (CurrentXPosition != 7 && CurrentYPosition != 0)
            {
                if (enpassant[0] == CurrentXPosition + 1 && enpassant[1] == CurrentYPosition - 1)
                {
                    Debug.Log("I'm here");
                    target[enpassant[0], enpassant[1]] = true;
                }

                cp1 = BoardManager.BoardManagerInstance.ChessPices[CurrentXPosition + 1, CurrentYPosition - 1];

                if (cp1 != null && cp1.isWhite)
                {
                    target[CurrentXPosition + 1, CurrentYPosition - 1] = true;
                }
            }
        }

        return target;
    }
}

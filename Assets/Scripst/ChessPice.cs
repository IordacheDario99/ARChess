using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPice : MonoBehaviour
{
    //clasa de baza pentru toate pisele de sah (pion, rege etc.)
    //un script abstract nu poate fii atasat unui obiect din scena
    //asa ca trebuie sa mostenim aceasta clasa abstracta ("template")
    public int CurrentXPosition { set; get; }
    public int CurrentYPosition { set; get; }

    public bool isWhite;

    public void SetCurrentPostion(int x, int y )
    {
        CurrentXPosition = x;
        CurrentYPosition = y;
    }
    
    public virtual bool [,] LegalMove()
    {
        return new bool[8, 8];
    }


}

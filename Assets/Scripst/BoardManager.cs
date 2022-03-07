using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager BoardManagerInstance { get; set; }
    private bool[,] _legalMoves { set; get; }


    //creem o propietate lista (list property), lista care este multidimesnionala (bi, 8 linii a cate 8 coloane)
    public ChessPice[,] ChessPices { get; set; } = new ChessPice[8, 8];
    private ChessPice _selectedChessPice;
    private ChessPice _tower;
    public bool isWhiteTurn = true;


    [SerializeField] private float _taleSize = 1.0f;
    //Centrul unui carou = _taleSize / 2;
    //TODO: offsetul trebuie sa fie variabil in functie de marimea jocului
    [SerializeField] private float _taleCenterPoint = 0.5f;
    //private Quaternion orientation = Quaternion.Euler(0, 180, 0);

    private int _XSelector = -1;
    private int _YSelector = -1;

    public List<GameObject> picesPrefabs;
    private List<GameObject> onBoardPices = new List<GameObject>();

    /*private Material _originalMaterial;
    public Material selectedMaterial;*/

    [SerializeField] private ParticleSystem _selectedPiceFX;

    public int[] EnPassant { get; set; }

    public bool hasSelectedWhiteKing = false;
    public bool hasSelectedBlackKing = false;
    public bool[] MoveTower { get; set; }




    // Start is called before the first frame update
    void Start()
    {
        PicesStartPosition();
        BoardManagerInstance = this;

    }

    // Update is called once per frame
    void Update()
    {
        Target();
        


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (_XSelector >= 0 && _YSelector >= 0)
            {
                if (_selectedChessPice == null)
                {
                    //select
                    SelectPice(_XSelector, _YSelector);
                }
                else
                {
                    //move
                    MovePice(_XSelector, _YSelector);
                }
            }

        }
        Debug.LogError("Nu trebuie sa fiu apelata!");
    }

    private void SelectPice(int x, int y)
    {


        //daca array-ul este gol inseamna ca nu exista nici o piesa in zona aleasa
        if (ChessPices[x, y] == null)
        {
            _selectedPiceFX.Stop();
            return;
        }
        //daca este randul albelor si piesa selectata este neagra, atunci iesim din functie (si invers)
        //si astfel se respecta ordinea
        else if (isWhiteTurn != ChessPices[x, y].isWhite)
        {
            _selectedPiceFX.Stop();
            return;
        }
        else
        {
            bool hasAtleastOneMove = false;
            _legalMoves = ChessPices[x, y].LegalMove();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (_legalMoves[i, j])
                    {
                        Debug.Log("one move is true now");
                        hasAtleastOneMove = true;
                    }
                }
            }
            if (!hasAtleastOneMove)
            {
                return;
            }


            _selectedChessPice = ChessPices[x, y];
            Debug.LogError(_selectedChessPice);
            GuidHighlights.GuidHighlightsInstance.HighlightLegalMoves(_legalMoves);



            _selectedPiceFX.transform.position = _selectedChessPice.transform.position + new Vector3(0, 0.1f, 0);
            _selectedPiceFX.Play();


        }

    }

    private void MovePice(int x, int y)
    {
        //daca miscarea este una legala (posibila) atunci intram in block
        if (_legalMoves[x, y] == true)
        {
            ChessPice cp1 = ChessPices[x, y];

            if (cp1 != null && cp1.isWhite != isWhiteTurn)
            {
                //remove enemy chesspice
                _selectedPiceFX.Stop();
                Destroy(cp1.gameObject);
                onBoardPices.Remove(cp1.gameObject);


                //end game if king is targeted
                if (cp1.GetType() == typeof(King))
                {

                    if (isWhiteTurn)
                    {
                        Debug.Log("White wins!");
                    }
                    else
                        Debug.Log("Black wins!");
                    foreach (GameObject go in onBoardPices)
                    {
                        Destroy(go);
                    }
                    isWhiteTurn = true;
                    PicesStartPosition();
                    //end game
                    Debug.LogWarning("End of game");
                    return;
                }



                //play capture sound

            }

            //En Passant
            if (EnPassant[0] == x && EnPassant[1] == y)
            {
                //white team
                if (isWhiteTurn)
                {
                    cp1 = ChessPices[x, y - 1];
                    Destroy(cp1.gameObject);
                    onBoardPices.Remove(cp1.gameObject);
                }
                //black team
                else
                {
                    cp1 = ChessPices[x, y + 1];
                    Destroy(cp1.gameObject);
                    onBoardPices.Remove(cp1.gameObject);
                }
            }

            EnPassant[0] = -1;
            EnPassant[1] = -1;
            if (_selectedChessPice.GetType() == typeof(Pawn))
            {

                //White's Promotion
                if (y == 7)
                {
                    Debug.LogWarning("I'm a Queen !!!");
                    Destroy(_selectedChessPice.gameObject);
                    onBoardPices.Remove(_selectedChessPice.gameObject);
                    SpawnPice(1, x, y, Quaternion.identity);
                    _selectedChessPice = ChessPices[x, y];
                }
                //Black's Promotion
                else if (y == 0)
                {
                    Destroy(_selectedChessPice.gameObject);
                    onBoardPices.Remove(_selectedChessPice.gameObject);
                    SpawnPice(7, x, y, Quaternion.identity);
                    _selectedChessPice = ChessPices[x, y];


                }



                //daca pionul se situeaza pe pozitia 1 (pentru albe)
                //si pozitia selectata pentru mutare este 3 atunci suntem dispusi la enPassant
                if (_selectedChessPice.CurrentYPosition == 1 && y == 3)
                {
                    //retinem pozitiile x si y in array-ul[2]
                    EnPassant[0] = x;
                    // y - 1 deoarece dorim sa optinem patratica pe care trebuie sa ajunga pionul adversar pentru 
                    //a inceplini en passant (adica in spatele piesei)
                    EnPassant[1] = y - 1;
                    Debug.Log(EnPassant[0] + " " + EnPassant[1]);


                }
                else if (_selectedChessPice.CurrentYPosition == 6 && y == 4)
                {
                    //retinem pozitiile x si y in array-ul[2]
                    EnPassant[0] = x;
                    EnPassant[1] = y + 1;

                }
                Debug.Log(EnPassant[0] + " " + EnPassant[1]);
            }

            //Move tower if player chose to do the castling
            if (MoveTower[0] == true)
            {
                _tower = ChessPices[0, _selectedChessPice.CurrentYPosition];
                ChessPices[_tower.CurrentXPosition, _tower.CurrentYPosition] = null;
                _tower.transform.position = CenterPice(3, _selectedChessPice.CurrentYPosition);
                _tower.SetCurrentPostion(3, _selectedChessPice.CurrentYPosition);
                ChessPices[3, _selectedChessPice.CurrentYPosition] = _tower;

                MoveTower[0] = false;
            }
            else if (MoveTower[1] == true)
            {
                _tower = ChessPices[7, _selectedChessPice.CurrentYPosition];
                ChessPices[_tower.CurrentXPosition, _tower.CurrentYPosition] = null;
                _tower.transform.position = CenterPice(5, _selectedChessPice.CurrentYPosition);
                _tower.SetCurrentPostion(3, _selectedChessPice.CurrentYPosition);
                ChessPices[5, _selectedChessPice.CurrentYPosition] = _tower;

                MoveTower[1] = false;

            }




            //se elibereaza poziti initiala a piesei selectate (ii atribuim valoarea null piesei din array-ul multidimensional
            //aflata pe pozitia initiala CurrentXposition si CurrentYPosition)
            ChessPices[_selectedChessPice.CurrentXPosition, _selectedChessPice.CurrentYPosition] = null;
            _selectedChessPice.transform.position = CenterPice(x, y);
            _selectedChessPice.SetCurrentPostion(x, y);
            ChessPices[x, y] = _selectedChessPice;

            //Castling is allowed only if the king (and the rook) was never moved to another position, other than tha start position
            if (_selectedChessPice.GetType() == typeof(King) && _selectedChessPice.isWhite)
            {
                hasSelectedWhiteKing = true;
            }
            else if (_selectedChessPice.GetType() == typeof(King) && !_selectedChessPice.isWhite)
            {
                hasSelectedBlackKing = true;
            }
            //dupa mutarea pisei oprim particles effects
            _selectedPiceFX.Stop();
            //dupa ce am mutat piesa se schimba randul culorii care trebuie sa mute
            isWhiteTurn = !isWhiteTurn;

        }
        else
        {
            _legalMoves[x, y] = false;
        }
        GuidHighlights.GuidHighlightsInstance.DiscardHighlights();

        _selectedChessPice = null;
    }

    private void DrawChessBoard()
    {
        //(a, b, c, d, e, f, g, h)
        Vector3 width = Vector3.right * 8;
        //(1, 2, 3, 4, 5, 6, 7, 8)
        Vector3 length = Vector3.forward * 8;

        //se deseneaza linile orizontale si verticale
        for (int i = 0; i <= 7; i++)
        {
            Vector3 origin = new Vector3(0, 0, 1) * i;
            Debug.DrawLine(origin, origin + width, Color.green);

            //se deseneaza linile verticale
            for (int j = 0; j <= 8; j++)
            {
                origin = new Vector3(1, 0, 0) * j;
                Debug.DrawLine(origin, origin + length, Color.red);
            }
        }

        //desenarea liniilor pentru selectare.
        if (_XSelector >= 0 && _YSelector >= 0)
        {
            //desenam o linie cu punctul de origine din x0,y0 catre x1,y1 (de retinut ca _Xselevtor si _Yselector sunt numere intregi)
            //TODO: pentru scalare cauta sa modifici valoare 1 cu o variabila in care salvezi valoarea cu care se face scalarea
            Debug.DrawLine(
                Vector3.forward * _YSelector + Vector3.right * _XSelector,
                Vector3.forward * (_YSelector + 1) + Vector3.right * (_XSelector + 1),
                Color.black
                );
            Debug.DrawLine(
                Vector3.forward * (_YSelector + 1) + Vector3.right * _XSelector,
                Vector3.forward * _YSelector + Vector3.right * (_XSelector + 1),
                Color.black
                );

        }
    }


    private void Target()
    {
        //mai intai testam disponibilitatea unei camere in scena
        //daca nu dispunem de o camera atunci nu putem face proiectarea razei in planul grafic
        if (!Camera.main)
        {
            //daca nu se gaseste nici o camera se afiseaza in log urmatorul string si se iese din functie
            Debug.Log("No main camera available");
            return;
        }

        //se verifica daca ecranul este atins cu un singru deget
        if (Input.touchCount == 1)
        {
            //se creeaza un obiect de tip RaycastHit
            RaycastHit hitInfo;
            //se creeaza un obiect de tip ray care primeste ca valoare coordonatele pozitionarii degetului pe ecran
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            //daca raza proiectata loveste un obiect cu layer mask-ul "ChessBoard" se intra in block.
            if (Physics.Raycast(ray, out hitInfo, 50.0f, LayerMask.GetMask("ChessBoard")))
            {
                Debug.DrawLine(ray.origin, ray.direction * 10, Color.blue);
                //Debug.Log(hitInfo.point);
                //se retin coordonatele punctului de intersectie a razei cu planul (chess board) in _XSelector si _YSelector
                _XSelector = (int)hitInfo.point.x;
                _YSelector = (int)hitInfo.point.z;
            }
            else
            {
                _XSelector = -1;
                _XSelector = -1;
            }

        }


    }


    private void SpawnPice(int indexValue, int x, int y, Quaternion orientation)
    {
        GameObject pice = Instantiate(picesPrefabs[indexValue], CenterPice(x, y), orientation);
        pice.transform.SetParent(transform);
        onBoardPices.Add(pice);
        ChessPices[x, y] = pice.GetComponent<ChessPice>();
        ChessPices[x, y].SetCurrentPostion(x, y);
        //Debug.Log(ChessPices[0, 0] + " " + x + " " + y + "@@@@");
    }


    public Vector3 CenterPice(int x, int y)
    {
        Vector3 origin = new Vector3(0, 0, 0);
        origin.x = (_taleSize * x) + _taleCenterPoint;
        origin.z = (_taleSize * y) + _taleCenterPoint;

        return origin;
    }


    private void PicesStartPosition()
    {
        _selectedPiceFX.Stop();
        EnPassant = new int[2] { -1, -1 };
        MoveTower = new bool[2] { false, false };


        //Spawn white king
        SpawnPice(0, 4, 0, Quaternion.identity);

        //Spawn white queen
        SpawnPice(1, 3, 0, Quaternion.identity);

        //Spawn white rook
        SpawnPice(2, 0, 0, Quaternion.identity);
        SpawnPice(2, 7, 0, Quaternion.identity);

        //Spawn white bishop
        SpawnPice(3, 2, 0, Quaternion.identity);
        SpawnPice(3, 5, 0, Quaternion.identity);

        //Spawn white knight
        SpawnPice(4, 1, 0, Quaternion.identity);
        SpawnPice(4, 6, 0, Quaternion.identity);

        //Spawn white pawn
        for (int i = 0; i <= 7; i++)
            SpawnPice(5, i, 1, Quaternion.identity);

        // BLACK PICES

        //Spawn black king
        SpawnPice(6, 4, 7, Quaternion.identity);

        //Spawn black queen
        SpawnPice(7, 3, 7, Quaternion.identity);

        //Spawn black rook
        SpawnPice(8, 0, 7, Quaternion.identity);
        SpawnPice(8, 7, 7, Quaternion.identity);

        //Spawn black bishop
        SpawnPice(9, 2, 7, Quaternion.identity);
        SpawnPice(9, 5, 7, Quaternion.identity);

        //Spawn black knight
        SpawnPice(10, 1, 7, Quaternion.Euler(0, 180, 0));
        SpawnPice(10, 6, 7, Quaternion.Euler(0, 180, 0));

        //Spawn black pawn
        for (int i = 0; i <= 7; i++)
            SpawnPice(11, i, 6, Quaternion.identity);

    }

}

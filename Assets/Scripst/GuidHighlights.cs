using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidHighlights : MonoBehaviour
{
    public static GuidHighlights GuidHighlightsInstance { get; set; }
    public GameObject guidHighlightPrefab;
    private List<GameObject> highlights;

    private void Start()
    {
        GuidHighlightsInstance = this;
        highlights = new List<GameObject>();
    }

    private GameObject GetSquare()
    {
        //cautam in lista<GameObject> de highlights primul obiect care indeplinsete conditia din predicat (!g.activeInHierarchy)
        //(nu este activ in Hierarchy )
        GameObject go = highlights.Find(g => !g.activeInHierarchy);

        if (go == null)
        {
            go = Instantiate(guidHighlightPrefab);
            highlights.Add(go);

        }
        return go;
    }

    public void HighlightLegalMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    GameObject go = GetSquare();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                }
                //Debug.LogError(moves[i, j]);
            }
        }
    }

    public void DiscardHighlights()
    {
        foreach (GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }

}

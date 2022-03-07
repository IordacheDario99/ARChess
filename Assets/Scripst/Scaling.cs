using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 100), "Scaling");
        GUI.Label(new Rect(20, 40, 300, 20), "Position: " + this.transform.position.ToString("F3"));
        GUI.Label(new Rect(20, 60, 300, 20), "Rotation: " + this.transform.rotation.ToString("F3"));
        GUI.Label(new Rect(20, 80, 300, 20), "Scale: " + this.transform.localScale.ToString("F3"));
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchPosition = Input.GetTouch(0).deltaPosition;
            transform.RotateAround(transform.position, new Vector3(0, 1, 0), touchPosition.y * 0.1f);
        }*/
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchPosition = Input.GetTouch(0).deltaPosition;
            transform.localScale = new Vector3(transform.localScale.x + touchPosition.y * 0.001f,
                                                transform.localScale.y + touchPosition.y * 0.001f,
                                                transform.localScale.z + touchPosition.y * 0.001f);
        }
        else if (Input.touchCount == 3 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchPosition.x * 0.00f, 0, -touchPosition.y * 0.00f);

        }
        else if (Input.touchCount == 4 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(0, -touchPosition.y * 0.001f, 0);
        }
    }
}

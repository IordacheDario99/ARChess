using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAngle : MonoBehaviour
{
    GameObject cameraController;
    //Move the camera to either right or left of the board,
    //considering as pivot point the white side view

    private void Start()
    {
        cameraController = GameObject.Find("CameraController");
    }
    public void MoveCameraToLeft()
    {
        cameraController.transform.position = new Vector3(-4, 5, 4);
        cameraController.transform.eulerAngles = new Vector3(35, 90, 0);
    }

    public void MoveCameraToRight()
    {
        cameraController.transform.position = new Vector3(12, 5, 4);
        cameraController.transform.eulerAngles = new Vector3(35, 270, 0);
    }

    public void MoveCameraToTop()
    {
        cameraController.transform.position = new Vector3(4, 11, 4);
        cameraController.transform.eulerAngles = new Vector3(90, -90, 0);
    }

}

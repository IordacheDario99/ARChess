using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadInstantTrackingMode()
    {
        SceneManager.LoadScene(1); // loads the scene with index value 1 (Chess InstantTracking)
    }
    public void LoadImageRecognitionMode()
    {
        SceneManager.LoadScene(2); // Loads the scene with index value 2 (Chess ImageRecognition)
    }
    public void LoadClassicMode()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); // Loads the main menu scene
    }

}

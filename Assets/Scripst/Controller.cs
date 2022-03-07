using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wikitude;


public class Controller : MonoBehaviour
{
    public InstantTracker iTraker;
    public Button toggleButton;
    public Text toggleButtonText;
    public Text messageBoxText;
    public Text debugger;

    GridRenderer grid;
    public InstantTrackable trakable;

    InstantTrackingState state = InstantTrackingState.Initializing;
    bool isChanging = false;

    void Awake()
    {
        
        grid = GetComponent<GridRenderer>();
        grid.enabled = true;
        //trakable = iTraker.GetComponentInChildren<InstantTrackable>();
        debugger.text = "I'M AWAKEN " + trakable;
    }

    // Start is called before the first frame update
    void Start()
    {
        messageBoxText.text = "Starting Instant Traking";
        /*debugger.text = "I'M IN START ()";*/

    }

    public void ToggleOn()
    {
        if (!isChanging)
        {
            if (state == InstantTrackingState.Initializing)
            {
                if (iTraker.CanStartTracking())
                {
                    toggleButtonText.text = "Switching State";
                    isChanging = true;
                    iTraker.SetState(InstantTrackingState.Tracking);
                    debugger.text = "I CAN TRACK AND I'M INITIALIZED";
                }
            }
            else
            {
                toggleButtonText.text = "Switching State";
                isChanging = true;
                debugger.text = "I'M NOT INITIALIZING";
                iTraker.SetState(InstantTrackingState.Initializing);
            }
        }
        else
        {
            debugger.text = "I CAN'T TOGGLE";
        }
    }

    public void OnItitializationStarted(InstantTarget target)
    {
        SetSceneState(true);
    }
    public void OnItitializationStopped(InstantTarget target)
    {
        SetSceneState(false);
    }


    public void OnSceneRecocnized(InstantTarget target)
    {
        isChanging = true;
        SetSceneState(true);
        messageBoxText.text = "Scene found!";
        Debug.LogError("OnSceneFound");
    }
    public void OnSceneLost(InstantTarget target)
    {
        isChanging = false;
        SetSceneState(false);
        messageBoxText.text = "Scene lost!";
        Debug.LogError("OnSceneLost");
    }

    public void SetSceneState (bool state)
    {
        grid.enabled = state;
    }

    public void ToggleChange(InstantTrackingState newState)
    {
        debugger.text = "I'M IN TOGGLECHANGE";
        state = newState;

        if (state == InstantTrackingState.Initializing)
        {
            toggleButtonText.text = "Start Traking";
            messageBoxText.text = "Not traking";
        }
        else
        {
            toggleButtonText.text = "Stop Traking";
            messageBoxText.text = "Traking";
        }
        isChanging = false;
    }

    public void OnHeightValueChanged(float heightValue)
    {
        iTraker.DeviceHeightAboveGround = heightValue;
    }

    // Update is called once per frame
    void Update()
    {
        //debugger.text = "I'M IN UPDATE";
        if (state == InstantTrackingState.Initializing)
        {
            if (iTraker.CanStartTracking())
            {
                grid.TargetColor = Color.green;
            }
            else
            {
                grid.TargetColor = Color.white;
            }
        }
        else
        {
            grid.TargetColor = Color.blue;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Wikitude;
using System;

using Plane = UnityEngine.Plane;

public class InstantTrackingController : SampleController
{
    public GameObject ButtonDock;
    public GameObject InitializationControls;
    public Text HeightLabel;

    public InstantTracker Tracker;

    public Button ResetButton;

    public List<Button> Buttons;
    public List<GameObject> Models;

    public Text MessageBox;

    public Image ActivityIndicator;

    public Color EnabledColor = new Color(0.2f, 0.75f, 0.2f, 0.8f);
    public Color DisabledColor = new Color(1.0f, 0.2f, 0.2f, 0.8f);

    private MoveController _moveController;
    private GridRenderer _gridRenderer;

    private HashSet<GameObject> _activeModels = new HashSet<GameObject>();
    private InstantTrackingState _currentState = InstantTrackingState.Initializing;
    public InstantTrackingState CurrentState {
        get { return _currentState; }
    }
    private bool _isTracking = false;

    public HashSet<GameObject> ActiveModels {
        get {
            return _activeModels;
        }
    }

    private void Awake() {
        Application.targetFrameRate = 60;

        _moveController = GetComponent<MoveController>();
        _gridRenderer = GetComponent<GridRenderer>();
    }

    protected override void Start() {
        base.Start();
        QualitySettings.shadowDistance = 4.0f;


        MessageBox.text = "Starting the SDK";
        // The Wikitude SDK needs to be fully started before we can query for platform assisted tracking support
        // SDK initialization happens during start, so we wait one frame in a coroutine
        StartCoroutine(CheckPlatformAssistedTrackingSupport());
    }

    private IEnumerator CheckPlatformAssistedTrackingSupport() {
        yield return null;
        if (Tracker.SMARTEnabled) {
            Tracker.IsPlatformAssistedTrackingSupported((SmartAvailability smartAvailability) => {
                UpdateTrackingMessage(smartAvailability);
            });
        }
    }

    private void UpdateTrackingMessage(SmartAvailability smartAvailability) {
        if (Tracker.SMARTEnabled) {
            string sdk;
            if (Application.platform == RuntimePlatform.Android) {
                sdk = "ARCore";
            } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                sdk = "ARKit";
            } else {
                return;
            }

            switch (smartAvailability) {
                case SmartAvailability.IndeterminateQueryFailed: {
                    MessageBox.text = "Platform support query failed. Running without platform assisted tracking support.";
                    break;
                }
                case SmartAvailability.CheckingQueryOngoing: {
                    MessageBox.text = "Platform support query ongoing.";
                    break;
                }
                case SmartAvailability.Unsupported: {
                    MessageBox.text = "Running without platform assisted tracking support.";
                    break;
                }
                case SmartAvailability.SupportedUpdateRequired:
                case SmartAvailability.Supported: {
                    string runningWithMessage = "Running with platform assisted tracking support (" + sdk + ").";

                    if (_currentState == InstantTrackingState.Tracking) {
                        MessageBox.text = runningWithMessage;
                    } else {
                        MessageBox.text = runningWithMessage + "\n Move your phone around until the target turns green, which is when you can start tracking.";
                    }
                    break;
                }
            }
        } else {
            MessageBox.text = "Running without platform assisted tracking support.";
        }
    }

    protected override void Update() {
        base.Update();
        if (_currentState == InstantTrackingState.Initializing) {
            if (Tracker.CanStartTracking()) {
                _gridRenderer.TargetColor = Color.green;
            } else {
                _gridRenderer.TargetColor = GridRenderer.DefaultTargetColor;
            }
        } else {
            _gridRenderer.TargetColor = GridRenderer.DefaultTargetColor;
        }
    }

    #region UI Events
    public void OnInitializeButtonClicked() {
        Tracker.SetState(InstantTrackingState.Tracking);
    }

    public void OnHeightValueChanged(float newHeightValue) {
        HeightLabel.text = string.Format("{0:0.##} m", newHeightValue);
        Tracker.DeviceHeightAboveGround = newHeightValue;
    }

    public void OnBeginDrag (int modelIndex) {
        if (_isTracking) {
            // Create object
            GameObject modelPrefab = Models[modelIndex];
            Transform model = Instantiate(modelPrefab).transform;
            _activeModels.Add(model.gameObject);
            // Set model position at touch position
            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane p = new Plane(Vector3.up, Vector3.zero);
            float enter;
            if (p.Raycast(cameraRay, out enter)) {
                model.position = cameraRay.GetPoint(enter);
            }

            // Set model orientation to face toward the camera
            Quaternion modelRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-Camera.main.transform.forward, Vector3.up), Vector3.up);
            model.rotation = modelRotation;

            _moveController.SetMoveObject(model);
        }
    }

    public void OnResetButtonClicked() {
        Tracker.SetState(InstantTrackingState.Initializing);
        ResetButton.gameObject.SetActive(false);
    }
    #endregion

    #region Tracker Events
    public void OnSceneRecognized(InstantTarget target) {
        SetSceneActive(true);
    }

    public void OnSceneLost(InstantTarget target) {
        SetSceneActive(false);
    }

    private void SetSceneActive(bool active) {
        foreach (var button in Buttons) {
            button.interactable = active;
        }

        foreach (var model in _activeModels) {
            model.SetActive(active);
        }

        ActivityIndicator.color = active ? EnabledColor : DisabledColor;

        _gridRenderer.enabled = active;
        _isTracking = active;
    }

    public void OnStateChanged(InstantTrackingState newState) {
        _currentState = newState;
        if (newState == InstantTrackingState.Tracking) {
            if (InitializationControls != null) {
                InitializationControls.SetActive(false);
            }
            ButtonDock.SetActive(true);
            ResetButton.gameObject.SetActive(true);
        } else {
            foreach (var model in _activeModels) {
                Destroy(model);
            }
            _activeModels.Clear();

            if (InitializationControls != null) {
                InitializationControls.SetActive(true);
            }
            ButtonDock.SetActive(false);
        }
    }

    /// <summary>
    /// Used when augmentations are loaded from disk.
    /// Please see SaveInstantTarget and LoadInstantTarget for more information.
    /// </summary>
    internal void LoadAugmentation(AugmentationDescription augmentation) {
        GameObject modelPrefab = Models[augmentation.ID];
        Transform model = Instantiate(modelPrefab).transform;
        _activeModels.Add(model.gameObject);

        model.localPosition = augmentation.LocalPosition;
        model.localRotation = augmentation.LocalRotation;
        model.localScale = augmentation.LocalScale;

        model.gameObject.SetActive(false);
    }

    public void OnError(Error error) {
        PrintError("Instant Tracker error!", error);
    }
    #endregion
}

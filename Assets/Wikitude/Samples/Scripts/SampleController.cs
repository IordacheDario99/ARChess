using UnityEngine;
using UnityEngine.SceneManagement;
using Wikitude;
using System.Text;

public class SampleController : MonoBehaviour
{
    protected virtual void Start() {
        QualitySettings.shadowDistance = 60.0f;
    }

    public virtual void OnBackButtonClicked() {
        SceneManager.LoadScene("Main Menu");
    }

    // URLResource events
    public virtual void OnFinishLoading() {
        Debug.Log("URL Resource loaded successfully.");
    }

    public virtual void OnErrorLoading(Error error) {
        PrintError("Error loading URL Resource!", error);
    }

    // Tracker events
    public virtual void OnTargetsLoaded() {
        Debug.Log("Targets loaded successfully.");
    }

    public virtual void OnErrorLoadingTargets(Error error) {
        PrintError("Error loading targets!", error);
    }

    public virtual void OnCameraError(Error error) {
        PrintError("Camera Error!", error);
    }

    protected virtual void Update() {
        // Also handles the back button on Android
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnBackButtonClicked();
        }
    }

    protected void PrintError(string message, Error error) {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(message);
        AppendError(stringBuilder, error);

        Debug.LogError(stringBuilder.ToString());

        // Adds the error to the log and displays the Error Console
        _errorLog.AppendLine(stringBuilder.ToString());
        _showConsole = true;
    }

    /// <summary>
    /// Handles underlying errors by recursively calling itself
    /// </summary>
    private void AppendError(StringBuilder stringBuilder, Error error) {
        stringBuilder.AppendLine("        Error Code: " + error.Code);
        stringBuilder.AppendLine("        Error Domain: " + error.Domain);
        stringBuilder.AppendLine("        Error Message: " + error.Message);

        if (error.UnderlyingError != null) {
            stringBuilder.AppendLine("    Underlying Error: ");
            AppendError(stringBuilder, error.UnderlyingError);
        }
    }

    // Handles drawing of the Error Console GUI, where all errors are diplayed on the screen to the user.
    #region Error Console GUI

    private StringBuilder _errorLog = new StringBuilder();
    private Vector2 _scrollPosition = Vector2.zero;
    private bool _showConsole = false;

    protected virtual void OnGUI() {
        if (_showConsole) {
            int panelHeight = Screen.height / 5;
            int panelWidth = Screen.width;

            int buttonHeight = 30;
            int buttonWidth = 100;

            GUILayout.BeginArea(new Rect(0, Screen.height - panelHeight - buttonHeight, panelWidth, panelHeight + buttonHeight));
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(2 * buttonWidth));
                    {
                        if (GUILayout.Button("Clear", GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth))) {
                            _errorLog = new StringBuilder();
                        }
                        if (GUILayout.Button("Hide", GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth))) {
                            _showConsole = false;
                        }
                    }
                    GUILayout.EndHorizontal();
                    _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(panelHeight), GUILayout.ExpandHeight(false));
                    {
                        GUILayout.TextArea(_errorLog.ToString(), GUILayout.ExpandHeight(true));
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
    }
    #endregion
}

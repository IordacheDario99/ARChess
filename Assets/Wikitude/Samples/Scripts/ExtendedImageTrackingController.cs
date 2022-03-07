using UnityEngine;
using UnityEngine.UI;
using Wikitude;

public class ExtendedImageTrackingController : SampleController
{
    public ImageTracker Tracker;

    public Text TrackingQualityText;
    public Image TrackingQualityBackground;
    public GameObject StopExtendedTrackingButton;

    protected override void Start() {
        base.Start();
        StopExtendedTrackingButton.SetActive(false);
    }

    public void OnStopExtendedTrackingButtonPressed() {
        Tracker.StopExtendedTracking();
        StopExtendedTrackingButton.SetActive(false);
    }

    public void OnExtendedTrackingQualityChanged(ImageTarget target, ExtendedTrackingQuality oldQuality, ExtendedTrackingQuality newQuality) {
        switch (newQuality) {
        case ExtendedTrackingQuality.Bad:
            TrackingQualityText.text = "Target: " + target.Name + " Quality: Bad";
            TrackingQualityBackground.color = Color.red;
            break;
        case ExtendedTrackingQuality.Average:
            TrackingQualityText.text = "Target: " + target.Name + " Quality: Average";
            TrackingQualityBackground.color = Color.yellow;
            break;
        case ExtendedTrackingQuality.Good:
            TrackingQualityText.text = "Target: " + target.Name + " Quality: Good";
            TrackingQualityBackground.color = Color.green;
            break;
        default:
            break;
        }
    }

    public void OnImageRecognized(ImageTarget target) {
        StopExtendedTrackingButton.SetActive(true);
    }

    public void OnImageLost(ImageTarget target) {
        TrackingQualityText.text = "Target lost";
        TrackingQualityBackground.color = Color.white;
    }
}

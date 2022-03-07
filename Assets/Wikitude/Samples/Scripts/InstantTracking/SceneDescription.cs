using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple struct used to serialize augmentations to disk.
/// </summary>
[System.Serializable]
public struct AugmentationDescription {
    /// <summary>
    /// Corresponds to the index of this augmentation in InstantTrackingController.Models
    /// </summary>
    public int ID;
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;
    public Vector3 LocalScale;

    public AugmentationDescription(int id, Transform transform) {
        ID = id;
        LocalPosition = transform.localPosition;
        LocalRotation = transform.localRotation;
        LocalScale = transform.localScale;
    }
}

[System.Serializable]
public class SceneDescription {
    public List<AugmentationDescription> Augmentations = new List<AugmentationDescription>();
}

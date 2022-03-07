using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Wikitude;
using System.Runtime.InteropServices;

/// <summary>
/// Handles getting the camera frame from the device and forwarding it to the Wikitude SDK.
/// </summary>
public class SimpleInputPluginController : SampleController
{
    public Plugin InputPlugin;
    protected WebCamTexture _feed;

    public const int FrameWidth = 640;
    public const int FrameHeight = 480;

    private int _frameDataSize = 0;
    private int _frameIndex = 0;
    private Color32[] _pixels;
    private bool _cameraReleasedByTheSDK = false;

    public void OnCameraReleased() {
        _cameraReleasedByTheSDK = true;
        StartCoroutine(Initialize());
    }

    public void OnCameraReleaseFailed(Error error) {
        PrintError("Input plugin failed!", error);
    }

    protected IEnumerator Initialize() {
        // Waiting for a frame can help on some devices, especially when initializing the camera when returning from  background
        yield return null;
        WebCamDevice? selectedDevice = null;
        // First search for a back-facing device
        foreach (var device in WebCamTexture.devices) {
            if (!device.isFrontFacing) {
                selectedDevice = device;
                break;
            }
        }

        // If no back-facing device was found, search again for a front facing device
        if (selectedDevice == null) {
            if (WebCamTexture.devices.Length > 0) {
                selectedDevice = WebCamTexture.devices[0];
            }
        }

        if (selectedDevice != null) {
            _feed = new WebCamTexture(selectedDevice.Value.name, FrameWidth, FrameHeight);
            _feed.Play();
        }

        if (_feed == null) {
            Debug.LogError("Could not find any cameras on the device.");
        }

        ResetBuffers(FrameWidth, FrameHeight, 4);

        // Wait a frame before getting the camera rotation, otherwise it might not be initialized yet
        yield return null;
        if (Application.platform == RuntimePlatform.Android) {

            bool rotatedSensor = false;
            switch (Screen.orientation) {
                case ScreenOrientation.Portrait: {
                    rotatedSensor = _feed.videoRotationAngle == 270;
                    break;
                }
                case ScreenOrientation.LandscapeLeft: {
                    rotatedSensor = _feed.videoRotationAngle == 180;
                    break;
                }
                case ScreenOrientation.LandscapeRight: {
                    rotatedSensor = _feed.videoRotationAngle == 0;
                    break;
                }
                case ScreenOrientation.PortraitUpsideDown: {
                    rotatedSensor = _feed.videoRotationAngle == 90;
                    break;
                }
            }

            if (rotatedSensor) {
                // Normally, we use InvertedFrame = true, because textures in Unity are mirrored vertically, when compared with the ones the camera provides.
                // However, when we detect that the camera sensor is rotated by 180 degrees, as is the case for the Nexus 5X for example,
                // We turn off inverted frame and enable mirrored frame, which has the effect of rotating the frame upside down.
                // We use the MirroredFrame property and not the EnableMirroring property because the first one actually changes the data that
                // is being processed, while the second one only changes how the frame is rendered, leaving the frame data intact.

                // WikitudeCam.InvertedFrame = false;
                // WikitudeCam.MirroredFrame = true;
            }
        }
    }

    protected virtual void ResetBuffers(int width, int height, int bytesPerPixel) {
        _frameDataSize = width * height * bytesPerPixel;
        _pixels = new Color32[width * height];
    }

    protected override void Update() {
        base.Update();
        if (_feed == null || !_feed.didUpdateThisFrame) {
            return;
        }

        if (_feed.width < 100 || _feed.height < 100) {
            Debug.LogError("Camera feed has unexpected size.");
            return;
        }

        int newFrameDataSize = _feed.width * _feed.height * 4;
        if (newFrameDataSize != _frameDataSize) {
            ResetBuffers(_feed.width, _feed.height, 4);
        }

        _feed.GetPixels32(_pixels);
        InputPlugin.CameraToSurfaceAngle = (float)_feed.videoRotationAngle;
        SendNewCameraFrame();
    }

    private void SendNewCameraFrame() {
        GCHandle handle = default(GCHandle);
        try {
            handle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);
            IntPtr frameData = handle.AddrOfPinnedObject();

            var metadata = new ColorCameraFrameMetadata();
            metadata.HorizontalFieldOfView = 58.0f;
            metadata.Width = _feed.width;
            metadata.Height = _feed.height;
            metadata.CameraPosition = CaptureDevicePosition.Back;
            metadata.ColorSpace = FrameColorSpace.RGBA;
            metadata.TimestampScale = 1;

            var plane = new CameraFramePlane();
            plane.Data = frameData;
            plane.DataSize = (uint)_frameDataSize;
            plane.PixelStride = 4;
            plane.RowStride = _feed.width;
            var planes = new List<CameraFramePlane>();
            planes.Add(plane);

            var cameraFrame = new CameraFrame(++_frameIndex, 0, metadata, planes);
            InputPlugin.NotifyNewCameraFrame(cameraFrame);
        } finally {
            if (handle != default(GCHandle)) {
                handle.Free();
            }
        }
    }

    protected virtual void Cleanup() {
        _frameDataSize = 0;
        if (_feed != null) {
            _feed.Stop();
            _feed = null;
        }
    }

    private void OnApplicationPause(bool paused) {
        if (paused) {
            Cleanup();
        } else {
            if (_cameraReleasedByTheSDK) {
                // Only attempt to start the camera if the Wikitude SDK already released it.
                // Otherwise simply wait until the Wikitude SDK releases it and the OnCameraRelease callback is called.
                StartCoroutine(Initialize());
            }
        }
    }

    private void OnDestroy() {
        Cleanup();
    }
}

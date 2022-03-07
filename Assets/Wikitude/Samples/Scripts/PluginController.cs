using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using Wikitude;
using ZXing;

public class PluginController : SampleController
{
    public Text ResultText;

    private Thread _backgroundThread;

    private bool _closeBackgroundThread = false;
    private bool _scanningInProgress = false;
    private string _scanningResult = null;
    private int _width = 0;
    private int _height = 0;
    private Color32[] _data = null;

    private readonly object _monitor = new object();

    protected override void Start() {
        base.Start();
        _backgroundThread = new Thread(() => {
            Scan();
        });
        _backgroundThread.Start();
    }

    protected override void Update() {
        base.Update();

        lock(_monitor) {
            if (_scanningInProgress) {
                return;
            }
        }

        if (_scanningResult != null) {
            ResultText.text = _scanningResult;
            _scanningResult = null;
        }
    }

    public void OnCameraFrameAvailable(CameraFrame frame) {
        lock(_monitor) {
            if (_scanningInProgress) {
                return;
            }
        }

        var metadata = frame.ColorMetadata;
        var data = new Color32[metadata.Width * metadata.Height];
        if (metadata.ColorSpace == FrameColorSpace.RGBA) {
            if (frame.ColorData.Count != 1) {
                Debug.LogError("Unexpected number of data planes. Expected 1, but got " + frame.ColorData.Count);
                return;
            }
            var rawBytes = new byte[frame.ColorData[0].DataSize];
            Marshal.Copy(frame.ColorData[0].Data, rawBytes, 0, (int)frame.ColorData[0].DataSize);

            for (int i = 0; i < metadata.Width * metadata.Height; ++i) {
                data[i] = new Color32(rawBytes[i * 4], rawBytes[i * 4 + 1], rawBytes[i * 4 + 2], rawBytes[i * 4 + 3]);
            }
        } else if (metadata.ColorSpace == FrameColorSpace.RGB) {
            if (frame.ColorData.Count != 1) {
                Debug.LogError("Unexpected number of data planes. Expected 1, but got " + frame.ColorData.Count);
                return;
            }

            var rawBytes = new byte[frame.ColorData[0].DataSize];
            Marshal.Copy(frame.ColorData[0].Data, rawBytes, 0, (int)frame.ColorData[0].DataSize);

            for (int i = 0; i < metadata.Width * metadata.Height; ++i) {
                data[i] = new Color32(rawBytes[i * 3], rawBytes[i * 3 + 1], rawBytes[i * 3 + 2], 0);
            }

        } else if (metadata.ColorSpace == FrameColorSpace.YUV_420_NV12 ||
                   metadata.ColorSpace == FrameColorSpace.YUV_420_NV21 ||
                   metadata.ColorSpace == FrameColorSpace.YUV_420_YV12 ||
                   metadata.ColorSpace == FrameColorSpace.YUV_420_888) {

            if (frame.ColorData.Count < 1) {
                Debug.LogError("Unexpected number of data planes. Expected at least 1, but got " + frame.ColorData.Count);
                return;
            }

            var rawBytes = new byte[frame.ColorData[0].DataSize];
            Marshal.Copy(frame.ColorData[0].Data, rawBytes, 0, (int)frame.ColorData[0].DataSize);

            for (int i = 0; i < metadata.Width * metadata.Height; ++i) {
                data[i] = new Color32(rawBytes[i], rawBytes[i], rawBytes[i], 0);
            }
        }

        lock(_monitor) {
            _width = metadata.Width;
            _height = metadata.Height;
            _data = data;
            _scanningInProgress = true;
            Monitor.Pulse(_monitor);
        }
    }

    private void Scan() {
        while (!_closeBackgroundThread) {
            if (_data != null) {
                var result = Decode(_data, _width, _height);
                lock(_monitor) {
                    _scanningResult = result;
                    _data = null;
                    _scanningInProgress = false;
                }
            }

            lock(_monitor) {
                if (_closeBackgroundThread) {
                    return;
                }
                Monitor.Wait(_monitor);
            }
        }
    }

    private string Decode(Color32[] colors, int width, int height) {
        var reader = new BarcodeReader();
        reader.AutoRotate = true;
        var result = reader.Decode(colors, width, height);
        if (result != null) {
            return result.Text;
        } else {
            return null;
        }
    }

    void OnDestroy() {
        lock(_monitor) {
            _closeBackgroundThread = true;
            Monitor.Pulse(_monitor);
        }
        _backgroundThread.Join();
    }

    public void OnPluginFailure(Error error) {
        PrintError("Plugin failed!", error);
    }
}

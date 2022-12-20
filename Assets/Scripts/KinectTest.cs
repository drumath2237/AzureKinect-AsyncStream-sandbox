using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

namespace AzureKinectAsyncStreamSandbox
{
    public class KinectTest : MonoBehaviour
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private SynchronizationContext _synchronizationContext;
        private Texture2D _colorTexture;

        private readonly DeviceConfiguration _deviceConfig = new()
        {
            ColorFormat = ImageFormat.ColorMJPG,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.Off,
            SynchronizedImagesOnly = false,
            CameraFPS = FPS.FPS30,
        };

        [SerializeField] private MeshRenderer colorMeshRenderer;

        private void Start()
        {
            _synchronizationContext = SynchronizationContext.Current;
            _colorTexture = new Texture2D(720, 1280, TextureFormat.BGRA32, false);
            colorMeshRenderer.material.mainTexture = _colorTexture;

            _ = StartCaptureLoop();
        }

        private async Task StartCaptureLoop()
        {
            using var kinect = Device.Open();
            kinect.StartCameras(_deviceConfig);

            await foreach (var capture in kinect.GetAsyncFrameStream(_cancellationTokenSource.Token))
            {
                var jpegData = capture.Color.Memory.ToArray();
                ApplyColorImageInMainThread(jpegData);
                capture.Dispose();
            }

            kinect.StopCameras();
        }

        private void ApplyColorImageInMainThread(byte[] imageData)
        {
            _synchronizationContext.Post(_ =>
            {
                if (_colorTexture == null)
                {
                    return;
                }

                _colorTexture.LoadImage(imageData);
                _colorTexture.Apply();
            }, null);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
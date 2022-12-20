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
        [SerializeField] private Texture2D colorTexture;

        private void Start()
        {
            _synchronizationContext = SynchronizationContext.Current;
            _ = StartCaptureLoop();
        }

        private async Task StartCaptureLoop()
        {
            using var kinect = Device.Open();
            kinect.StartCameras(new DeviceConfiguration
            {
                ColorFormat = ImageFormat.ColorMJPG,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.Off,
                SynchronizedImagesOnly = false,
                CameraFPS = FPS.FPS30,
            });

            await foreach (var capture in kinect.GetAsyncFrameStream(_cancellationTokenSource.Token))
            {
                var jpegData = capture.Color.Memory.ToArray();
                _synchronizationContext.Post(_ =>
                {
                    if (colorTexture == null)
                    {
                        return;
                    }

                    colorTexture.LoadRawTextureData(jpegData);
                    colorTexture.Apply();
                }, null);
                capture.Dispose();
            }
            
            kinect.StopCameras();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
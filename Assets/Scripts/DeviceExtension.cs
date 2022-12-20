using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;

namespace AzureKinectAsyncStreamSandbox
{
    public static class DeviceExtension
    {
        public static async IAsyncEnumerable<Capture> GetAsyncFrameStream(
            this Device device,
            [EnumeratorCancellation] CancellationToken token
        )
        {
            while (!token.IsCancellationRequested)
            {
                var capture = await Task.Run(device.GetCapture, token);
                yield return capture;
            }
        }
    }
}
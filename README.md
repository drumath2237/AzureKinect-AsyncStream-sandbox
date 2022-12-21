# AzureKinect-AsyncStream-sandbox

![img](https://img.shields.io/badge/Unity-2022.2.1-blue)

## About

Unity 2022から利用できる非同期ストリームの仕組みを使って、
Azure Kinect Sensor SDKに拡張メソッドを実装することで
センサーデータをawait foreachで取得できるのか試したプロジェクト。

```csharp
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
```

## Environment

| Env                     |                       |
|:------------------------|:----------------------|
| Unity                   | 2022.2.1              |
| Azure Kinect Sensor SDK | v1.2.0 via UnityNuGet |
| OS                      | Windows 10 Home       |

## Install & Usage

Unityプロジェクトを開いたら`Assets/Scenes/SampleScene.unity`を開きます。
AKDKをつなげた状態で再生すればカラー画像が表示されます。

## Contact

何かございましたら[にー兄さんのTwitter](https://twitter.com/ninisan_drumath)までご連絡ください。
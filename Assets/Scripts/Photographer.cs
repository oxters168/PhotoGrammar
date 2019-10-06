using UnityEngine;
using System.Collections;

public class Photographer : MonoBehaviour
{
    public Texture2D screenCapture;

    public event ImageCapturedHandler onImageCaptured;
    public delegate void ImageCapturedHandler(Color[] colors);

    public bool isCapturing;

    private void Awake()
    {
        screenCapture = new Texture2D(1, 1, TextureFormat.RGB24, false);
    }

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    public void CaptureScreen()
    {
        CaptureScreen(Screen.width, Screen.height);
    }
    public void CaptureScreen(int width, int height)
    {
        isCapturing = true;
        StartCoroutine(CaptureScreenRoutine(width, height));
    }
    private IEnumerator CaptureScreenRoutine(int width, int height)
    {
        yield return frameEnd;

        screenCapture.Resize(width, height);
        screenCapture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenCapture.LoadRawTextureData(screenCapture.GetRawTextureData());
        screenCapture.Apply();

        isCapturing = false;
        onImageCaptured?.Invoke(screenCapture.GetPixels());
    }
}

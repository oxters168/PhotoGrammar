using UnityEngine;
using System.Collections;

public class Photographer : MonoBehaviour
{
    public Camera photographersCamera;
    public Texture2D screenCapture;

    public float secondsPerCapture = 2;
    private float lastCapture;

    private void Awake()
    {
        screenCapture = new Texture2D(1, 1, TextureFormat.RGB24, false);
    }
    private void Update()
    {
        if (Time.time - lastCapture > secondsPerCapture)
        {
            lastCapture = Time.time;
            CaptureScreen(Screen.width, Screen.height);
        }
    }

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    public void CaptureScreen(int width, int height)
    {
        StartCoroutine(CaptureScreenRoutine(width, height));
    }
    private IEnumerator CaptureScreenRoutine(int width, int height)
    {
        yield return frameEnd;

        screenCapture.Resize(width, height);
        screenCapture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenCapture.LoadRawTextureData(screenCapture.GetRawTextureData());
        screenCapture.Apply();
    }
    public Color[] GetCurrentScreenCaptureColors()
    {
        return screenCapture.GetPixels();
    }
}

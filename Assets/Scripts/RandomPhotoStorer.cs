using UnityEngine;
using System.Collections.Generic;

public class RandomPhotoStorer : MonoBehaviour
{
    public Photographer photographer;
    public float minAngle = 10, maxAngle = 45;
    public float totalAngle = 360;
    private float totalAngleAdded;

    public float secondsPerCapture = 2;
    private float lastCapture;
    public int maxImageCapacity = 20;
    private List<Color[]> capturedScreens = new List<Color[]>();

    private void OnEnable()
    {
        photographer.onImageCaptured += Photographer_onImageCaptured;
    }
    private void OnDisable()
    {
        photographer.onImageCaptured -= Photographer_onImageCaptured;
    }
    private void Update()
    {
        if (!photographer.isCapturing && capturedScreens.Count < maxImageCapacity && Time.time - lastCapture > secondsPerCapture)
        {
            lastCapture = Time.time;
            photographer.CaptureScreen();
        }
    }

    private void Photographer_onImageCaptured(Color[] colors)
    {
        capturedScreens.Add(colors);
        RotateByRandomAmount();
    }

    private void RotateByRandomAmount()
    {
        float randomAngle = Random.Range(minAngle, maxAngle);
        randomAngle = Mathf.Min(randomAngle, totalAngle - totalAngleAdded);
        totalAngleAdded += randomAngle;

        transform.rotation = Quaternion.Euler(0, randomAngle, 0) * transform.rotation;
    }
    public Color[] RetrieveAllImagesAsOne()
    {
        List<Color> allImages = new List<Color>();
        for (int i = capturedScreens.Count - 1; i >= 0; i--)
        {
            allImages.AddRange(capturedScreens[i]);
            capturedScreens.RemoveAt(i);
        }
        return allImages.ToArray();
    }
}

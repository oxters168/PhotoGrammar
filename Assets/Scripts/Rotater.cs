using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float minSpeed, maxSpeed;
    public float minXAngle, maxXAngle;

    private float xAngle, yAngle;

    void Update()
    {
        float currentSpeed = Random.Range(minSpeed, maxSpeed);

        xAngle = Mathf.Clamp(xAngle + currentSpeed, minXAngle, maxXAngle);
        yAngle += currentSpeed;
        transform.rotation = Quaternion.Euler(xAngle, yAngle, 0);
    }
}

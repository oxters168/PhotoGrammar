using UnityEngine;
using UnityHelpers;

public class FunctionsTester : MonoBehaviour
{
    public Renderer[] objectsToTestOn;
    public Transform point;

    public bool surfaceTester;
    public bool boundsTester;

    void Update()
    {
        if (surfaceTester)
        {
            foreach (Renderer renderer in objectsToTestOn)
                renderer.material.color = renderer.transform.IsPointOnSurfaceOf(point.position) ? Color.green : Color.red;
        }
    }
    void OnDrawGizmos()
    {
        if (boundsTester)
        {
            foreach (Renderer renderer in objectsToTestOn)
            {
                Bounds currentBounds = renderer.transform.GetBounds();
                Gizmos.color = renderer.transform.HasPointInBounds(point.position) ? Color.green : Color.red;
                Gizmos.DrawWireCube(currentBounds.center, currentBounds.size);
            }
        }
    }
}

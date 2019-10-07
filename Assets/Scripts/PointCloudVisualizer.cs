using UnityEngine;
using UnityHelpers;

public class PointCloudVisualizer : MonoBehaviour
{
    public GameObject pointPrefab;
    public Transform pointsParent;
    private ObjectPool<Transform> pointPool;

    public PhotogramAgent toFollow;

    void Start()
    {
        pointPool = new ObjectPool<Transform>(pointPrefab.transform, 20, false, pointsParent);
    }

    void Update()
    {
        pointPool.ReturnAll();
        foreach (Vector3 goodPoint in toFollow.goodPoints)
        {
            pointPool.Get(null, true, goodPoint);
        }
    }
}

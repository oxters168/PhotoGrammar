using MLAgents;
using UnityEngine;
using System.Collections.Generic;

public class PhotogramAgent : Agent
{
    //public Photographer photographer;
    public GameObject rootBoundsObject;

    public int maxBadPoints = 10, maxRepeatedPoints = 10;
    public List<Vector3> goodPoints = new List<Vector3>();
    public List<Vector3> badPoints = new List<Vector3>();
    public List<Vector3> repeatedPoints = new List<Vector3>();

    private ResetParameters m_ResetParams;

    public override void InitializeAgent()
    {
        //photographer.onImageCaptured += Photographer_onImageCaptured;
        var academy = FindObjectOfType<Academy>();
        m_ResetParams = academy.resetParameters;
    }

    private void Photographer_onImageCaptured(Color[] colors)
    {
        throw new System.NotImplementedException();
    }

    public override void CollectObservations()
    {
        //Nothing is here because a camera is already set in the inspector. Though I'm not sure if it will work with just one camera. It depends on whether the agent can observe multiple screenshots at once while making actions.
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //0 yRotDelta
        //1 xRotDelta
        //2 xPosDelta
        //3 zPosDelta
        //4 pointPosX
        //5 pointPosY
        //6 pointPosZ

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.Continuous)
        {
            transform.Rotate(vectorAction[1], vectorAction[0], 0);
            transform.position += new Vector3(vectorAction[2], 0, vectorAction[3]);
            //if (vectorAction[4] >= 1)
            //{
                Vector3 point = new Vector3(ToOneDecimal(vectorAction[4]), ToOneDecimal(vectorAction[5]), ToOneDecimal(vectorAction[6]));
                var possibleGameObjects = CheckInGameObjectBounds(point);
                if (IsPointOnSurfaceOf(point, possibleGameObjects) != null)
                {
                    if (!goodPoints.Contains(point))
                    {
                        SetReward(0.1f);
                        goodPoints.Add(point);
                    }
                    else
                    {
                        SetReward(-0.2f);
                        repeatedPoints.Add(point);
                    }
                }
                else
                {
                    SetReward(-0.2f);
                    badPoints.Add(point);
                }
            //}
        }

        if (badPoints.Count >= maxBadPoints || repeatedPoints.Count >= maxRepeatedPoints)
        {
            Done();
            SetReward(-1f);
        }
    }

    public override void AgentReset()
    {
        goodPoints.Clear();
        badPoints.Clear();
        repeatedPoints.Clear();

        transform.position = new Vector3(m_ResetParams["x"], m_ResetParams["y"], m_ResetParams["z"]);
        transform.rotation = Quaternion.identity;
    }

    private float ToOneDecimal(float value)
    {
        return ((int)(value * 10)) / 10f;
    }
    private float ToTwoDecimals(float value)
    {
        return ((int)(value * 100)) / 100f;
    }
    private GameObject IsPointOnSurfaceOf(Vector3 point, List<GameObject> gameObjects)
    {
        foreach(GameObject currentObject in gameObjects)
        {
            MeshFilter meshFilter = currentObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                for (int i = 0; i < mesh.triangles.Length; i += 3)
                {
                    Vector3 triangleA = mesh.vertices[mesh.triangles[i + 0]];
                    Vector3 triangleB = mesh.vertices[mesh.triangles[i + 1]];
                    Vector3 triangleC = mesh.vertices[mesh.triangles[i + 2]];

                    if (PointInTriangle(triangleA, triangleB, triangleC, point))
                    {
                        return currentObject;
                    }
                }
            }
        }
        return null;
    }
    private List<GameObject> CheckInGameObjectBounds(Vector3 point)
    {
        List<GameObject> pointPierces = new List<GameObject>();
        
        foreach(Transform t in rootBoundsObject.GetComponentsInChildren<Transform>())
        {
            if (UnityHelpers.BoundsHelpers.Bounds(t).Contains(point))
                pointPierces.Add(t.gameObject);
        }
        return pointPierces;
    }

    private bool PointInTriangle(Vector3 triangleA, Vector3 triangleB, Vector3 triangleC, Vector3 point)
    {
        if (SameSide(point, triangleA, triangleB, triangleC) && SameSide(point, triangleB, triangleA, triangleC) && SameSide(point, triangleC, triangleA, triangleB))
        {
            Vector3 vc1 = Vector3.Cross(triangleA - triangleB, triangleA - triangleC);
            if (Mathf.Abs(Vector3.Dot((triangleA - point).normalized, vc1.normalized)) <= .01f)
                return true;
        }

        return false;
    }
    private bool SameSide(Vector3 p1, Vector3 p2, Vector3 A, Vector3 B)
    {
        Vector3 cp1 = Vector3.Cross(B - A, p1 - A);
        Vector3 cp2 = Vector3.Cross(B - A, p2 - A);
        if (Vector3.Dot(cp1, cp2) >= 0) return true;
        return false;

    }
}

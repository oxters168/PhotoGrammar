using MLAgents;
using UnityEngine;
using System.Collections.Generic;
using UnityHelpers;

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
                var possibleGameObjects = rootBoundsObject.transform.HasPointInTotalBounds(point);
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

    private Transform IsPointOnSurfaceOf(Vector3 point, List<Transform> candidates)
    {
        Transform theOneTrueObject = null;
        foreach (Transform anObject in candidates)
        {
            if (anObject.IsPointOnSurfaceOf(point))
            {
                theOneTrueObject = anObject;
                break;
            }
        }
        return theOneTrueObject;
    }
    private float ToOneDecimal(float value)
    {
        return ((int)(value * 10)) / 10f;
    }
    private float ToTwoDecimals(float value)
    {
        return ((int)(value * 100)) / 100f;
    }
}

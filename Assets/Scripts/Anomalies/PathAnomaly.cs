using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class PathAnomaly : Anomaly
{
    [SerializeField] Transform[] pathPoints;
    private Transform currentPathPoint;
    private int index = 0;
    [SerializeField] float moveSpeed = 2f;
    // The amount by which the PathAnomaly's position has to be within to be considered "at" the next point
    [SerializeField] const float EPSILON = 0.1f;

    public void Start()
    {
        this.currentPathPoint = pathPoints[index];
    }

    public void FixedUpdate()
    {
        if (WithinDistance(this.transform, currentPathPoint))
        {
            SetNextPathPoint();
        }

        else
        {
            this.transform.position += 
        }
    }

    private bool WithinDistance(Transform currentTransform, Transform targetTransform, float epsilon = EPSILON)
    {
        if (
            math.abs(targetTransform.position.x - currentTransform.position.x) < EPSILON &&
            math.abs(targetTransform.position.y - currentTransform.position.y) < EPSILON &&
            math.abs(targetTransform.position.z - currentTransform.position.z) < EPSILON
        )
        {
            return true;
        }

        return false;
    }

    private void SetNextPathPoint()
    {
        index++;

        if (index >= pathPoints.Length)
        {
            index = 0;
        }

        currentPathPoint = pathPoints[index];
    }

    public override void ExecuteHaunt(HauntableObject hauntableObject = null)
    {

    }
}

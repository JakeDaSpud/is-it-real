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

    private void Awake()
    {
        if (pathPoints.Length < 1)
        {
            Debug.LogError($"No Transforms in [{this}]'s pathPoints[]");
            return;
        }
    }

    private void Start()
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
            MoveTowards(currentPathPoint);
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

    private void MoveTowards(Transform target)
    {
        this.transform.position += ((target.position - this.transform.position).normalized * moveSpeed) * Time.deltaTime;
        // normalize(Target Pos - My Pos) * speed
    }

    public override void ExecuteHaunt(HauntableObject hauntableObject = null)
    {
        // Spawn pathPoints

        // Spawn HauntableObject representation of Anomaly (i.e. FloatingSkullSprite, EnemyLookingGuySprite)

        // Start() can continue?
    }
}

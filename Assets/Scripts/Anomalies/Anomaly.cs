using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Anomaly : MonoBehaviour
{
    [Header("Hauntable Objects")]
    public HauntableObject[] hauntableObjects;
    protected HauntableObject currentHauntedObject;

    public void HauntRandom()
    {
        if (hauntableObjects.Length == 0 || hauntableObjects == null)
        {
            Debug.Log($"[{name} has no HauntableObjects to choose from!]");
            return;
        }

        List<HauntableObject> availableObjects = new List<HauntableObject>();

        foreach (HauntableObject obj in hauntableObjects)
        {
            if (obj.hauntingAnomaly == null)
            {
                availableObjects.Add(obj);
            }
        }

        if (availableObjects.Count == 0)
        {
            Debug.Log($"[{name}] could not find any unoccupied HauntableObjects.");
            return;
        }

        int index = UnityEngine.Random.Range(0, hauntableObjects.Length);
        currentHauntedObject = availableObjects[index];
        currentHauntedObject.hauntingAnomaly = this.gameObject;
    }

    /// <summary>
    /// This Anomaly's specific behaviour in a Day.
    /// </summary>
    /// <param name="hauntableObject">The Object this Anomaly is Haunting, otherwise it is Haunting something it will instantiate.</param>
    public abstract void ExecuteHaunt(HauntableObject hauntableObject = null);
}

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Anomaly : MonoBehaviour
{
    [Header("Hauntable Objects")]
    public HauntableObject[] hauntableObjects;
    protected HauntableObject currentHauntedObject;

    [Header("Journal Info")]
    [SerializeField] public String journalPageTitle;
    [SerializeField] public String journalPageEntry;

    public bool HauntRandom()
    {
        if (hauntableObjects.Length == 0 || hauntableObjects == null)
        {
            Debug.Log($"[{name} has no HauntableObjects in hauntableObjects[] to choose from!]");
            return false;
        }

        List<HauntableObject> availableObjects = new List<HauntableObject>();

        foreach (HauntableObject obj in hauntableObjects)
        {
            if (!obj.canBeHaunted)
            {
                Debug.LogError($"[{name}] tried to add [{obj.name}] to availableObjects[] but canBeHaunted is false.");
            }

            else if (obj.hauntingAnomaly == null)
            {
                availableObjects.Add(obj);
            }
        }

        if (availableObjects.Count == 0)
        {
            Debug.Log($"[{name}] could not find any unoccupied HauntableObjects.");
            return false;
        }

        int index = UnityEngine.Random.Range(0, hauntableObjects.Length);
        currentHauntedObject = availableObjects[index];
        currentHauntedObject.hauntingAnomaly = this;
        return true;
    }

    /// <summary>
    /// This Anomaly's specific behaviour in a Day.
    /// </summary>
    /// <param name="hauntableObject">The Object this Anomaly is Haunting, otherwise it is Haunting something it will instantiate.</param>
    public abstract void ExecuteHaunt(HauntableObject hauntableObject = null);
}

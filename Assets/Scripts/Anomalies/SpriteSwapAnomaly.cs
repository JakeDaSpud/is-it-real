using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwapAnomaly : Anomaly
{
    [SerializeField] private HauntableObject swappingHauntableObject;
    [SerializeField] private Sprite hauntedSprite;

    public override void ExecuteHaunt(HauntableObject hauntableObject)
    {
        swappingHauntableObject.GetComponent<SpriteRenderer>().sprite = hauntedSprite;
    }
}

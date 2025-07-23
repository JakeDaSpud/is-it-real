using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwapAnomaly : Anomaly
{
    [SerializeField] private HauntableObject swappingHauntableObject;
    [SerializeField] private Sprite hauntedSprite;

    void FixedUpdate()
    {
        if (swappingHauntableObject.GetComponent<SpriteRenderer>().sprite != hauntedSprite)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }

    public override void ExecuteHaunt(HauntableObject hauntableObject)
    {
        swappingHauntableObject.GetComponent<SpriteRenderer>().sprite = hauntedSprite;
        swappingHauntableObject.hauntingAnomaly = this;
    }
}

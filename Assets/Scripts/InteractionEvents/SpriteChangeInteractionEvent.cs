using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteChangeInteraction", menuName = "InteractionEvent")]
public class SpriteChangeInteractionEvent : InteractionEvent
{
    [SerializeField] public GameObject hauntableObject;
    [SerializeField] public Sprite newSprite;
    [SerializeField] public int newSpriteIndex = -1;

    override public void Interact()
    {
        EventManager.Instance.RaiseSpriteChangeInteraction(hauntableObject.GetComponent<HauntableObject>(), newSprite, newSpriteIndex);
    }
}
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteChangeInteraction", menuName = "InteractionEvent")]
public class SpriteChangeInteractionEvent : InteractionEvent
{
    [SerializeField] public GameObject hauntableObject;
    [SerializeField] public Sprite newSprite;
    [SerializeField] public int newSpriteIndex = -1;
    private bool interactionHappened = false;

    override public void Interact()
    {
        if (!interactionHappened)
        {
            EventManager.Instance.RaiseSpriteChangeInteraction(hauntableObject.GetComponent<HauntableObject>(), newSprite, newSpriteIndex);
            EventManager.Instance.RaiseSuccessfulInteraction(this);
            interactionHappened = true;
        }
    }
}
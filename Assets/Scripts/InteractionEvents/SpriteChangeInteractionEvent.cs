using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteChangeInteraction", menuName = "InteractionEvent")]
public class SpriteChangeInteractionEvent : InteractionEvent
{
    [SerializeField] public GameObject hauntableObject;
    [SerializeField] public Sprite newSprite;
    [SerializeField] public int newSpriteIndex = -1;
    private bool interactionHappened = false;

    void OnEnable()
    {
        EventManager.Instance.OnDayStart += ResetState;
    }

    void OnDisable()
    {
        EventManager.Instance.OnDayStart -= ResetState;
    }
    
    private void ResetState(int dayNumber)
    {
        interactionHappened = false;
    }

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
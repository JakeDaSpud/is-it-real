using System;
using UnityEngine;

public class WorkOnEssayInteractionEvent : InteractionEvent
{
    override public void Interact()
    {
        EventManager.Instance.RaiseWriteEssayInteraction();
        EventManager.Instance.RaiseSuccessfulInteraction(this);
    }
}
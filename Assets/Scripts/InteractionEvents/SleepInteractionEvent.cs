using System;
using UnityEngine;

public class SleepInteractionEvent : InteractionEvent
{
    override public void Interact()
    {
        EventManager.Instance.RaiseSleepInteraction();
    }
}
using System;
using UnityEngine;

public abstract class InteractionEvent : MonoBehaviour
{
    new protected String name = "Default_InteractionEvent";
    public abstract void Interact();
}

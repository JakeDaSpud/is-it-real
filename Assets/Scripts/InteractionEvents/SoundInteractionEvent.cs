using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundInteraction", menuName = "InteractionEvent")]
public class SoundInteractionEvent : InteractionEvent
{
    [SerializeField] public AudioClip sound;

    override public void Interact()
    {
        EventManager.Instance.RaiseSoundInteraction(sound);
    }
}
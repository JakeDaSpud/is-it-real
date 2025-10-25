using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundInteraction", menuName = "InteractionEvent")]
public class SoundInteractionEvent : InteractionEvent
{
    [SerializeField] public AudioManager.SFX sound;

    override public void Interact()
    {
        EventManager.Instance.RaiseSoundInteraction(sound);
    }
}
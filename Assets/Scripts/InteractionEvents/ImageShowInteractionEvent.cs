using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewImageShowInteraction", menuName = "InteractionEvent")]
public class ImageShowInteractionEvent : InteractionEvent
{
    [SerializeField] Sprite currentImage;
    [SerializeField] Sprite[] possibleImages;

    public void ChangeImage(int newSpriteIndex)
    {
        currentImage = possibleImages[newSpriteIndex];
    }

    public void ChangeImage(Sprite newSprite)
    {
        currentImage = newSprite;
    }

    override public void Interact()
    {
        EventManager.Instance.RaiseImageShowInteraction(currentImage);
    }
}
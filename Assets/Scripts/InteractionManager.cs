using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private InteractionEvent[] allInteractionEvents;

    void OnEnable()
    {
        EventManager.Instance.OnSpriteChangeInteraction += SpriteChange;
        EventManager.Instance.OnImageShowInteraction += ImageShow;
        EventManager.Instance.OnSleepInteraction += Sleep;
        EventManager.Instance.OnWriteEssayInteraction += WriteEssay;
    }

    void OnDisable()
    {
        EventManager.Instance.OnSpriteChangeInteraction -= SpriteChange;
        EventManager.Instance.OnImageShowInteraction -= ImageShow;
        EventManager.Instance.OnSleepInteraction -= Sleep;
        EventManager.Instance.OnWriteEssayInteraction -= WriteEssay;
    }

    private void SpriteChange(HauntableObject hauntableObject, Sprite newSprite, int newSpriteIndex)
    {
        if (newSpriteIndex != -1)
        {
            hauntableObject.ChangeSprite(newSpriteIndex);
        }

        else
        {
            hauntableObject.ChangeSprite(newSprite);
        }
    }

    private void ImageShow(Sprite newSprite)
    {

    }

    private void Sleep()
    {
        GameManager.Instance.SleepInBed();
    }
    
    private void WriteEssay()
    {
        GameManager.Instance.WorkOnEssay();
    }
}

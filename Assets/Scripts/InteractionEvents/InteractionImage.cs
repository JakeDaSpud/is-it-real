using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionImage : MonoBehaviour
{
    [SerializeField] private Sprite m_currentImage;
    private SpriteRenderer m_spriteRenderer;

    void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.enabled = false; // Hide the renderer on init
    }

    public void ShowProtocol(Sprite newSprite)
    {
        ChangeSprite(newSprite);
        EventManager.Instance.RaisePause(); // Pause Game
        ShowSprite();
    }

    private void ChangeSprite(Sprite newSprite)
    {
        this.m_currentImage = newSprite;
    }

    public void ShowSprite()
    {
        m_spriteRenderer.enabled = true;
    }
}

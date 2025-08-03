using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractionImage : MonoBehaviour
{
    [SerializeField] private Sprite m_currentImage;
    private Image m_imageRenderer;
    public bool imageActive = false;

    void Awake()
    {
        m_imageRenderer = GetComponent<Image>();
        m_imageRenderer.enabled = false; // Hide the renderer on init
    }

    void OnEnable()
    {
        EventManager.Instance.OnImageShowInteraction += ShowProtocol;
        EventManager.Instance.OnPause += HandlePause;
    }

    void OnDisable()
    {
        EventManager.Instance.OnImageShowInteraction -= ShowProtocol;
        EventManager.Instance.OnPause -= HandlePause;
    }

    public void ShowProtocol(Sprite newSprite)
    {
        if (imageActive)
        {
            HandlePause();
            EventManager.Instance.RaisePause(); // Unpause Game
        }

        else
        {
            EventManager.Instance.RaisePause(); // Pause Game

            ChangeSprite(newSprite);
            ShowSprite();
        }
    }

    private void HandlePause()
    {
        if (imageActive)
        {
            Debug.Log("TURNING OFF IMAGEEEEEEEE");
            TurnOff();
        }
    }

    private void ChangeSprite(Sprite newSprite)
    {
        this.m_currentImage = newSprite;
        m_imageRenderer.sprite = m_currentImage;
    }

    public void ShowSprite()
    {
        m_imageRenderer.enabled = true;
        imageActive = true;
    }

    public void TurnOff()
    {
        m_imageRenderer.enabled = false;
        imageActive = false;
    }
}

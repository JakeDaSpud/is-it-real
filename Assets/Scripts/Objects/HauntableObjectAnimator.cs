using System;
using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

[RequireComponent(typeof(HauntableObject))]
[RequireComponent(typeof(SpriteRenderer))]
public class HauntableObjectAnimator : MonoBehaviour
{
    private HauntableObject m_hauntableObject;
    private SpriteRenderer m_spriteRenderer;
    private Sprite[] sprites;
    private InteractionEvent interactionEvent;

    [Header("Animation Settings")]
    [Tooltip("The index of the frame the Animation should return to once finished.")]
    [SerializeField] private int returnFrameIndex = 0;

    [Tooltip("The index of the Animation's first frame.")]
    [SerializeField] private int firstFrameIndex = -1;

    [Tooltip("The index of the Animation's last frame. Leave as -1 to loop from firstFrameIndex to the last frame in the array.")]
    [SerializeField] private int lastFrameIndex = -1;
    private int currentFrameIndex;

    [Tooltip("How long each frame lasts, in milliseconds.")]
    [SerializeField] private float frameDurationMS = 330;
    private float frameDurationS = 0.33f;

    [Tooltip("The amount of times the Animation should loop AFTER the first time. Leave as -1 to infinitely loop.")]
    [SerializeField] private int loopCount = -1;
    private int loopsLeft;
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private bool canReplayAnimation = false;
    private bool animationPlayed = false;

    void OnEnable()
    {
        EventManager.Instance.OnSuccessfulInteraction += StartAnimation;
        EventManager.Instance.OnDayStart += HandleNewDay;
    }

    void OnDisable()
    {
        EventManager.Instance.OnSuccessfulInteraction -= StartAnimation;
        EventManager.Instance.OnDayStart -= HandleNewDay;
    }

    void Awake()
    {
        if (firstFrameIndex < 0)
        {
            Debug.Log("firstFrameIndex cannot be less than 0!");
            return;
        }

        frameDurationS = frameDurationMS / 1000;
        currentFrameIndex = firstFrameIndex;

        m_hauntableObject = GetComponent<HauntableObject>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        // Request the HauntableObject's sprites[] and interactionEvent
        sprites = m_hauntableObject.GetSprites(this);
        interactionEvent = m_hauntableObject.GetInteractionEvent(this);

        // Figure out where the final frame is
        if (lastFrameIndex < -1 || lastFrameIndex >= sprites.Length)
        {
            Debug.LogError($"[{m_hauntableObject.objectName}]'s lastFrameIndex is out of bounds! Setting to -1 (Playing infinitely).");
            lastFrameIndex = -1;
        }

        if (lastFrameIndex == -1) lastFrameIndex = sprites.Length - 1;
    }

    private void StartAnimation(InteractionEvent raisedInteractionEvent)
    {
        if (isPlaying || (animationPlayed && !canReplayAnimation))
        {
            
        }

        else if (interactionEvent == raisedInteractionEvent)
        {
            loopsLeft = loopCount;
            isPlaying = true;
            animationPlayed = true;
            NextFrame();
        }
    }

    private IEnumerator WaitForFrameDuration()
    {
        yield return new WaitForSeconds(frameDurationS);
        NextFrame();
    }

    private void NextFrame()
    {
        m_spriteRenderer.sprite = sprites[currentFrameIndex];

        currentFrameIndex++;

        if (currentFrameIndex > lastFrameIndex)
        {
            // Is Animation Finished?
            if (loopCount != -1 && loopsLeft == 0)
            {
                FinishAnimation();
                return;
            }

            if (loopsLeft > 0) loopsLeft--;

            currentFrameIndex = firstFrameIndex;
        }

        StartCoroutine(WaitForFrameDuration());
    }

    private void FinishAnimation()
    {
        isPlaying = false;
        animationPlayed = true;
        currentFrameIndex = firstFrameIndex;
        m_spriteRenderer.sprite = sprites[returnFrameIndex];
        EventManager.Instance.RaiseAnimationFinished(m_hauntableObject);
    }
    
    private void HandleNewDay(int dayNumber)
    {
        if (loopCount == -1)
        {
            FinishAnimation();
            animationPlayed = false;
        }

        // This way it won't affect the Essay or Sleeping Animations
        if (!isPlaying)
        {
            loopsLeft = loopCount;
        }
    }
}

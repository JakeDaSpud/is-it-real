using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HauntableObject))]
[RequireComponent(typeof(SpriteRenderer))]
public class HauntableObjectAnimator : MonoBehaviour
{
    private HauntableObject m_hauntableObject;
    private SpriteRenderer m_spriteRenderer;
    private Sprite[] sprites;
    private InteractionEvent interactionEvent;
    private enum AnimationState { NULL, EDITOR_SPRITE, DEFAULT, INTERACTION, REACTION };
    [SerializeField] private AnimationState state = AnimationState.NULL;

    [SerializeField] private bool logStatements = false;

    [Header("General Animation Settings")]
    [Tooltip("How long each frame lasts, in milliseconds.")]
    [SerializeField] private float frameDurationMS = 330;
    private float frameDurationS = 0.33f;
    [SerializeField] private int currentFrameIndex;
    [SerializeField] private int editorDefaultSpriteIndex; // The Sprite that this HauntableObject is set to, from the editor
    private bool isDayChanging = false;

    [Header("Default Animation Settings")]
    [SerializeField] private bool hasDefaultAnimation = false;
    [SerializeField] private int defaultFirstFrameIndex = -1;
    [SerializeField] private int defaultLastFrameIndex = -1;
    [SerializeField] private bool isPlayingDefaultAnimation = false;

    [Header("Interaction Animation Settings")]
    [Tooltip("The index of the frame the Animation should return to once finished.")]
    [SerializeField] private int returnFrameIndex = 0;

    [Tooltip("The index of the Animation's first frame.")]
    [SerializeField] private int firstFrameIndex = -1;

    [Tooltip("The index of the Animation's last frame. Leave as -1 to loop from firstFrameIndex to the last frame in the array.")]
    [SerializeField] private int lastFrameIndex = -1;

    [Tooltip("The amount of times the Animation should loop AFTER the first time. Leave as -1 to infinitely loop.")]
    [SerializeField] private int loopCount = -1;
    private int loopsLeft;
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private bool canReplayAnimation = false;
    [SerializeField] private bool animationPlayed = false;

    [Header("Reaction Animator Trigger")]
    [Tooltip("If this Animation can finish, the Interaction Animation of the specified HauntableObjectAnimator will be played.")]
    [SerializeField] private HauntableObjectAnimator animationToStartWhenFinished;

    void OnEnable()
    {
        EventManager.Instance.OnSuccessfulInteraction += StartInteractionAnimation;
        EventManager.Instance.OnDayStart += HandleNewDay;
    }

    void OnDisable()
    {
        EventManager.Instance.OnSuccessfulInteraction -= StartInteractionAnimation;
        EventManager.Instance.OnDayStart -= HandleNewDay;
    }

    void Awake()
    {
        logStatements = false;
        Setup();
        
        editorDefaultSpriteIndex = System.Array.IndexOf(sprites, m_spriteRenderer.sprite);

        if (hasDefaultAnimation) StartDefaultAnimation();
    }

    private void Setup()
    {
        if (firstFrameIndex < 0)
        {
            Debug.Log("firstFrameIndex cannot be less than 0!");
            return;
        }

        if (hasDefaultAnimation && defaultFirstFrameIndex < 0)
        {
            Debug.Log("defaultFirstFrameIndex cannot be less than 0!");
            return;
        }

        frameDurationS = frameDurationMS / 1000;

        m_hauntableObject = GetComponent<HauntableObject>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} Setup()");

        // Request the HauntableObject's sprites[] and interactionEvent
        sprites = m_hauntableObject.GetSprites(this);
        interactionEvent = m_hauntableObject.GetInteractionEvent(this);

        // Figure out where the final frame is
        if (lastFrameIndex < -1 || lastFrameIndex >= sprites.Length)
        {
            Debug.LogError($"[{m_hauntableObject.objectName}]'s lastFrameIndex is out of bounds! Setting to -1 (Playing infinitely).");
            lastFrameIndex = -1;
        }

        // Figure out where the final frame is
        if (hasDefaultAnimation && (defaultLastFrameIndex < -1 || defaultLastFrameIndex >= sprites.Length))
        {
            Debug.LogError($"[{m_hauntableObject.objectName}]'s defaultLastFrameIndex is out of bounds! Setting to -1 (Playing infinitely).");
            defaultLastFrameIndex = -1;
        }

        if (lastFrameIndex == -1) lastFrameIndex = sprites.Length - 1;
        if (defaultLastFrameIndex == -1) defaultLastFrameIndex = sprites.Length - 1;
    }

    private void StartDefaultAnimation()
    {
        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} StartDefaultAnimation");

        currentFrameIndex = defaultFirstFrameIndex;
        state = AnimationState.DEFAULT;
        isPlayingDefaultAnimation = true;
        NextDefaultFrame();
    }

    private IEnumerator WaitForDefaultFrame()
    {
        if (state != AnimationState.DEFAULT) yield break;
        yield return new WaitForSeconds(frameDurationS);
        NextDefaultFrame();
    }

    private void NextDefaultFrame()
    {
        if (!isPlayingDefaultAnimation || state != AnimationState.DEFAULT) return;

        m_spriteRenderer.sprite = sprites[currentFrameIndex];

        currentFrameIndex++;

        if (currentFrameIndex > defaultLastFrameIndex)
        {
            currentFrameIndex = defaultFirstFrameIndex;
        }

        StartCoroutine(WaitForDefaultFrame());
    }

    public void StartInteractionAnimationAsReaction()
    {
        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} StartInteractionAnimationAsReaction");
        
        if (isDayChanging) return;

        currentFrameIndex = firstFrameIndex;
        state = AnimationState.INTERACTION;
        isPlayingDefaultAnimation = false;
        loopsLeft = loopCount;
        isPlaying = true;
        animationPlayed = true;
        NextInteractionFrame();
    }

    private void StartInteractionAnimation(InteractionEvent raisedInteractionEvent)
    {
        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} StartInteractionAnimation");
        
        if (isPlaying || (animationPlayed && !canReplayAnimation))
        {
            
        }

        else if (interactionEvent == raisedInteractionEvent)
        {
            currentFrameIndex = firstFrameIndex;
            state = AnimationState.INTERACTION;
            isPlayingDefaultAnimation = false;
            loopsLeft = loopCount;
            isPlaying = true;
            animationPlayed = true;
            NextInteractionFrame();
        }
    }

    private IEnumerator WaitForInteractionFrame()
    {
        yield return new WaitForSeconds(frameDurationS);
        NextInteractionFrame();
    }

    private void NextInteractionFrame()
    {
        if (!isPlaying || state != AnimationState.INTERACTION) return;

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

        StartCoroutine(WaitForInteractionFrame());
    }

    private void FinishAnimation()
    {
        isPlayingDefaultAnimation = false;
        isPlaying = false;
        animationPlayed = true;
        currentFrameIndex = returnFrameIndex;
        m_spriteRenderer.sprite = sprites[returnFrameIndex];

        if (!isDayChanging && animationToStartWhenFinished != null)
        {
        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} FinishAnimation Branch 1");
            state = AnimationState.REACTION;
            animationToStartWhenFinished.StartInteractionAnimationAsReaction();
        }
        
        if (!isDayChanging)
        {        
            if (logStatements) Debug.Log($"{m_hauntableObject.objectName} FinishAnimation Branch 2");
            EventManager.Instance.RaiseAnimationFinished(m_hauntableObject);
        }
    }

    private void HandleNewDay(int dayNumber)
    {
        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} HandleNewDay {dayNumber}");
        
        if (dayNumber == 0) return;

        isDayChanging = true; // Stop Reaction Animations from being triggered

        isPlaying = false;
        isPlayingDefaultAnimation = false;
        state = AnimationState.NULL;

        if (loopCount == -1)
        {
            if (logStatements) Debug.Log($"{m_hauntableObject.objectName} HandleNewDay {dayNumber} Branch 1");
            FinishAnimation();
        }

        // This way it won't affect the Essay or Sleeping Animations
        if (m_hauntableObject.objectName != "Desk" && m_hauntableObject.objectName != "Bed")
        {
            if (logStatements) Debug.Log($"{m_hauntableObject.objectName} HandleNewDay {dayNumber} Branch 2");
            loopsLeft = loopCount;
        }

        if (hasDefaultAnimation)
        {
            if (logStatements) Debug.Log($"{m_hauntableObject.objectName} HandleNewDay {dayNumber} Branch 3");
            currentFrameIndex = defaultFirstFrameIndex;
            StartDefaultAnimation();
        }

        else
        {
            if (logStatements) Debug.Log($"{m_hauntableObject.objectName} HandleNewDay {dayNumber} Branch 4");
            state = AnimationState.EDITOR_SPRITE;
            currentFrameIndex = editorDefaultSpriteIndex;
            if (sprites != null && sprites.Length > currentFrameIndex)
            {
                m_spriteRenderer.sprite = sprites[currentFrameIndex];
            }
        }

        animationPlayed = false;
        StartCoroutine(StopDayChange());
    }
    
    private IEnumerator StopDayChange()
    {
        if (logStatements) Debug.Log($"{m_hauntableObject.objectName} StopDayChange");
        yield return null;
        isDayChanging = false;
    }
}

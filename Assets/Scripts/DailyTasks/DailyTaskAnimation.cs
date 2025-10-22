using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class DailyTaskAnimation : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;
    private Animator m_dailyTaskAnimation;

    [Header("Event Settings")]
    /// The DailyTask that this animation will play after
    [SerializeField] private DailyTask targetDailyTask;
    [SerializeField] private String animationClipName;

    void OnEnable()
    {
        EventManager.Instance.OnDailyTaskCompleted += StartAnimation;
    }

    void OnDisable()
    {
        EventManager.Instance.OnDailyTaskCompleted -= StartAnimation;
    }

    void Awake()
    {
        if (animationClipName == null)
        {
            Debug.Log("animationClipName cannot be null!");
            return;
        }

        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_dailyTaskAnimation = GetComponent<Animator>();
    }

    private void StartAnimation(DailyTask dT)
    {
        if (dT != targetDailyTask || m_dailyTaskAnimation.GetCurrentAnimatorStateInfo(0).IsName(animationClipName)) return;

        m_spriteRenderer.enabled = true;
        m_dailyTaskAnimation.Play(animationClipName, 0, 0f);

        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        var clip = m_dailyTaskAnimation.runtimeAnimatorController.animationClips[0];
        yield return new WaitForSeconds(clip.length);
        FinishAnimation();
    }
    
    private void FinishAnimation()
    {
        m_spriteRenderer.enabled = false;
    }
}

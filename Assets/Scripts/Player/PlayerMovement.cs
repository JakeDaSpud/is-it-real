using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 5f;

    private Transform m_spriteTransform;
    private Vector2 m_movementInput;
    private Rigidbody2D m_rb;
    private InputActions m_inputActions;
    private bool m_canMove = true;

    private void Awake()
    {
        m_spriteTransform = transform.Find("Sprite").transform;
        m_inputActions = new InputActions();
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        m_inputActions.Player.Enable();
        
        m_inputActions.Player.Move.performed += (ctx) => { m_movementInput = ctx.ReadValue<Vector2>(); };
        m_inputActions.Player.Move.canceled += (ctx) => { m_movementInput = Vector2.zero; };
        
        m_inputActions.Player.Pause.performed += (ctx) => { Pause(); };
        m_inputActions.Player.Interact.performed += (ctx) => { TryInteract(); };
        m_inputActions.Player.Interact.performed += (ctx) => { TrySelect(); };
        m_inputActions.Player.HighlightMode.performed += (ctx) => { HighlightMode(); };
    }

    private void OnDisable()
    {
        m_inputActions.Player.Disable();
    }

    private void Pause() {
        Debug.Log("Pause() triggered...");
    }

    private void TryInteract() {
        Debug.Log("TryInteract() triggered...");
    }

    private void HighlightMode()
    {
        GameManager.Instance.ToggleHighlightMode();
    }

    public void TrySelect()
    {
        if (GameManager.Instance.InHighlightMode)
        {
            EventManager.Instance.RaisePlayerLeftClick();
        }
    }

    private void FixedUpdate()
    {
        if (!m_canMove) return;

        // Normalise input to prevent diagonal speed boost
        Vector2 move = m_movementInput.normalized * m_moveSpeed * Time.fixedDeltaTime;

        // Flipping Sprite
        if (move.x < 0 && m_spriteTransform.transform.localScale.x < 0)
        {
            Vector3 scale = m_spriteTransform.transform.localScale;
            scale.x *= -1;
            m_spriteTransform.transform.localScale = scale;
        }
        else if (move.x > 0 && m_spriteTransform.transform.localScale.x > 0)
        {
            Vector3 scale = m_spriteTransform.transform.localScale;
            scale.x *= -1;
            m_spriteTransform.transform.localScale = scale;
        }

        // Move using Rigidbody2D (has collision)
        m_rb.MovePosition(m_rb.position + move);
    }

    public void SetCanMove(bool canMove)
    {
        this.m_canMove = canMove;
    }

}

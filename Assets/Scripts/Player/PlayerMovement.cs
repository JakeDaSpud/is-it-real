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

    private void FixedUpdate()
    {
        // Normalise input to prevent diagonal speed boost
        Vector2 move = m_movementInput.normalized * m_moveSpeed * Time.fixedDeltaTime;

        // Flipping Sprite
        if (move.x < 0)
            m_spriteTransform.transform.localScale = new Vector3(-1, 1, 1);
        else if (move.x > 0)
            m_spriteTransform.transform.localScale = new Vector3(1, 1, 1);

        // Move using Rigidbody2D (has collision)
        m_rb.MovePosition(m_rb.position + move);
    }
}

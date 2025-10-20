using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 5f;

    private Transform m_spriteAndInteractBoxTransform;
    private Vector2 m_movementInput;
    private Rigidbody2D m_rb;
    private InputActions m_inputActions;
    private bool m_canMove = true;
    private bool m_shouldRespawn = false;
    private Vector3 m_respawnPosition;
    private float m_lastPositionY = 0;

    [Header("Collision")]
	[Tooltip("The Collider of the Player.")]
	[SerializeField] protected Collider2D m_hitBox;

    [Header("Interact Settings")]
    [SerializeField] private Collider2D m_interactBox;
    [SerializeField] private List<HauntableObject> m_currentlyInteractable = new List<HauntableObject>();

    private void Awake()
    {
        m_spriteAndInteractBoxTransform = transform.Find("Sprite and InteractBox").transform;
        m_inputActions = new InputActions();
        m_rb = GetComponent<Rigidbody2D>();
        m_lastPositionY = m_hitBox.transform.position.y;
    }

    private void OnEnable()
    {
        m_inputActions.Player.Enable();

        m_inputActions.Player.Move.performed += (ctx) => { m_movementInput = ctx.ReadValue<Vector2>(); };
        m_inputActions.Player.Move.canceled += (ctx) => { m_movementInput = Vector2.zero; };
        m_inputActions.Player.Pause.performed += (ctx) => { Pause(); };
        m_inputActions.Player.Interact.performed += (ctx) => { TryInteract(); };
        m_inputActions.Player.HighlightMode.performed += (ctx) => { HighlightMode(); };
        m_inputActions.Player.ToggleJournal.performed += (ctx) => { ToggleJournal(); };
    }

    private void OnDisable()
    {
        m_inputActions.Player.Disable();
    }

    public void Respawn(Vector3 position)
    {
        m_shouldRespawn = true;
        m_respawnPosition = position;
    }

    private void Pause()
    {
        EventManager.Instance.RaisePause();
    }

    private void ToggleJournal()
    {
        Pause();
        EventManager.Instance.RaiseToggleJournal();
    }

    private void TryInteract()
    {
        if (GameManager.Instance.GamePaused)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
        }

        if (GameManager.Instance.InHighlightMode)
        {
            TrySelect();
        }
        else if (m_currentlyInteractable != null)
        {
            Debug.Log($"TryInteract() triggered on {m_currentlyInteractable.Last<HauntableObject>()}");

            /*if (m_currentlyInteractable.Last<HauntableObject>().name == "BedObject")
            {
                GameManager.Instance.SleepInBed();
            }

            else if (m_currentlyInteractable.Last<HauntableObject>().name == "DeskObject")
            {
                GameManager.Instance.WorkOnEssay();
            }

            else
            {*/
                m_currentlyInteractable.Last<HauntableObject>().Interact(m_interactBox);
            //}
        }
        else
        {
            Debug.LogError("TryInteract() triggered with NO HauntableObject.");
        }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            GameManager.Instance.ResetScene();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.FullReset();
        }
    }

    private void FixedUpdate()
    {
        if (m_shouldRespawn)
        {
            this.transform.position = m_respawnPosition;
            this.GetComponent<Rigidbody2D>().position = m_respawnPosition;
            this.GetComponent<Rigidbody2D>().MovePosition(m_respawnPosition);
            m_shouldRespawn = false;
        }

        if (!m_canMove) return;

        // Normalise input to prevent diagonal speed boost
        Vector2 move = m_movementInput.normalized * m_moveSpeed * Time.fixedDeltaTime;

        // Flipping Sprite
        if (move.x < 0 && m_spriteAndInteractBoxTransform.transform.localScale.x < 0)
        {
            Vector3 scale = m_spriteAndInteractBoxTransform.transform.localScale;
            scale.x *= -1;
            m_spriteAndInteractBoxTransform.transform.localScale = scale;
        }
        else if (move.x > 0 && m_spriteAndInteractBoxTransform.transform.localScale.x > 0)
        {
            Vector3 scale = m_spriteAndInteractBoxTransform.transform.localScale;
            scale.x *= -1;
            m_spriteAndInteractBoxTransform.transform.localScale = scale;
        }

        // Move using Rigidbody2D (has collision)
        m_rb.MovePosition(m_rb.position + move);

        // Only update if the position is new
        if (m_hitBox.transform.position.y != m_lastPositionY)
        {
            EventManager.Instance.RaisePlayerMove(m_hitBox.transform.position.y);
            m_lastPositionY = m_hitBox.transform.position.y;
        }
    }

    public void SetCanMove(bool canMove)
    {
        this.m_canMove = canMove;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Anomaly")
        {
            Debug.LogError($"You touched a [{collision.gameObject}] Anomaly!");
            GameManager.Instance.FailDay();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HauntableObject>() != null)
        {
            HauntableObject collisionHO = collision.GetComponent<HauntableObject>();

            if (collisionHO.canBeInteracted && !this.m_currentlyInteractable.Contains(collisionHO))
            {
                // Reset the colour of the previously last object, if there is one!
                if (this.m_currentlyInteractable.Count() > 0)
                {
                    this.m_currentlyInteractable.Last<HauntableObject>().BecomeCurrent();
                }

                // Add the newest object, change its highlight colour
                this.m_currentlyInteractable.Add(collisionHO);
                //Debug.Log($"m_currentlyInteractable.Last set to [{collisionHO}].");
                this.m_currentlyInteractable.Last<HauntableObject>().BecomeInteractHighlighted();
            }
        }
        else
        {
            //Debug.Log($"NO HAUNTABLEOBJECT ON [{collision.gameObject}]");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<HauntableObject>() != null)
        {
            HauntableObject collisionHO = collision.GetComponent<HauntableObject>();

            if (this.m_currentlyInteractable.Contains(collision.GetComponent<HauntableObject>()))
            {
                this.m_currentlyInteractable.Last<HauntableObject>().BecomeCurrent();
                this.m_currentlyInteractable.Remove(collisionHO);
                //Debug.Log($"m_currentlyInteractable removed [{collisionHO}].");
            }
        }
    }

}

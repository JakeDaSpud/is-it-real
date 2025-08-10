using System;
using UnityEngine;

public class HauntableObject : MonoBehaviour, ISelectable
{
	[Header("Basic Info")]
	[SerializeField] protected string objectName = "Unnamed_Hauntable_Object";
	[SerializeField] protected bool isActive = true;
	/// <summary>
	/// If true, this HauntedObject will be removed at the start of a new day.
	/// Meant to be used for HOs spawned by Anomalies. Like the FloatingSkull for example.
	/// </summary>
	[SerializeField] protected bool isTemporary = false;

	[Header("Visuals")]
	[SerializeField] private bool hasSprite = true;
	[SerializeField] private bool showSprite = true;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite[] sprites; // sprites[0] is always the default sprite

	[Header("Collision")]
	[Tooltip("The Collider of the Object, where the Player can't walk.")]
	[SerializeField] protected Collider2D hitBox;

	[Tooltip("The Collider that the Player can interact with.")]
	[SerializeField] protected Collider2D interactBox;
	[SerializeField] public bool canBeInteracted = true;
	[SerializeField] protected InteractionEvent interactionEvent;
	[SerializeField] protected DailyTask dailyTask;

	[Header("Haunting")]
	public bool canBeHaunted = false;
	public Anomaly hauntingAnomaly; // null unless haunted!

	private enum SpriteUpdateMode { NONE, PROGRESS, RANDOM_CHANGE }
	[Tooltip("NONE: Sprite will not update. PROGRESS: Sprite will update at the start of each day to the next Sprite. RANDOM_CHANGE: Randomly swap to a DIFFERENT Sprite.")]
	[SerializeField] private SpriteUpdateMode updateMode = SpriteUpdateMode.NONE;
	private int currentSpriteIndex = 0;

	[Header("Selectable")]
	[SerializeField] public bool IsHighlighted = false;
	[SerializeField] public bool IsSelected = false;
	[SerializeField] public bool IsInteractHighlighted = false;
	protected Color m_Original_Colour { get; set; }
	protected Color m_Current_Colour { get; set; }

	void OnEnable()
	{
		EventManager.Instance.OnPlayerLeftClick += TrySuspectOrUnsuspect;
		EventManager.Instance.OnHighlightModeChange += CheckHighlightState;
	}

	void OnDisable()
	{
		EventManager.Instance.OnPlayerLeftClick -= TrySuspectOrUnsuspect;
		EventManager.Instance.OnHighlightModeChange -= CheckHighlightState;
	}

	void Awake()
	{
		if (!isActive)
		{
			this.gameObject.SetActive(false);
			return;
		}

		if (hasSprite)
		{
			m_Original_Colour = spriteRenderer.color;
			m_Current_Colour = m_Original_Colour;
		}

		if (hasSprite && showSprite)
		{
			if (updateMode != SpriteUpdateMode.NONE)
			{
				if (sprites == null)
				{
					throw new System.Exception($"updatingSprite will try to update [{objectName}], but a Sprite[] Array is not assigned.");
				}

				if (sprites.Length < 2)
				{
					throw new System.Exception($"updatingSprite will try to update [{objectName}], but there's no Sprites to switch to.");
				}
			}

			spriteRenderer.sprite = sprites[currentSpriteIndex];
		}

		else
		{
			spriteRenderer.gameObject.SetActive(false);
		}

		if (dailyTask)
		{
			dailyTask.SetHauntableObject(this);
		}
	}

	public void ChangeSprite(int newSpriteIndex)
	{
		Debug.Log($"{this.name}::ChangeSprite[{newSpriteIndex}]");
		spriteRenderer.sprite = sprites[newSpriteIndex];
	}

	public void ChangeSprite(Sprite sprite)
	{
		Debug.Log($"{this.name}::ChangeSprite[{sprite.name}]");
		spriteRenderer.sprite = sprite;
	}

	public void UpdateObject()
	{
		// Default sprite set
		Sprite[] currentSpriteArray = sprites;

		// Check which update option is being used, update accordingly
		if (this.updateMode == SpriteUpdateMode.NONE)
		{
			SpriteUpdateNone(currentSpriteArray);
			return;
		}

		if (this.updateMode == SpriteUpdateMode.PROGRESS)
		{
			SpriteUpdateProgress(currentSpriteArray);
			return;
		}

		if (this.updateMode == SpriteUpdateMode.RANDOM_CHANGE)
		{
			SpriteUpdateRandom(currentSpriteArray);
			return;
		}
	}

	protected void SpriteUpdateNone(Sprite[] sprites)
	{
		// currentSpriteIndex should just stay as 0, when not being an updated sprite!
		spriteRenderer.sprite = sprites[currentSpriteIndex];
	}

	protected void SpriteUpdateProgress(Sprite[] sprites)
	{
		if (currentSpriteIndex >= sprites.Length)
		{
			currentSpriteIndex = sprites.Length - 1;
		}

		else
		{
			currentSpriteIndex++;
		}

		spriteRenderer.sprite = sprites[currentSpriteIndex];

		Debug.Log($"[{objectName}] PROGRESSED Sprite.");
	}

	protected void SpriteUpdateRandom(Sprite[] sprites)
	{
		currentSpriteIndex = UnityEngine.Random.Range(0, sprites.Length);

		spriteRenderer.sprite = sprites[currentSpriteIndex];

		Debug.Log($"[{objectName}] changed Sprite RANDOMLY.");
	}

	public void BecomeTemporary()
	{
		this.isTemporary = true;
	}

	public void Interact(Collider2D other)
	{
		interactionEvent.Interact();

		if (dailyTask && !dailyTask.IsCompleted)
		{
			EventManager.Instance.RaiseDailyTaskCompleted(dailyTask);
		}

		if (other.tag == "Player" && isActive)
		{
			Debug.Log($"Player interacted with [{objectName}].");
		}
	}

	public void OnMouseEnter()
	{
		if (GameManager.Instance.InHighlightMode)
		{
			Debug.Log($"mouse entered {this.name}");
			BecomeHighlighted();
		}
	}

	public void OnMouseExit()
	{
		if (GameManager.Instance.InHighlightMode)
		{
			Debug.Log($"mouse exited {objectName}");
			BecomeCurrent();
		}

		this.IsHighlighted = false;
	}

	public void BecomeOriginal()
	{
		// Bool setting
		this.IsHighlighted = false;
		this.IsSelected = false;

		// Set Colour
		m_Current_Colour = m_Original_Colour;
		BecomeCurrent();
	}

	public void BecomeCurrent()
	{
		this.spriteRenderer.color = m_Current_Colour;
	}

	public void BecomeInteractHighlighted()
	{
		this.spriteRenderer.color = GameManager.Instance.InteractHighlighted_Colour;
	}

	public void BecomeHighlighted()
	{
		// Bool setting
		this.IsHighlighted = true;
		Debug.Log($"[{objectName}] highlighted by player.");

		// Set Colour
		this.spriteRenderer.color = GameManager.Instance.Highlighted_Colour;
		EventManager.Instance.RaiseHauntableObjectHighlighted(this);
	}

	public void BecomeSelected()
	{
		// Bool setting
		if (!this.IsHighlighted)
		{
			Debug.LogError($"[{objectName}] NOT highlighted by player, but tried to BecomeSelected().");
			return;
		}
		else
		{
			this.IsSelected = true;
			Debug.Log($"[{objectName}] selected by player.");
		}

		// Set Colour
		m_Current_Colour = GameManager.Instance.Selected_Colour;
		BecomeCurrent();

		EventManager.Instance.RaiseHauntableObjectSelected(this);
	}

	private void TrySuspectOrUnsuspect()
	{
		if (IsHighlighted && !IsSelected)
		{
			if (GameManager.Instance.AddSuspectObject(this))
			{
				BecomeSelected();
			}
		}

		else if (IsHighlighted && IsSelected)
		{
			if (GameManager.Instance.RemoveSuspectObject(this))
			{
				BecomeOriginal();
				BecomeHighlighted();
			}
		}
	}

	private void CheckHighlightState()
	{
		if (!GameManager.Instance.InHighlightMode)
		{
			BecomeCurrent();
		}
	}

	public void ResetObject()
	{
		this.hauntingAnomaly = null;
		this.spriteRenderer.sprite = sprites[0];
		BecomeOriginal();

		// If this is an Anomaly HauntableObject like the FloatingSkullObject, it should remove itself from the scene
		if (isTemporary)
		{
			if (GameManager.Instance.temporaryObjects.Contains(this))
			{
				GameManager.Instance.temporaryObjects.Remove(this);
			}
			Destroy(this.gameObject);
		}
	}
}

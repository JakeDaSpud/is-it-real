using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class HauntableObject : MonoBehaviour, ISelectable
{
	[Header("Basic Info")]
	[SerializeField] protected string objectName = "Unnamed_Hauntable_Object";
	[SerializeField] protected bool isActive = true;

	[Header("Visuals")]
	[SerializeField] private bool hasSprite = true;
	[SerializeField] private bool showSprite = true;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite[] sprites;

	[Header("Collision")]
	[Tooltip("The Collider of the Object, where the Player can't walk.")]
	[SerializeField] protected Collider2D hitBox;

	[Tooltip("The Collider that the Player can interact with.")]
	[SerializeField] protected Collider2D interactBox;

	[Header("Haunting")]
	public bool canBeHaunted = false;
	public Anomaly hauntingAnomaly; // null unless haunted!

	private enum SpriteUpdateMode { NONE, PROGRESS, RANDOM_CHANGE }
	[Tooltip("NONE: Sprite will not update. PROGRESS: Sprite will update at the start of each day to the next Sprite. RANDOM_CHANGE: Randomly swap to a DIFFERENT Sprite.")]
	[SerializeField] private SpriteUpdateMode updateMode = SpriteUpdateMode.NONE;
	private int currentSpriteIndex = 0;

	// Selectable Fields
	protected Material m_Original_Material { get; set; }
	protected Color m_Original_Colour { get; set; }

    public bool IsSelected { get; set ; }
    public bool IsHighlighted { get; set; }

    void Awake()
	{
		if (!isActive)
		{
			this.gameObject.SetActive(false);
			return;
		}

		if (hasSprite)
		{
			m_Original_Material = spriteRenderer.material;
			m_Original_Colour = spriteRenderer.color;
			// TODO
			// - [ ] Have GameManager store the Highlighted and Selected Materials
			/*m_Highlighted_Material = GameManager.Instance.Highlighted_Material;
			m_Selected_Material = GameManager.Instance.Selected_Material;*/
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
	}

	public void UpdateObject()
	{
		// Default sprite set
		Sprite[] currentSpriteArray = sprites;

		// Check which update option is being used, update accordingly
		if (this.updateMode == SpriteUpdateMode.NONE)
		{
			return;
		}

		if (this.updateMode == SpriteUpdateMode.PROGRESS)
		{
			if (currentSpriteIndex >= currentSpriteArray.Length)
			{
				currentSpriteIndex = currentSpriteArray.Length - 1;
			}

			else
			{
				currentSpriteIndex++;
			}

			spriteRenderer.sprite = currentSpriteArray[currentSpriteIndex];

			Debug.Log($"[{objectName}] PROGRESSED Sprite.");
			return;
		}

		if (this.updateMode == SpriteUpdateMode.RANDOM_CHANGE)
		{
			currentSpriteIndex = Random.Range(0, currentSpriteArray.Length);

			spriteRenderer.sprite = currentSpriteArray[currentSpriteIndex];

			Debug.Log($"[{objectName}] changed Sprite RANDOMLY.");
			return;
		}
	}

	public void Interact(Collider2D other)
	{
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
			Debug.Log($"mouse exited {this.name}");
			BecomeOriginal();
		}
	}

	public void BecomeOriginal()
	{
		// Bool setting
		this.IsHighlighted = false;
		this.IsSelected = false;

		// Set Material
		//this.spriteRenderer.SetMaterials(new List<Material> { this.m_Original_Material });
		this.spriteRenderer.color = m_Original_Colour;
    }

	public void BecomeHighlighted()
	{
		// Bool setting
		this.IsHighlighted = true;
		Debug.Log($"[{objectName}] highlighted by player.");

		// Set Material
		//this.spriteRenderer.material = GameManager.Instance.Highlighted_Material;
		this.spriteRenderer.color = GameManager.Instance.Highlighted_Colour;
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

		// Set Material
		//this.spriteRenderer.material = GameManager.Instance.Selected_Material;
		this.spriteRenderer.color = GameManager.Instance.Selected_Colour;

		if (hauntingAnomaly != null)
		{
			Debug.LogWarning($"[{objectName}] is haunted by [{hauntingAnomaly.name}]!");
		}

		// TODO
		// - [ ] Add me to GameManager.Instance.SuspectedObjects[]
		Debug.Log($"This is where [{objectName}] tries to get added to the end-day bestiary to check if it's haunted!");
    }
}

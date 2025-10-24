using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PathAnomaly : Anomaly
{
    [SerializeField] Transform[] pathPoints;
    private Transform currentPathPoint;
    private int index = 0;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Transform hoSpawnTransform;
    /// <summary>
    /// The Anomaly's visual, Highlightable representation.
    /// </summary>
    [SerializeField] HauntableObject hoRepresentation;
    private HauntableObject m_hoRepObject;
    
    [Header("Sound Settings")]
    [SerializeField] AudioManager.SFX repeatingSound;
    [SerializeField] float minimumDurationBetweenSound = 5f;
    [SerializeField] float maxDurationBetweenSound = 10f;
    private float soundTimer;
    private float nextSoundDelay;

    /// <summary>
    /// The amount by which the PathAnomaly's position has to be within to be considered "at" the next point
    /// </summary>
    [SerializeField] const float EPSILON = 0.1f;

    private void Awake()
    {
        if (pathPoints.Length < 1)
        {
            Debug.LogError($"No Transforms in [{this}]'s pathPoints[]");
            return;
        }
    }

    private void Start()
    {
        this.currentPathPoint = pathPoints[index];
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.GamePaused)
        {
            return;
        }

        if (m_hoRepObject == null)
            {
                this.gameObject.SetActive(false);
                return;
            }

        if (WithinDistance(m_hoRepObject.transform, currentPathPoint))
        {
            SetNextPathPoint();
        }

        else
        {
            MoveTowards(currentPathPoint);
        }

        HandleSoundTimer();
    }

    private bool WithinDistance(Transform currentTransform, Transform targetTransform, float epsilon = EPSILON)
    {
        if (
            math.abs(targetTransform.position.x - currentTransform.position.x) < EPSILON &&
            math.abs(targetTransform.position.y - currentTransform.position.y) < EPSILON &&
            math.abs(targetTransform.position.z - currentTransform.position.z) < EPSILON
        )
        {
            return true;
        }

        return false;
    }

    private void SetNextPathPoint()
    {
        index++;

        if (index >= pathPoints.Length)
        {
            index = 0;
        }

        currentPathPoint = pathPoints[index];
    }

    private void MoveTowards(Transform target)
    {
        m_hoRepObject.transform.position += (target.position - m_hoRepObject.transform.position).normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleSoundTimer()
    {
        soundTimer -= Time.deltaTime;

        if (soundTimer <= 0f)
        {
            AudioManager.Instance.PlaySound(repeatingSound);
            ResetSoundTimer();
        }
    }

    private void ResetSoundTimer()
    {
        nextSoundDelay = UnityEngine.Random.Range(minimumDurationBetweenSound, maxDurationBetweenSound);
        soundTimer = nextSoundDelay;
    }

    public override void ExecuteHaunt(HauntableObject hauntableObject = null)
    {
        // Spawn HauntableObject representation of Anomaly (i.e. FloatingSkullSprite, EnemyLookingGuySprite)
        m_hoRepObject = Instantiate(hoRepresentation, hoSpawnTransform);
        m_hoRepObject.hauntingAnomaly = this;
        GameManager.Instance.temporaryObjects.Add(m_hoRepObject);
    }
}

using UnityEngine;

public class NewObjectAnomaly : Anomaly
{
    [SerializeField] Transform newObjectSpawnPoint;
    [SerializeField] HauntableObject newObject;
    [SerializeField] HauntableObject objectToHide;
    HauntableObject m_hoSpawnedObject;

    void FixedUpdate()
    {
        if (m_hoSpawnedObject == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }

    public override void ExecuteHaunt(HauntableObject hauntableObject = null)
    {
        // Spawn HauntableObject
        m_hoSpawnedObject = Instantiate(newObject, newObjectSpawnPoint);
        m_hoSpawnedObject.canBeInteracted = objectToHide.canBeInteracted;
        m_hoSpawnedObject.BecomeTemporary();
        m_hoSpawnedObject.hauntingAnomaly = this;
        if (objectToHide != null)
        {
            objectToHide.gameObject.SetActive(false);
        }
        GameManager.Instance.temporaryObjects.Add(m_hoSpawnedObject);
    }
}

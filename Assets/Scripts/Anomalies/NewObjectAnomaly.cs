using UnityEngine;

public class NewObjectAnomaly : Anomaly
{
    [SerializeField] Transform newObjectSpawnPoint;
    [SerializeField] HauntableObject newObject;
    HauntableObject m_hoSpawnedObject;

    public override void ExecuteHaunt(HauntableObject hauntableObject = null)
    {
        m_hoSpawnedObject = Instantiate(newObject, newObjectSpawnPoint);
        m_hoSpawnedObject.hauntingAnomaly = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private Door otherDoor;
    [SerializeField] private Transform teleportPoint;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && active)
            Teleport(other.GetComponentInParent<PlayerMovement>());
            Debug.Log("Player touch door");
    }

    void Teleport(PlayerMovement player)
    {
        player.GetComponent<Rigidbody2D>().position = new Vector3(otherDoor.teleportPoint.transform.position.x, otherDoor.teleportPoint.transform.position.y, otherDoor.teleportPoint.transform.position.z);
    }
}

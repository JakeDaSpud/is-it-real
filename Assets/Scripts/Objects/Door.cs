using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private bool showDoorSprite = true;
    [SerializeField] private bool isSideDoor = false;
    [SerializeField] private bool isFrontDoor = false;
    [SerializeField] private Door otherDoor;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer mDoorSprite;

    void Awake()
    {
        mDoorSprite = GetComponentInChildren<SpriteRenderer>();
        mDoorSprite.sprite = sprites[0];
    }

    void Start()
    {
        if (!showDoorSprite)
        {
            mDoorSprite.gameObject.SetActive(false);
        }

        if (isSideDoor)
        {
            mDoorSprite.sprite = sprites[1];
        }

        if (isFrontDoor)
        {
            mDoorSprite.sprite = sprites[2];
        }
    }

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

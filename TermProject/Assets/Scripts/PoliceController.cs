using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceFollower : MonoBehaviour
{
    public Transform player;
    public float followDistance = 6f;

    private PlayerController playerController;
    private float currentSpeed;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerController = player.GetComponent<PlayerController>();
            }
        }
        else
        {
            playerController = player.GetComponent<PlayerController>();
        }

        // Initial position
        if (player != null)
        {
            transform.position = player.position - player.forward * followDistance;
            transform.forward = player.forward;
        }
    }

    void Update()
    {
        if (player != null && playerController != null)
        {
            // Match player speed
            currentSpeed = playerController.playerSpeed;

            // Move forward at player's speed
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            // Maintain follow distance
            float currentDistance = Vector3.Distance(transform.position, player.position);
            if (currentDistance < followDistance - 0.5f)
            {
                // Too close, slow down slightly
                transform.Translate(Vector3.forward * -0.5f * Time.deltaTime);
            }
            else if (currentDistance > followDistance + 0.5f)
            {
                // Too far, speed up slightly
                transform.Translate(Vector3.forward * 0.5f * Time.deltaTime);
            }
        }
    }
}
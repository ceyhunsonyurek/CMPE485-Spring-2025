using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float offsetZ = -6f;
    public float offsetY = 3f;
    public float smoothSpeed = 100f;

    private Vector3 targetPosition;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Assign the player transform or tag your player as 'Player'.");
            }
        }
    }

    private void Update()
    {
        if (player != null)
        {
            targetPosition = new Vector3(
                transform.position.x,
                player.position.y + offsetY,
                player.position.z + offsetZ
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }
    }
}
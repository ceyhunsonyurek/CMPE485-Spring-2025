using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeTrap : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the saw blade part that should rotate")]
    public Transform sawBlade;
    public string playerTag = "Player";

    [Header("Movement Settings")]
    [Tooltip("How fast the saw blade moves between sides")]
    public float moveSpeed = 0.3f;
    [Tooltip("Total distance the saw blade travels from one side to the other")]
    public float moveDistance = 11f;

    [Header("Rotation Settings")]
    [Tooltip("How fast the saw blade rotates")]
    public float rotationSpeed = 180.0f; // degrees per second

    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        // Store the initial position as the start position
        startPosition = transform.position;

        // Calculate the end position based on the move distance
        // We'll move along the local forward direction (Z-axis) of the trap home
        endPosition = startPosition + transform.right * moveDistance;

        // Start the movement coroutine
        StartCoroutine(MoveSawBlade());
    }

    void Update()
    {
        // Continuously rotate the saw blade
        if (sawBlade != null)
        {
            sawBlade.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator MoveSawBlade()
    {
        while (true)
        {
            // Move to end position
            yield return StartCoroutine(MoveToPosition(endPosition));

            // Small pause at the end
            yield return new WaitForSeconds(0.1f);

            // Move back to start position
            yield return StartCoroutine(MoveToPosition(startPosition));

            // Small pause at the start
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float journeyLength = Vector3.Distance(transform.position, targetPosition);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, targetPosition, fractionOfJourney);

            yield return null;
        }

        // Ensure we reach exactly the target position
        transform.position = targetPosition;
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with" + collision.gameObject.tag);
        // Check if the saw blade collided with the player
        if (collision.gameObject.CompareTag(playerTag))
        {
            // Get the player's ThirdPersonController component and call the Die method
            ThirdPersonController playerController = collision.gameObject.GetComponent<ThirdPersonController>();
            if (playerController != null)
            {
                playerController.Die();
            }
            else
            {
                Debug.LogWarning("Player object doesn't have ThirdPersonController component attached!");
            }
        }
    }
}
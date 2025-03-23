using System.Collections;
using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Distance to move downward when game ends")]
    public float moveDistance = 15f;

    [Tooltip("Time it takes to complete the movement")]
    public float moveDuration = 1.5f;

    [Tooltip("Slight delay before starting movement")]
    public float startDelay = 0.2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool hasStartedMoving = false;

    void Start()
    {
        // Store the initial position
        startPosition = transform.position;

        // Calculate the target position (10 units down)
        targetPosition = startPosition + Vector3.down * moveDistance;
    }

    void Update()
    {
        // Check if the game is over by looking at the time scale
        // This works because the KeyPhysics script sets Time.timeScale = 0 when the game ends
        if (Time.timeScale == 0 && !hasStartedMoving)
        {
            StartCoroutine(MoveWallDown());
            hasStartedMoving = true;
        }
    }

    // This function can be called directly from other scripts
    public void TriggerWallMovement()
    {
        if (!hasStartedMoving)
        {
            StartCoroutine(MoveWallDown());
            hasStartedMoving = true;
        }
    }

    IEnumerator MoveWallDown()
    {
        // Wait for the specified delay
        yield return new WaitForSecondsRealtime(startDelay);

        // Use real-time since game time might be paused
        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            // Calculate progress
            elapsedTime = Time.realtimeSinceStartup - startTime;
            float percentComplete = elapsedTime / moveDuration;

            // Smoothly interpolate position
            transform.position = Vector3.Lerp(startPosition, targetPosition, percentComplete);

            // Wait for next frame (using realtime since game might be paused)
            yield return null;
        }

        // Ensure we end at exactly the target position
        transform.position = targetPosition;
    }
}
using System.Collections;
using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Points that define the patrol path")]
    public Transform startPoint;
    public Transform endPoint;

    [Tooltip("Movement speed and distance of the guard")]
    public float moveSpeed = 3.0f;
    public float distance = 30.0f;

    [Tooltip("Time to wait at each end of the patrol")]
    public float waitTime = 2.0f;

    [Tooltip("Whether the guard should look in the direction of movement")]
    public bool rotateTowardsMovement = true;

    [Header("Animation")]
    [Tooltip("Optional animator for the guard")]
    public Animator animator;

    [Tooltip("Name of the walking animation parameter")]
    public string walkAnimationParam = "isWalking";

    [Header("Debug")]
    [Tooltip("Draw the patrol path in the editor")]
    public bool showPath = true;
    public Color pathColor = Color.red;

    private bool isMoving = false;

    void Start()
    {
        // If no points are assigned, use the current position as the start point
        if (startPoint == null)
        {
            GameObject start = new GameObject("StartPoint");
            start.transform.position = transform.position;
            startPoint = start.transform;
        }

        // If no end point is assigned, create one 5 units ahead
        if (endPoint == null)
        {
            GameObject end = new GameObject("EndPoint");
            end.transform.position = transform.position + transform.forward * distance;
            endPoint = end.transform;
        }

        // Start the patrol coroutine
        StartCoroutine(PatrolCoroutine());
    }

    IEnumerator PatrolCoroutine()
    {
        // Infinite patrol loop
        while (true)
        {
            // Move to the end point
            yield return StartCoroutine(MoveToPoint(endPoint.position));

            // Wait at the end point
            yield return StartCoroutine(WaitAtPoint());

            // Move back to the start point
            yield return StartCoroutine(MoveToPoint(startPoint.position));

            // Wait at the start point
            yield return StartCoroutine(WaitAtPoint());
        }
    }

    IEnumerator MoveToPoint(Vector3 targetPosition)
    {
        isMoving = true;

        // Set animation state if animator exists
        if (animator != null)
        {
            animator.SetBool(walkAnimationParam, true);
        }

        // Calculate the direction to the target
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0; // Keep on the same Y level

        // Rotate towards movement direction if enabled
        if (rotateTowardsMovement)
        {
            transform.forward = directionToTarget.normalized;
        }

        // While not close enough to the target
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Move towards the target
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Update rotation during movement if enabled
            if (rotateTowardsMovement)
            {
                directionToTarget = targetPosition - transform.position;
                directionToTarget.y = 0;

                if (directionToTarget.magnitude > 0.1f)
                {
                    transform.forward = directionToTarget.normalized;
                }
            }

            yield return null;
        }

        // Ensure we're exactly at the target position
        transform.position = targetPosition;

        isMoving = false;

        // Set animation state if animator exists
        if (animator != null)
        {
            animator.SetBool(walkAnimationParam, false);
        }
    }

    IEnumerator WaitAtPoint()
    {
        // Simply wait for the specified amount of time
        yield return new WaitForSeconds(waitTime);
    }

    // Draw the patrol path in the Scene view
    void OnDrawGizmos()
    {
        if (showPath && startPoint != null && endPoint != null)
        {
            Gizmos.color = pathColor;
            Gizmos.DrawLine(startPoint.position, endPoint.position);

            // Draw spheres at the patrol points
            Gizmos.DrawSphere(startPoint.position, 0.2f);
            Gizmos.DrawSphere(endPoint.position, 0.2f);
        }
    }

    // Public method to check if guard is currently moving
    public bool IsMoving()
    {
        return isMoving;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the saw blade collided with the player
        if (collision.gameObject.CompareTag("Player"))
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public static float playerSpeed;
    public float playerMaxSpeed = 20f;
    public float speedIncreaseInterval = 3f;
    public float speedIncreaseAmount = 0.5f;
    private float timeSinceLastSpeedIncrease = 0f;
    public float horizontalSpeed = 2f;
    public float rightPos = 3.5f;
    public float centerPos = 0f;
    public float leftPos = -3.5f;

    [Header("Jumping Settings")]
    public float jumpForce = 3.5f;
    public float jumpCooldown = 0.8f;

    [Header("Sliding Settings")]
    public float slideTime = 0.7f;
    public float slideCooldown = 0.1f;
    public float slideColliderHeight = 0.5f;

    [Header("References")]
    public Animator animator;
    public CapsuleCollider playerCollider;
    public GameObject police;

    private int currentPos = 1; //0:left 1:center 2:right
    private Rigidbody rb;
    private bool isJumping = false;
    private bool isSliding = false;
    private bool canJump = true;
    private bool canSlide = true;
    private float defaultColliderHeight;
    private Vector3 defaultColliderCenter;
    private float slideTimeCurrent = 0;

    

    private void Start()
    {
        playerSpeed = 10f;
        rb = GetComponent<Rigidbody>();

        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();

        defaultColliderHeight = playerCollider.height;
        defaultColliderCenter = playerCollider.center;
    }

    void Update()
    {
        if (!GameManager.isGameOver)
        {
            HandleSpeedIncrease();
            HandleHorizontalMovement();
            HandleJumpInput();
            HandleSlideInput();

            if (isSliding)
            {
                slideTimeCurrent += Time.deltaTime;

                if (slideTimeCurrent >= slideTime)
                {
                    StopSliding();
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 forwardMovement = transform.forward * playerSpeed;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwardMovement.z);
    }

    private void HandleSpeedIncrease()
    {
        if (playerMaxSpeed > playerSpeed)
        {
            timeSinceLastSpeedIncrease += Time.deltaTime;

            if (timeSinceLastSpeedIncrease >= speedIncreaseInterval)
            {
                playerSpeed += speedIncreaseAmount;
                timeSinceLastSpeedIncrease = 0f;
            }
        }
    }

    private void HandleHorizontalMovement()
    {
        float targetPosition = centerPos;
        if (currentPos == 0)
            targetPosition = leftPos;
        else if (currentPos == 1)
            targetPosition = centerPos;
        else if (currentPos == 2)
            targetPosition = rightPos;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentPos > 0)
            {
                currentPos--;
            }
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentPos < 2)
            {
                currentPos++;
            }
        }

        float currentX = transform.position.x;
        float newX = Mathf.Lerp(currentX, targetPosition, Time.deltaTime * horizontalSpeed);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void HandleJumpInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && canJump && IsGrounded() && !isSliding)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isJumping = true;
        canJump = false;

        StartCoroutine(JumpCooldown());

        if (animator != null)
        {
            animator.Play("Jump");
        }
    }

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
        isJumping = false;
    }

    private void HandleSlideInput()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftControl)) && canSlide && IsGrounded() && !isJumping)
        {
            StartSliding();
        }
    }

    private void StartSliding()
    {
        isSliding = true;
        canSlide = false;
        slideTimeCurrent = 0;

        playerCollider.height = slideColliderHeight;
        playerCollider.center = new Vector3(0, -0.7f, 0);

        if (animator != null)
        {
            animator.Play("Sprinting Forward Roll");
        }
    }

    private void StopSliding()
    {
        isSliding = false;
        playerCollider.height = defaultColliderHeight;
        playerCollider.center = defaultColliderCenter;

        StartCoroutine(SlideCooldown());
    }

    private IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    private bool IsGrounded()
    {
        float groundThreshold = 1.25f;
        float playerY = transform.position.y;
        bool grounded = playerY <= groundThreshold;

        return grounded;
    }

    public void OnCornerTurn(Vector3 newForward)
    {
        transform.forward = newForward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            OnObstacleHit();
        }
    }

    public void OnObstacleHit()
    {
        GameManager.isGameOver = true;
        playerSpeed = 0f;
        police.SetActive(true);
        animator.Play("Stumble Backwards");
    }
}
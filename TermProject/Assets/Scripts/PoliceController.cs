using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoliceController : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject police;
    public Animator animator;
    public Transform player;
    private bool isRunning = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (GameManager.isGameOver && isRunning)
        {
            transform.position = new Vector3(player.position.x + 1.5f, 0, player.position.z - 5);
            isRunning = false;
            animator.Play("Run To Stop");

        }
        else
        {
            if (GameManager.isGameOver)
            {
                rb.velocity = new Vector3(0, 0, 0);
            }
            else
            {
                Vector3 forwardMovement = transform.forward * (PlayerController.playerSpeed - 1);
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwardMovement.z);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!GameManager.isGameOver)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                police.SetActive(false);
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPhysics : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the door object")]
    public string doorTag = "Door";
    public string trapTag = "Trap";

    private bool hasGameEnded = false;

    public GameOverUI gameOverUI;

    private Rigidbody rb;

    void Start()
    {
        // Make sure the key has a Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configure the Rigidbody for proper physics interaction
        rb.mass = 1.0f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }
    }


    IEnumerator Win()
    {
        yield return new WaitForSeconds(2);

        gameOverUI.Setup("You Won!", "Play Again");

        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameOverScene");
        // while (!asyncLoad.isDone) { yield return null; }
    }


    IEnumerator Lose()
    {
        yield return new WaitForSeconds(1);

        gameOverUI.Setup("You Lost!", "Try Again");

        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameOverScene");
        // while (!asyncLoad.isDone) { yield return null; }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the key collided with the door
        if (collision.gameObject.CompareTag(doorTag))
        {
            if (!hasGameEnded)
            {
                hasGameEnded = true;
                Time.timeScale = 0.7f;
                StartCoroutine(Win());
            }
        } else if (collision.gameObject.CompareTag(trapTag))
        {
            if (!hasGameEnded)
            {
                hasGameEnded = true;
                Time.timeScale = 0.8f;
                StartCoroutine(Lose());
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPhysics : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the door object")]
    public string doorTag = "Door";

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


    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);

        gameOverUI.Setup("You Won!", "Play Again");

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
                StartCoroutine(Wait());
            }
        }
    }
}
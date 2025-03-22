using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPhysics : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the door object")]
    public string doorTag = "Door";

    [Tooltip("Optional game over UI to activate")]
    public GameObject gameOverUI;

    [Tooltip("Optional win message text")]
    public UnityEngine.UI.Text winMessageText;

    [Tooltip("Optional sound to play when key hits the door")]
    public AudioSource collisionSound;

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

    void OnCollisionEnter(Collision collision)
    {
        // Check if the key collided with the door
        if (collision.gameObject.CompareTag(doorTag))
        {
            // Play collision sound if available
            if (collisionSound != null)
            {
                collisionSound.Play();
            }

            // Show game over UI if available
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }

            // Show win message if text component is available
            if (winMessageText != null)
            {
                winMessageText.text = "You Won! The key reached the door.";
            }

            // End the game (you can replace this with your own game end logic)
            Debug.Log("Game Over - Key reached the door!");

            // Option 1: Freeze the game but don't reload
            Time.timeScale = 0;

            // Option 2: Reload the current scene after a delay
            // Invoke("ReloadScene", 2.0f);

            // Option 3: Load a specific scene
            // Invoke("LoadGameOverScene", 2.0f);
        }
    }

    void ReloadScene()
    {
        Time.timeScale = 1; // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadGameOverScene()
    {
        Time.timeScale = 1; // Reset time scale
        SceneManager.LoadScene("GameOverScene"); // Replace with your scene name
    }
}
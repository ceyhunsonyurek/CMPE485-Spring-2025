using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;

    [Header("UI Elements")]
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;
    public Button mainMenuButton;
    public Button musicToggleButton;

    // Singleton instance
    public static UIManager Instance { get; private set; }

    private bool isGamePaused = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Hide all panels except HUD at start
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);

        // Set up button listeners
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ShowGameOverScreen()
    {
        if (hudPanel != null) hudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Ensure the cursor is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Optional: pause the game
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        if (hudPanel != null) hudPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(true);

        // Ensure the cursor is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Optional: pause the game
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            // Pause the game
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);

            // Show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);

            // Hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
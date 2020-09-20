using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    // Objects that represent the pause panel and dead panel.
    public GameObject pausePanel, deadPanel;
    static bool isPaused;

    void Start()
    {
        isPaused = false;
    }

    void Update()
    {
        // If escape is pressed, pause game if not paused, else resume game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                pauseGame();
            else
                resumeGame();
        }
    }

    // Pauses the game.
    public void pauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    // Resumes the game.
    public void resumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    // When retry is cliced, reset everything and reload the game scene.
    public void onRetryClicked()
    {
        FindObjectOfType<platforms>().resetSpeed();
        FindObjectOfType<background>().resetSpeed();
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        deadPanel.SetActive(false);
        SceneManager.LoadScene("game");
    }

    // When quit is clicked, reset everything and load the menu scene.
    public void onQuitClicked()
    {
        FindObjectOfType<platforms>().resetSpeed();
        FindObjectOfType<background>().resetSpeed();
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        deadPanel.SetActive(false);
        SceneManager.LoadScene("menu");
    }

    // When game is over.
    public void gameOver()
    {
        isPaused = true;
        Time.timeScale = 0;
        deadPanel.SetActive(true);
    }
}

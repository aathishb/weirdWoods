using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    // Objects representing the main menu panel and help menu panel.
    public GameObject menuPanel, helpPanel;

    // Loads the game scene when play is clicked.
    public void onPlayButtonClicked()
    {
        SceneManager.LoadScene("game");
    }

    // Displays the help panel when help is clicked.
    public void onHelpButtonClicked()
    {
        menuPanel.SetActive(false);
        helpPanel.SetActive(true);
    }

    // Goes back to the main menu from the help menu.
    public void onBackButtonClicked()
    {
        helpPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}

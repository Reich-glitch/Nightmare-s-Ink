using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for ScrollRect

public class StartMenuController : MonoBehaviour
{
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject aboutButton;
    public GameObject aboutPanel; // Panel that holds the Scroll View
    public GameObject mainMenu;
    public ScrollRect aboutScrollView; // Reference to the Scroll View



    private void Start()
    {
        aboutPanel.SetActive(false); // Ensure About Panel is hidden at start
        mainMenu.SetActive(true);
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnAboutClick()
    {
        // Hide main menu buttons
        startButton.SetActive(false);
        exitButton.SetActive(false);
        aboutButton.SetActive(false);

        // Show About Panel
        aboutPanel.SetActive(true);
        
        // Reset Scroll to the Top
        aboutScrollView.verticalNormalizedPosition = 1f;
    }

    public void OnCloseAbout()
    {
        // Show main menu buttons
        startButton.SetActive(true);
        exitButton.SetActive(true);
        aboutButton.SetActive(true);

        // Hide About Panel
        aboutPanel.SetActive(false);
    }
}

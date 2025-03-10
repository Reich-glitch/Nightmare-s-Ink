using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject LoadCanvas;
    public GameObject levelContainer; // Assign your LevelContainer here
    private List<GameObject> levels = new List<GameObject>();
    public static int currentLevelIndex = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    public static int survivedLevelsCount;

    public static event Action OnReset;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoad.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayedDied += GameOverScreen;
        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);

        // Automatically Get All Levels from the LevelContainer
        foreach (Transform child in levelContainer.transform)
        {
            levels.Add(child.gameObject);
        }

        LoadLevel(0, false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        MusicManager.PauseBackgroundMusic();
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) survivedText.text += "S";
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.PlayBackgroundMusic(true);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset?.Invoke(); // Fire reset event
        GameController.currentLevelIndex = 0;
        Time.timeScale = 1;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            // Level Complete
            LoadCanvas.SetActive(true);
        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        LoadCanvas.SetActive(false);

        // Turn off all Levels
        foreach (var lvl in levels)
        {
            lvl.SetActive(false);
        }

        // Loop Levels
        int loopedLevel = level % levels.Count; // Automatically loop based on level count
        levels[loopedLevel].SetActive(true);

        player.transform.position = Vector3.zero; 

        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = 0;

        if (wantSurvivedIncrease) survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}

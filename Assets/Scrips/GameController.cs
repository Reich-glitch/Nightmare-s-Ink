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
    public List<GameObject> levels;
    public static int currentLevelIndex = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    public static int survivedLevelsCount;

    public static event Action OnReset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoad.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayedDied += GameOverScreen;
        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) survivedText.text += "S";
        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset?.Invoke(); // Fire reset event
        GameController.currentLevelIndex = 0; // Reset the level count
        Time.timeScale = 1;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        if(progressAmount >= 100)
        {
            //Level Complete
            LoadCanvas.SetActive(true);
        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        LoadCanvas.SetActive(false);

        // Deactivate all levels first
        foreach (var lvl in levels)
        {
            lvl.SetActive(false);
        }

        int loopedLevel = level % levels.Count; // Loop the levels
        levels[loopedLevel].gameObject.SetActive(true);

        player.transform.position = new Vector3(0,0,0); 

        currentLevelIndex = survivedLevelsCount;
        progressAmount = 0;
        progressSlider.value = 0;
        if(wantSurvivedIncrease) survivedLevelsCount++;
    }
    
    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1; 
        LoadLevel(nextLevelIndex, true);
    }
}

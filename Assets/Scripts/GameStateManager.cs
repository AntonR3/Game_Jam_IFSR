using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [Header("Timer")]
    public Timer timer;
    public bool isPaused = false;

    [Header("CleaningScore")]
    public CleaningSlider progressBar;
    public int currentTrashCount;
    public int maxTrashCount;
    public float cleaningScore = 0f;
    public float totalCleaningScore = 1f;
    private int startingTrashCount = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        if (isPaused)
            return;

        CheckForWin();

    }
    private void RecalculateCleaningScore()
    {
        if (startingTrashCount <= 0)
        {
            // Keine Referenzmenge -> als vollst�ndig "sauber" behandeln
            cleaningScore = 1f;
        }
        else
        {
            int cleaned = startingTrashCount - currentTrashCount;
            float percent = (float)cleaned / (float)startingTrashCount;
            cleaningScore = Mathf.Clamp01(percent);
        }

        if (progressBar != null)
        {
            progressBar.AnimateTo(cleaningScore);
        }
    }
    public void IncreaseMaxTrashCount(int amount)
    {
        maxTrashCount += amount;
    }
    public void IncreaseTrashCount(int amount)
    {
        currentTrashCount += amount;
        RecalculateCleaningScore();
    }

    public void DecreaseTrashCount(int amount)
    {
        currentTrashCount -= amount;
        if (currentTrashCount < 0) currentTrashCount = 0;
        RecalculateCleaningScore();
    }
    public void CheckForWin()
    {
        if(cleaningScore >= totalCleaningScore)
        {
            Win();
        }
    }

    private void Win()
    {
        //SHOW WIN SCREEN
        //STOP GAME LOGIC
        PauseGame();
        Debug.Log("You are a winner!");
    }
    [Button("Start Night")]
    public void StartNight(int nightNumber, int trashAmount)
    {
        timer.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);
        SpawnManager.instance.SpawnTrash(trashAmount);
        startingTrashCount = currentTrashCount;
        RecalculateCleaningScore();

        timer.timeLeft = 300f; // Reset timer for the new night
        timer.isRunning = true;
    }

    public void EndNight()
    {
        timer.isRunning = false;
        //Show end of night stats

        //Spawn next night trash
        SpawnManager.instance.SpawnTrash(30);
        startingTrashCount = currentTrashCount;
        RecalculateCleaningScore();
    }

    public void GameOver()
    {
        Debug.Log("You Lost!");
    }

    public void PauseGame()
    {
        // Pause the game logic, e.g., stop timers, animations, etc.

        //SHOW PAUSE MENU
        isPaused = true;
    }

    public void ResumeGame()
    {
        // DEACTIVATE PAUSE UI
        isPaused = false;
    }

    public void HideTimerAndProgressBar()
    {
        if (timer != null)
        {
            timer.gameObject.SetActive(false);
        }
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }
}

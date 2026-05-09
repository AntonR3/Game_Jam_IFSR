using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    [Header("Timer")]
    public Timer timer;
    public bool isPaused = false;

    [Header("GameStats")]
    int currentNight = 0;
    int lastNight = 6;
    public int currentTrashCount;
    public int maxTrashCount;
    public float cleaningScore = 0f;
    public float totalCleaningScore = 1f;

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

    public void IncreaseTrashCount(int amount)
    {
        currentTrashCount += amount;
    }

    public void IncreaseMaxTrashCount(int amount)
    {
        maxTrashCount += amount;
    }
    public void Update()
    {
        if (isPaused)
            return;

        CheckForWin();

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
    public void StartNight()
    {
        if(currentNight == 0)
        {
            //Spawn first night trash
            SpawnManager.instance.SpawnTrash(90);
        }
        if (currentNight == lastNight)
        {
            GameOver();
            return;
        }
        currentNight++;
        timer.timeLeft = 300f; // Reset timer for the new night
        timer.isRunning = true;
    }

    public void EndNight()
    {
        timer.isRunning = false;
        //Show end of night stats

        //Spawn next night trash
        SpawnManager.instance.SpawnTrash(30);
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
}

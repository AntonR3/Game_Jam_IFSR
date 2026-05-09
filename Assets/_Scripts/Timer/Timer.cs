using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeLeft = 300f; // 5 Minuten in Sekunden
    public Text timerText;       // Verknüpfen Sie hier Ihr UI-Text-Element
    public bool isRunning = false;
    
    void Update()
    {
        if(isRunning)
            HandleTimer();
    }
    private void HandleTimer()
    {
        if (GameStateManager.instance.isPaused)
            return;

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            timeLeft = 0;
            // Hier Aktionen bei Zeitablauf ausführen
            GameStateManager.instance.EndNight();
        }
    }
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
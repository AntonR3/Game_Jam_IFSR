using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIController uiController;
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] Camera startScreenCamera;
    [Header("GameStats")]
    int lastNight = 6;

    [SerializeField] int initialTrashAmount = 200;
    [SerializeField] int trashRespawnPerNight = 20;
    int nightInteger = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //show startscreen
        uiController.ShowStartScreen();
        uiController.HidePauseScreen();
        uiController.HideMerchantScreen();
        startScreenCamera.gameObject.SetActive(true);
        gameStateManager.HideTimerAndProgressBar();

    }
    public void StartGame()
    {
        uiController.HideStartScreen();
        startScreenCamera.gameObject.SetActive(false);
        StartGameLoop();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGameLoop()
    {
        StartNight(nightInteger);
    }
    public void StartNight(int nightNumber)
    {
        nightInteger++;
        if (nightInteger >= lastNight)
        {
            QuitGame();
            return;
        }
        gameStateManager.StartNight(nightInteger, nightInteger== 1 ? initialTrashAmount : trashRespawnPerNight);
        Debug.Log($"Starting night {nightInteger}");
        // Spawn trash logic here, using initialTrashAmount and nightInteger to scale difficulty
    }
}

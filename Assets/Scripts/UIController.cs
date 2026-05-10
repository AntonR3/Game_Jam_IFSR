using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.UI;
using NUnit.Framework;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] Slider weightBar;
    // float weightTickTimer = 0f;

    [SerializeField] GameObject ingameUICanvas;
    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] GameObject startScreenCanvas;
    [SerializeField] Camera startScreenCamera;

    [SerializeField] GameObject merchantCanvas;
    [SerializeField] Camera merchantCamera;
    [SerializeField] CinemachineInputAxisController cinemachineFreeLook;
    [SerializeField] MovementController movementController;
    [SerializeField] TextMeshProUGUI cashText;

    public bool interactionEnabled = false;
    PlayerInput playerInput;
    InputAction escapeAction;
    InputAction interactAction;
    Keyboard keyboard;
    bool ingame = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            escapeAction = playerInput.actions["Escape"];
            interactAction = playerInput.actions["Interact"];
            Debug.Log($"UIController: PlayerInput found, Escape action: {(escapeAction != null ? "FOUND" : "NOT FOUND")}, Interact action: {(interactAction != null ? "FOUND" : "NOT FOUND")}");
        }
        else
        {
            Debug.LogWarning("UIController: PlayerInput NOT found on this GameObject. Using Input System directly.");
            keyboard = Keyboard.current;
        }
        ApplyCursorState(true);
    }

    void Update()
    {
        // weightTickTimer += Time.deltaTime;
        // if (weightTickTimer >= 1f)
        // {
        //     weightTickTimer -= 1f;
        //     UpdateWeight(5);
        // }

        if (!ingame)
        {
            return; // Don't check for pause input if not in game
        }
        bool escapePressed = false;

        // Try PlayerInput first
        if (escapeAction != null && escapeAction.WasPressedThisFrame())
        {
            escapePressed = true;
        }
        // Fallback to direct Input System keyboard
        else if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            escapePressed = true;
        }

        if (escapePressed)
        {
            Debug.Log($"ESC pressed!");
            ShowPauseScreen();
        }

        if (interactionEnabled && interactAction != null && interactAction.WasPressedThisFrame())
        {
            Debug.Log("Interact pressed while interaction enabled! Showing merchant screen.");
            HideIngameUI();
            ShowMerchantScreen();
        }

        // Unity Editor and focus changes can reset cursor lock state.
        // Re-apply when the current state differs from what gameplay/UI expects.
        if (Cursor.lockState != (ingame ? CursorLockMode.Locked: CursorLockMode.None) || Cursor.visible == ingame)
        {
            ApplyCursorState(!ingame);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplyCursorState(true);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
            ApplyCursorState(false);
    }

    void ApplyCursorState(bool menuOpen)
    {
        Cursor.lockState = menuOpen ? CursorLockMode.None : CursorLockMode.Locked;;
        Cursor.visible = menuOpen;
        cinemachineFreeLook.GetComponent<CinemachineInputAxisController>().enabled = !menuOpen;
        movementController.SetFreezeMovement(menuOpen);
    }

    public void OnContinuePressed()
    {
        HidePauseScreen();
    }

    public void SetMaxWeight(int maxWeight)
    {
        if (weightBar != null)
        {
            weightBar.minValue = 0;
            weightBar.maxValue = maxWeight;
            weightBar.value = 0;
            
        }
    }

    public void UpdateWeight(int newWeight)
    {
        weightBar.value = newWeight;
    }
    public void ResetWeight()
    {
        weightBar.value = 0;
    }

    public void UpdateMaxWeight(int newMaxWeight)
    {
        weightBar.maxValue = newMaxWeight;
    }

    public void ShowStartScreen()
    {
        Debug.Log("Showing start screen");
        startScreenCanvas.SetActive(true);
        startScreenCamera.gameObject.SetActive(true);
        ApplyCursorState(true);
        ingame = false;
        HideIngameUI();
    }

    public void HideStartScreen()
    {
        Debug.Log("Hiding start screen");
        startScreenCanvas.SetActive(false);
        startScreenCamera.gameObject.SetActive(false);
        ApplyCursorState(false);
        ingame = true;
        ShowIngameUI();
    }

    public void ShowPauseScreen()
    {
        Debug.Log("Showing pause screen");
        pauseMenuCanvas.SetActive(true);
        ApplyCursorState(true);
        ingame = false;
        HideIngameUI();
        
    }

    public void HidePauseScreen()
    {
        Debug.Log("Hiding pause screen");
        pauseMenuCanvas.SetActive(false);
        ApplyCursorState(false);
        ingame = true;
        ShowIngameUI();
    }

    public void ShowMerchantScreen()
    {
        Debug.Log("Showing merchant screen");
        merchantCanvas.SetActive(true);
        merchantCamera.gameObject.SetActive(true);
        ApplyCursorState(true);
        ingame = false;
        HideIngameUI();
    }

    public void HideMerchantScreen()
    {
        Debug.Log("Hiding merchant screen");
        merchantCanvas.SetActive(false);
        merchantCamera.gameObject.SetActive(false);
        ApplyCursorState(false);
        ingame = true;
        ShowIngameUI();
    }

    public void ShowIngameUI()
    {
        ingameUICanvas.SetActive(true);
    }

    public void HideIngameUI()
    {
        ingameUICanvas.SetActive(false);
    }

    public void EnableInteraction()
    {
        // Enable interaction UI elements, e.g. show "Press E to interact" text
        Debug.Log("Enabling interaction UI");
        interactionEnabled = true;
    }

    public void DisableInteraction()
    {
        // Disable interaction UI elements
        Debug.Log("Disabling interaction UI");
        interactionEnabled = false;
    }

    public void ExitMerchantScreen()
    {
        HideMerchantScreen();
        ShowIngameUI();
    }

    public void UpdateCash(int newCashAmount)
    {
        cashText.text = $"Cash: {newCashAmount}";
    }
}
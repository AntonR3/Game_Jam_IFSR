using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.UI;
using NUnit.Framework;
using TMPro;
using System.Collections;

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
    [SerializeField] TextMeshProUGUI cashTextInMerchant;

    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI staminaRegenText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] TextMeshProUGUI spaceText;


    public bool interactionEnabled = false;
    PlayerInput playerInput;
    InputAction escapeAction;
    InputAction interactAction;
    Keyboard keyboard;
    bool ingame = false;
    int[] pricelist = {5, 10, 25, 45, 70, 100, 135, 175, 220, 270};
    Coroutine failedUpgradeResetCoroutine;
    const float failedUpgradeMessageDuration = 2f;

    int currentSpeedLevel;
    int currentStaminaLevel;
    int currentStaminaRegenLevel;
    int currentRangeLevel;
    int currentSpaceLevel;


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
        cashTextInMerchant.text = $"Cash: {newCashAmount}";
    }

    public void UpdateUpgradeTexts(int speedLevel, int staminaLevel, int staminaRegenLevel, int rangeLevel, int spaceLevel)
    {
        currentSpeedLevel = speedLevel;
        currentStaminaLevel = staminaLevel;
        currentStaminaRegenLevel = staminaRegenLevel;
        currentRangeLevel = rangeLevel;
        currentSpaceLevel = spaceLevel;

        if (speedLevel < pricelist.Length)
            speedText.text = $"Speed Lv: {speedLevel} ({pricelist[speedLevel]}$)";
        else
            speedText.text = $"Speed Lv: {speedLevel} (MAX)";

        if (staminaLevel < pricelist.Length)
            staminaText.text = $"Stamina Lv: {staminaLevel} ({pricelist[staminaLevel]}$)";
        else
            staminaText.text = $"Stamina Lv: {staminaLevel} (MAX)";

        if (staminaRegenLevel < pricelist.Length)
            staminaRegenText.text = $"Stamina Regen Lv: {staminaRegenLevel} ({pricelist[staminaRegenLevel]}$)";
        else
            staminaRegenText.text = $"Stamina Regen Lv: {staminaRegenLevel} (MAX)";

        if (rangeLevel < pricelist.Length)
            rangeText.text = $"Range Lv: {rangeLevel} ({pricelist[rangeLevel]}$)";
        else
            rangeText.text = $"Range Lv: {rangeLevel} (MAX)";

        if (spaceLevel < pricelist.Length)
            spaceText.text = $"Space Lv: {spaceLevel} ({pricelist[spaceLevel]}$)";
        else
            spaceText.text = $"Space Lv: {spaceLevel} (MAX)";
    }

    public void FailedUpgrade(int reason)
    {
        switch (reason)
        {
            case 0:
            {
                string price = currentSpeedLevel < pricelist.Length ? $"{pricelist[currentSpeedLevel]}$" : "MAX";
                speedText.text = $"Not enough cash! Upgrade costs {price}";
            }
                break;
            case 1:
            {
                string price = currentStaminaLevel < pricelist.Length ? $"{pricelist[currentStaminaLevel]}$" : "MAX";
                staminaText.text = $"Not enough cash! Upgrade costs {price}";
            }
                break;
            case 2:
            {
                string price = currentStaminaRegenLevel < pricelist.Length ? $"{pricelist[currentStaminaRegenLevel]}$" : "MAX";
                staminaRegenText.text = $"Not enough cash! Upgrade costs {price}";
            }
                break;
            case 3:
            {
                string price = currentRangeLevel < pricelist.Length ? $"{pricelist[currentRangeLevel]}$" : "MAX";
                rangeText.text = $"Not enough cash! Upgrade costs {price}";
            }
                break;
            case 4:
            {
                string price = currentSpaceLevel < pricelist.Length ? $"{pricelist[currentSpaceLevel]}$" : "MAX";
                spaceText.text = $"Not enough cash! Upgrade costs {price}";
            }
                break;
            case 5:
                speedText.text = $"Speed is already at max level!";
                break;
            case 6:
                staminaText.text = $"Stamina is already at max level!";
                break;
            case 7:
                staminaRegenText.text = $"Stamina Regen is already at max level!";
                break;
            case 8:
                rangeText.text = $"Range is already at max level!";
                break;
            case 9:
                spaceText.text = $"Space is already at max level!";
                break;
            default:
                Debug.Log("Unknown upgrade failure reason!");
                break;
        }

        if (failedUpgradeResetCoroutine != null)
        {
            StopCoroutine(failedUpgradeResetCoroutine);
        }

        failedUpgradeResetCoroutine = StartCoroutine(ResetFailedUpgradeTextAfterDelay());
    }

    IEnumerator ResetFailedUpgradeTextAfterDelay()
    {
        yield return new WaitForSeconds(failedUpgradeMessageDuration);
        UpdateUpgradeTexts(currentSpeedLevel, currentStaminaLevel, currentStaminaRegenLevel, currentRangeLevel, currentSpaceLevel);
        failedUpgradeResetCoroutine = null;
    }
}
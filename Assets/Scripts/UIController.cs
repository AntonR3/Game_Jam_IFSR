using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    int maxWeight = 100;
    int currentWeight = 0;
    [SerializeField] Slider weightBar;
    float weightTickTimer = 0f;

    [SerializeField] bool startWithMenuOpen = false;
    [SerializeField] GameObject pauseMenuCanvas;
    [SerializeField] CinemachineInputAxisController cinemachineFreeLook;
    [SerializeField] MovementController movementController;
    PlayerInput playerInput;
    InputAction escapeAction;
    Keyboard keyboard;
    bool menuOpen;

    void Awake()
    {
        SetupWeightSlider();
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            escapeAction = playerInput.actions["Escape"];
            Debug.Log($"UIController: PlayerInput found, Escape action: {(escapeAction != null ? "FOUND" : "NOT FOUND")}");
        }
        else
        {
            Debug.LogWarning("UIController: PlayerInput NOT found on this GameObject. Using Input System directly.");
            keyboard = Keyboard.current;
        }

        menuOpen = startWithMenuOpen;
        ApplyCursorState();
    }

    void Start()
    {
        Debug.Log($"UIController Start: Canvas assigned: {(pauseMenuCanvas != null ? "YES" : "NO")}");
    }

    void Update()
    {
        weightTickTimer += Time.deltaTime;
        if (weightTickTimer >= 1f)
        {
            weightTickTimer -= 1f;
            UpdateWeight(5);
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
            menuOpen = !menuOpen;
            Debug.Log($"ESC pressed! Menu now: {menuOpen}, Canvas: {(pauseMenuCanvas != null ? "SET" : "NULL")}");
            ApplyCursorState();
            if (movementController != null)
            {
                movementController.SetFreezeMovement(menuOpen);
            }
        }

        // Unity Editor and focus changes can reset cursor lock state.
        // Re-apply when the current state differs from what gameplay/UI expects.
        if (Cursor.lockState != DesiredLockState() || Cursor.visible != menuOpen)
            ApplyCursorState();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplyCursorState();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
            ApplyCursorState();
    }

    CursorLockMode DesiredLockState()
    {
        return menuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void ApplyCursorState()
    {
        pauseMenuCanvas.SetActive(menuOpen);
        Cursor.lockState = DesiredLockState();
        Cursor.visible = menuOpen;
        if (cinemachineFreeLook != null)
        {
            cinemachineFreeLook.GetComponent<CinemachineInputAxisController>().enabled = !menuOpen;
        }
    }

    public void OnContinuePressed()
    {
        menuOpen = false;
        ApplyCursorState();
        if (movementController != null)
        {
            movementController.SetFreezeMovement(false);
        }
    }

    public bool getMenuState()
    {
        return menuOpen;
    }

    void SetupWeightSlider()
    {
        if (weightBar != null)
        {
            weightBar.minValue = 0;
            weightBar.maxValue = maxWeight;
            weightBar.value = currentWeight;
            
        }
    }

    public bool UpdateWeight(int weightChange)
    {
        if (currentWeight + weightChange > maxWeight)
        {
            return false; // Can't pick up item, would exceed max weight
        }
        currentWeight = Mathf.Clamp(currentWeight + weightChange, 0, maxWeight);
        if (weightBar != null)
        {
            weightBar.value = currentWeight;
        }
        return true;
    }

    public void updateMaxWeight(int change)
    {
        maxWeight += change;
        if (weightBar != null)
        {
            weightBar.maxValue = maxWeight;
            weightBar.value = currentWeight;
        }
    }

    public void updateMaxStamina(int change) {
        if (movementController != null)
        {
            movementController.IncreaseMaxStamina(change);
        }
    }

    public void updateStaminaRegen(int change) {
        if (movementController != null)
        {
            movementController.IncreaseStaminaRegen(change);
        }
    }
}
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] MovementController movementController;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] UIController uiController;
    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

        public void UpgradeSpeed()
    {
        movementController.UpgradeSpeed();
    }

    public void UpgradeStamina()
    {
        movementController.UpgradeStamina();
    }
    
    public void UpgradeStaminaRegen()
    {
        movementController.UpgradeStaminaRegen();
    }

    public void UpgradeRange()
    {
        playerInventory.UpgradeRange();
    }

    public void UpgradeSpace()
    {
        playerInventory.UpgradeSpace();
    }
}
using System;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] MovementController movementController;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] UIController uiController;

    int[] pricelist = {5, 10, 25, 45, 70, 100, 135, 175, 220, 270};
    int speedlevel = 0;
    int staminalevel = 0;
    int staminaregenlevel = 0;
    int rangelevel = 0;
    int spacelevel = 0;
    public void Start()
    {
        uiController.UpdateUpgradeTexts(speedlevel, staminalevel, staminaregenlevel, rangelevel, spacelevel);
    }

    public void Update()
    {
        
    }

        public void UpgradeSpeed()
    {
        if (!CanUpgradeSpeed())
        {
            return;   
        }
        movementController.UpgradeSpeed();
        speedlevel++;
        uiController.UpdateUpgradeTexts(speedlevel, staminalevel, staminaregenlevel, rangelevel, spacelevel);
        playerInventory.coins -= pricelist[speedlevel-1];
        uiController.UpdateCash(playerInventory.coins);
    }

    public void UpgradeStamina()
    {
        if (!CanUpgradeStamina())
        {
            return;   
        }
        movementController.UpgradeStamina();
        staminalevel++;
        uiController.UpdateUpgradeTexts(speedlevel, staminalevel, staminaregenlevel, rangelevel, spacelevel);
        playerInventory.coins -= pricelist[staminalevel-1];
        uiController.UpdateCash(playerInventory.coins);
    }
    
    public void UpgradeStaminaRegen()
    {
        if (!CanUpgradeStaminaRegen())
        {
            return;   
        }
        movementController.UpgradeStaminaRegen();
        staminaregenlevel++;
        uiController.UpdateUpgradeTexts(speedlevel, staminalevel, staminaregenlevel, rangelevel, spacelevel);
        playerInventory.coins -= pricelist[staminaregenlevel-1];
        uiController.UpdateCash(playerInventory.coins);
    }

    public void UpgradeRange()
    {
        if (!CanUpgradeRange())
        {
            return;   
        }
        playerInventory.UpgradeRange();
        rangelevel++;
        uiController.UpdateUpgradeTexts(speedlevel, staminalevel, staminaregenlevel, rangelevel, spacelevel);
        playerInventory.coins -= pricelist[rangelevel-1];
        uiController.UpdateCash(playerInventory.coins);
    }

    public void UpgradeSpace()
    {
        if (!CanUpgradeSpace())
        {
            return;   
        }
        playerInventory.UpgradeSpace();
        spacelevel++;
        uiController.UpdateUpgradeTexts(speedlevel, staminalevel, staminaregenlevel, rangelevel, spacelevel);
        playerInventory.coins -= pricelist[spacelevel-1];
        uiController.UpdateCash(playerInventory.coins);
    }

    public bool CanUpgradeSpeed()
    {
        if (speedlevel >= pricelist.Length)
        {
            Debug.Log("Speed is already at max level!");
            uiController.FailedUpgrade(5);
            return false;
        }
        if (playerInventory.coins >= pricelist[speedlevel])
        {
            return true;
        }
        uiController.FailedUpgrade(0);
        return false;
    }

    public bool CanUpgradeStamina()
    {
        if (staminalevel >= pricelist.Length)
        {
            Debug.Log("Stamina is already at max level!");
            uiController.FailedUpgrade(6);
            return false;
        }
        if (playerInventory.coins >= pricelist[staminalevel])
        {
            return true;
        }
        uiController.FailedUpgrade(1);
        return false;
    }

    public bool CanUpgradeStaminaRegen()
    {
        if (staminaregenlevel >= pricelist.Length)
        {
            Debug.Log("Stamina Regen is already at max level!");
            uiController.FailedUpgrade(7);
            return false;
        }
        if (playerInventory.coins >= pricelist[staminaregenlevel])
        {
            return true;
        }
        uiController.FailedUpgrade(2);
        return false;
    }

    public bool CanUpgradeRange()
    {
        if (rangelevel >= pricelist.Length)
        {
            Debug.Log("Range is already at max level!");
            uiController.FailedUpgrade(8);
            return false;
        }
        if (playerInventory.coins >= pricelist[rangelevel])
        {
            return true;
        }
        uiController.FailedUpgrade(3);
        return false;
    }

    public bool CanUpgradeSpace()
    {
        if (spacelevel >= pricelist.Length)
        {
            Debug.Log("Space is already at max level!");
            uiController.FailedUpgrade(9);
            return false;
        }
        if (playerInventory.coins >= pricelist[spacelevel])
        {
            return true;
        }
        uiController.FailedUpgrade(4);
        return false;
    }
}
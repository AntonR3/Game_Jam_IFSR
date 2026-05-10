using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int coins = 0;
    public int maxPocketSize = 5;
    public int weight = 7;
    public int pocketSizeUpgradeAmount = 1;
    public float rangeUpgradeAmount = 1f;
    [SerializeField] float pickupRange = 2f;
    public GameObject pickupCollider;
    public List<int> inventory = new List<int>();

    [SerializeField] UIController uiController;

    private void Start()
    {
        pickupCollider.transform.localScale = new Vector3(pickupRange, pickupRange/2, pickupRange);
        uiController.SetMaxWeight(maxPocketSize);
    }

    public void AddPickUpRange(float x)
    {
        SetPickupRange(pickupRange + x);
    }
    public void DecreasePickUpRange(float x)
    {
        SetPickupRange(pickupRange - x);
    }
    private void SetPickupRange(float range)
    {
        pickupRange = range;
        if(pickupCollider != null)
        {
            pickupCollider.transform.localScale = new Vector3(pickupRange, pickupRange / 2, pickupRange);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Trash")
        {
            SpawnNode node = other.GetComponentInParent<SpawnNode>();
            if(node != null)
            {
                if(AddToInventory(node.trashObjectID))
                    node.RemoveTrash();
            }
        }

        if(other.tag == "Merchant")
        {
            SellInventory();
        }
    }

    public bool AddToInventory(int trashID)
    {
        // if(inventory.Count >= maxPocketSize)
        // {
        //     Debug.Log("Inventory is full! Cannot add more trash.");
        //     return false;
        // }

        TrashData data = TrashDataManager.instance.GetTrashByID(trashID);
        if (data.weight + weight > maxPocketSize)
        {
            Debug.Log("Cannot add " + data._name + " to inventory! Not enough space. Current weight: " + weight);
            return false;
        }
        inventory.Add(trashID);
        weight += data.weight;
        uiController.UpdateWeight(data.weight);
        coins += data.value;

        GameStateManager.instance.DecreaseTrashCount(1);

        Debug.Log("Added: " + data._name + " to inventory! Current weight: " + weight);
        return true;
    }

    public void SellInventory()
    {
        //SELL ALL TRASH IN INVENTORY TO MERCHANT AND GAIN COINS
        foreach(int trashID in inventory)
        {
            TrashData data = TrashDataManager.instance.GetTrashByID(trashID);
            //GameStateManager.instance.IncreaseCleaningScore(data.cleaningScore);

            //Display what item was and how much it was worth
            Debug.Log("Sold: " + data._name + " for " + data.value + " coins!" );
        }

        inventory.Clear();
        weight = 0;
        uiController.ResetWeight();
    }

    public void IncreaseMaxPocketSize(int amount)
    {
        maxPocketSize += amount;
    }

    public void UpgradeRange()
    {
        AddPickUpRange(rangeUpgradeAmount);
    }

    public void UpgradeSpace()
    {
        IncreaseMaxPocketSize(pocketSizeUpgradeAmount);
        uiController.UpdateMaxWeight(maxPocketSize);
    }

}

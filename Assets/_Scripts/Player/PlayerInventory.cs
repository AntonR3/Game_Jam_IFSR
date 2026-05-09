using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public int coins = 0;
    public int maxPocketSize = 5;
    public int weight = 7;
    [SerializeField] float pickupRange = 2f;
    public GameObject pickupCollider;
    public List<int> inventory = new List<int>();

    private void Start()
    {
        pickupCollider.transform.localScale = new Vector3(pickupRange, pickupRange/2, pickupRange);
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
        if(inventory.Count >= maxPocketSize)
        {
            Debug.Log("Inventory is full! Cannot add more trash.");
            return false;
        }

        inventory.Add(trashID);
        TrashData data = TrashDataManager.instance.GetTrashByID(trashID);
        weight += data.weight;
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
    }


}

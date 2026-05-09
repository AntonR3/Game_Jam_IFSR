using JetBrains.Annotations;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [Header("Nodes")]
    public List<SpawnNode> allNodes = new List<SpawnNode>();
    public List<SpawnNode> floorNodes = new List<SpawnNode>();
    public List<SpawnNode> tableNodes = new List<SpawnNode>();
    public List<SpawnNode> kitchenNodes = new List<SpawnNode>();

    [Header("Raritys")]
    public int[] floorWeights;
    public int[] tableWeights;
    public int[] kitchenWeights;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitNodeLists();
    }

    private void InitNodeLists()
    {
        //TOD:
        //LOOP OVER ALL CHILDREN AND ADD THEM TO allNodes
        
        // GROUP THE NODES BY REGION
        foreach (SpawnNode node in allNodes)
        {
            switch (node.region)
            {
                case Region.Table:
                    tableNodes.Add(node);
                    break;
                case Region.Floor:
                    floorNodes.Add(node);
                    break;
                case Region.KitchenCounter:
                    kitchenNodes.Add(node);
                    break;
                default:
                    break;
            }
        }
    }

    private int CalculateItemIDByRegionRarity(Region region)
    {
        switch (region)
        {
            case Region.Floor:
                int i = Random.Range(0, floorWeights.Length);
                return floorWeights[i];
            case Region.KitchenCounter:
                int j = Random.Range(0, kitchenWeights.Length);
                return kitchenWeights[j];
            case Region.Table:
                int k = Random.Range(0, tableWeights.Length);
                return tableWeights[k];
            default:
                return 0;
        }
    }

    private List<SpawnNode> GetRandomValidNodesFromRegion(Region region)
    {
        List<SpawnNode> validNodes = new List<SpawnNode>();
        switch (region)
        {
            case Region.Floor:
                foreach (SpawnNode node in floorNodes)
                {
                    if (!node.isOccupied)
                    {
                        validNodes.Add(node);
                    }
                }
                return validNodes;
            case Region.KitchenCounter:
                foreach (SpawnNode node in kitchenNodes)
                {
                    if (!node.isOccupied)
                    {
                        validNodes.Add(node);
                    }
                }
                return validNodes;
            case Region.Table:
                foreach (SpawnNode node in tableNodes)
                {
                    if (!node.isOccupied)
                    {
                        validNodes.Add(node);
                    }
                }
                return validNodes;
            default:
                return null;
        }
    }
    [Button("Spawn Trash")]
    public void DebugTrash()
    {
        SpawnTrash(9);
    }
    public void SpawnTrash(int amount)
    {
        int trashAmountToSpawn = amount;
        List<SpawnNode> spawnPoints = new List<SpawnNode>();
        List<SpawnNode> validFloorNodes = GetRandomValidNodesFromRegion(Region.Floor);
        List<SpawnNode> validTableNodes = GetRandomValidNodesFromRegion(Region.Table);
        List<SpawnNode> validKitchenNodes = GetRandomValidNodesFromRegion(Region.KitchenCounter);

        int validNodesCount = validFloorNodes.Count + validTableNodes.Count + validKitchenNodes.Count;
        int spawnedTrashCount = 0;

        if (validNodesCount == 0)
        {
            Debug.Log("DU BIST EIN SCHMUTZFINK!!!");
            return;
        }

        if (validFloorNodes.Count > 0)
        { 
            //TODO:
            // SHUFFLE THE SPAWNPOINTS
            for (int i = 0; i < trashAmountToSpawn / 3; i++)
            {
                validFloorNodes[i].SpawnTrash(CalculateItemIDByRegionRarity(Region.Floor));
                spawnedTrashCount++;
            }

            for (int i = 0; i < trashAmountToSpawn / 3; i++)
            {
                validTableNodes[i].SpawnTrash(CalculateItemIDByRegionRarity(Region.Table));
                spawnedTrashCount++;
            }

            for (int i = 0; i < trashAmountToSpawn / 3; i++)
            {
                validKitchenNodes[i].SpawnTrash(CalculateItemIDByRegionRarity(Region.KitchenCounter));
                spawnedTrashCount++;
            }
        }
        Debug.Log("Spawned: " + spawnedTrashCount + " Trash Items");
        GameStateManager.instance.IncreaseTrashCount(spawnedTrashCount);
    }
}
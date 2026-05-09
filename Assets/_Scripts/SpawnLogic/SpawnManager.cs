using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    [Header("Debug")]
    public int trashcountDebug;
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
        //LOOP OVER ALL CHILDREN AND ADD THEM TO allNodes
        foreach (Transform child in transform)
        {
            SpawnNode node = child.GetComponent<SpawnNode>();
            if (node != null)
            {
                allNodes.Add(node);
            }
        }
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
                int i = UnityEngine.Random.Range(0, floorWeights.Length);
                return floorWeights[i];
            case Region.KitchenCounter:
                int j = UnityEngine.Random.Range(0, kitchenWeights.Length);
                return kitchenWeights[j];
            case Region.Table:
                int k = UnityEngine.Random.Range(0, tableWeights.Length);
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
    //HELPER
    // Fisher-Yates shuffle using Unity's Random
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
    [Button("Spawn Trash")]
    public void DebugTrash()
    {
        SpawnTrash(trashcountDebug);
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

        // Distribute 'amount' roughly equally across regions but respect available nodes per region
        var regionLists = new List<(Region region, List<SpawnNode> nodes)>
        {
            (Region.Floor, validFloorNodes),
            (Region.Table, validTableNodes),
            (Region.KitchenCounter, validKitchenNodes)
        };

        int[] available = regionLists.Select(r => r.nodes.Count).ToArray();
        int[] desired = new int[3];

        int baseShare = trashAmountToSpawn / 3;
        int remainder = trashAmountToSpawn % 3;

        // Give each region the base share, capped by availability
        for (int idx = 0; idx < 3; idx++)
        {
            desired[idx] = Math.Min(available[idx], baseShare);
        }

        // Calculate leftover to distribute
        int allocated = desired.Sum();
        int leftover = trashAmountToSpawn - allocated;

        // Distribute leftover to regions with remaining capacity (prefer those with larger spare capacity)
        while (leftover > 0)
        {
            int bestIdx = -1;
            int bestCap = 0;
            for (int idx = 0; idx < 3; idx++)
            {
                int cap = available[idx] - desired[idx];
                if (cap > bestCap)
                {
                    bestCap = cap;
                    bestIdx = idx;
                }
            }
            if (bestIdx == -1) break; // no capacity left anywhere
            desired[bestIdx]++;
            leftover--;
        }

        // For each region: shuffle valid nodes, take desired count and add to global spawnPoints
        for (int idx = 0; idx < 3; idx++)
        {
            var nodes = regionLists[idx].nodes;
            if (nodes.Count == 0 || desired[idx] == 0) continue;
            Shuffle(nodes);
            int takeCount = Math.Min(desired[idx], nodes.Count);
            for (int t = 0; t < takeCount; t++)
            {
                spawnPoints.Add(nodes[t]);
            }
        }

        // Shuffle all chosen spawn points so actual spawn order is random across regions
        Shuffle(spawnPoints);

        // Spawn at chosen points using their region rarity
        foreach (var node in spawnPoints)
        {
            node.SpawnTrash(CalculateItemIDByRegionRarity(node.region));
            spawnedTrashCount++;
        }

        Debug.Log("Spawned: " + spawnedTrashCount + " Trash Items");
        GameStateManager.instance.IncreaseTrashCount(spawnedTrashCount);
    }
}
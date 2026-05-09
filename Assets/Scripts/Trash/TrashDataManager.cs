using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TrashDataManager : MonoBehaviour
{
    public static TrashDataManager instance;

    [Header("TrashData")]
    public List<TrashData> Trash = new List<TrashData>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitTrashIds();
    }

    public TrashData GetTrashByID(int trashID)
    {
        foreach(TrashData trash in Trash)
        {
            if(trash.id == trashID)
                return trash;
        }
        return null;
    }

    private void InitTrashIds()
    {
        for (int i = 0; i < Trash.Count; i++)
        {
            Trash[i].id = i;
        }
    }
}

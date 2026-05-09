using UnityEngine;

public class SpawnNode : MonoBehaviour
{
    [Header("Info")]
    public int id;
    public Region region;
    public GameObject trashObject;
    public int trashObjectID;
    public bool isOccupied;
    public Vector3 position;

    public void SpawnTrash(int id)
    {
        isOccupied = true;
        TrashData trash = TrashDataManager.instance.GetTrashByID(id);
        trashObject = Instantiate(trash.trashModel, transform);
        trashObject.name = trash._name;
        trashObjectID = trash.id;
    }

    public void RemoveTrash()
    {
        //notify spawnmanager that you are free
        isOccupied = false;

        Destroy(trashObject);
    }


}

public enum Region
{
    None,
    Floor,
    Table,
    KitchenCounter,
}
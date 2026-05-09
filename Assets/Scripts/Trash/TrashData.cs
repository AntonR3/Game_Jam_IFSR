using UnityEngine;

[CreateAssetMenu(menuName = "Trash")]
public class TrashData : ScriptableObject
{
    [Header("Sats")]
    //ENUM RARITY
    public int id;
    public string _name = "Trash";
    public int value;
    public int weight;

    [Header("Rarity Weight")]
    public int floorWeight;
    public int tableWeight;
    public int kitchenWeight;

    [Header("Objects")]
    public GameObject trashModel;
    public Sprite trashSprite;
}

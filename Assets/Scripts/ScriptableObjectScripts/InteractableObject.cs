using UnityEngine;
public enum ItemTypeEnum
{
    Inventory,
    NonInventory,
}
[CreateAssetMenu(fileName = "InteractableObject", menuName = "Scriptable Objects/InteractableObject")]
public class InteractableObject : ScriptableObject
{
    [SerializeField] Sprite ItemSprite;
    [SerializeField] string ItemID;
    [SerializeField] string ItemName;
    [SerializeField] string Description;
    [SerializeField] ItemTypeEnum ItemType;
    [SerializeField] bool InteractedWith;
}

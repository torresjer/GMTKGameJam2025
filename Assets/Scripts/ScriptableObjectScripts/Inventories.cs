using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Inventories", menuName = "Scriptable Objects/Inventories")]
public class Inventories : ScriptableObject
{
    [SerializeField] string InventoryName;
    [SerializeField] List<ScriptableObject> Inventory;
    [SerializeField] int currentInventoryIndex;
    [SerializeField] int maxInventoryCapacity;
}

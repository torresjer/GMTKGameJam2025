using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "Inventories", menuName = "Scriptable Objects/Inventories")]
public class Inventories : ScriptableObject
{

    public GameEvent OnDataChanged;

    [SerializeField] string InventoryName;
    [SerializeField] List<ScriptableObject> Inventory;
    [SerializeField] int currentInventoryIndex;
    [SerializeField] int maxInventoryCapacity;
    public int MaxInventoryCapacity
    {
        get => maxInventoryCapacity;
        set
        {
            if (maxInventoryCapacity == value) return;
            maxInventoryCapacity = value;
            OnDataChanged?.Raise();
        }
    }
    private void OnValidate()
    {
        OnDataChanged?.Raise();
    }
    public string GetInventoryName() { return InventoryName; }
    public List<ScriptableObject> GetInventory() { return Inventory; }
    public int GetCurrentInventoryIndex() { return currentInventoryIndex; }
    public int GetMaxInventoryCapacity() { return maxInventoryCapacity; }
}

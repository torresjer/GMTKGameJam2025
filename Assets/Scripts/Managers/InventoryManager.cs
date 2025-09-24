using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InventoryManager : Singleton<InventoryManager>
{

    Dictionary<string, Inventories> _inventoriesByName = new Dictionary<string, Inventories>();
    AsyncOperationHandle<IList<Inventories>> _loadedinventories;
    bool _loaded = false;
    public bool IsReady => _loaded;

    public event Action OnInventoriesLoaded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();
        LoadAllInventoriesByAssetGroupLabel("InventoriesData");
    }

    async void LoadAllInventoriesByAssetGroupLabel(string label)
    {

        _loadedinventories = Addressables.LoadAssetsAsync<Inventories>(label, (Inventories inventories) =>
        {

            if (inventories != null)
            {
                Debug.Log(inventories.name);
                _inventoriesByName[inventories.name] = inventories;
            }
        });
        await _loadedinventories.Task;
        _loaded = true;
        Debug.Log($"Loaded {_inventoriesByName.Count} items.");
        OnInventoriesLoaded?.Invoke();
    }
    public Inventories GetInventoryByName(string name)
    {
        if (_inventoriesByName.TryGetValue(name, out Inventories inventory))
            return inventory;
        Debug.LogWarning($"Item with ID {name} not found.");
        return null;
    }
    private void OnDestroy()
    {
        if (_loadedinventories.IsValid())
        {
            Addressables.Release(_loadedinventories);
        }
    }
}

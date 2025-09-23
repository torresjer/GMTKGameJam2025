using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : Singleton<UIManager>
{

    int currentInventorySpacesAvailable;

    float lastInventoryInputValue = 0;
    float currentInventoryInputValue = 0;
    PlayerInventoryUI currentPlayerInventoryUI;

    private void OnEnable()
    {
        currentPlayerInventoryUI = new PlayerInventoryUI();
        StartCoroutine(currentPlayerInventoryUI.LoadPlayerInventoryAsset("PlayerInventoryUI"));
    }

    private void Update()
    {
        InventoryInput();
    }
   
    void InventoryInput()
    {
        // guard clause until UI is loaded
        if (currentPlayerInventoryUI == null || !currentPlayerInventoryUI.IsReady)
            return;

        // The Left axis of the InteractionInput is Tab (mapped to -1.0)
        currentInventoryInputValue = PlayerInput.Instance.GetPlayerInteractionInput().x;

        // only fires once when the axis goes below -0.5f
        bool pressed = currentInventoryInputValue < -0.5f && lastInventoryInputValue >= -0.5f;

        if (pressed)
        {
            currentPlayerInventoryUI.ToggleUI();
        }

        lastInventoryInputValue = currentInventoryInputValue;
        
    }

    public class PlayerInventoryUI
    {
      
        public Inventories playerInventoryData { get; private set; }
        public List<GameObject> PlayerInventoryUIGameObjects { get; private set; }
        public List<GameObject> PlayerInventoryRows { get; private set; }
        public List<GameObject> PlayerInventorySlots { get; private set; }

        bool InventoryOpen = false;
        public GameObject playerInventoryUI { get; private set; }
        public bool IsReady => playerInventoryUI != null;
        
       
        public PlayerInventoryUI() 
        {
            //This Corutine Intializes The PlayerInventoryUI Asynchronously.
           
        }
        public IEnumerator LoadPlayerInventoryAsset(string assetAddress)
        {
            //Loads the Asset Asynchronously.
            AsyncOperationHandle<GameObject> asset = Addressables.LoadAssetAsync<GameObject>(assetAddress);

            yield return asset;

            if(asset.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = asset.Result;
                playerInventoryUI = GameObject.Instantiate(prefab);
                playerInventoryUI.SetActive(false);

                Debug.Log($"Successfully loaded asset: {playerInventoryUI.name}");

                PlayerInventoryUIGameObjects = new List<GameObject>();
                PlayerInventoryRows = new List<GameObject>();
                PlayerInventorySlots = new List<GameObject>();

                if(InventoryManager.Instance != null)
                    playerInventoryData = InventoryManager.Instance.GetInventoryByName("Player Inventory");
           

                GetAllPlayerInventoryUIGameObjects();

            }
            else
            {
                Debug.LogError($"Failed to load asset at address: {assetAddress}");
            }

        
        }
        public void ToggleUI()
        {
            InventoryOpen = !InventoryOpen; ;
            playerInventoryUI.SetActive(InventoryOpen);
        }
        void GetAllPlayerInventoryUIGameObjects()
        {
            foreach (Transform childTransform in playerInventoryUI.transform)
            {
                PlayerInventoryUIGameObjects.Add(childTransform.gameObject);
                Debug.Log(childTransform.gameObject.name);
            }
        }
    }

}

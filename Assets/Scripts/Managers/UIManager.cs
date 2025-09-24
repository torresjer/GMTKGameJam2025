using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameEvent playerUIDataChangeGameEvent;
    int currentInventorySpacesAvailable;

    float lastInventoryInputValue = 0;
    float currentInventoryInputValue = 0;
    PlayerInventoryUI currentPlayerInventoryUI;

    private void OnEnable()
    {
        currentPlayerInventoryUI = new PlayerInventoryUI();
        StartCoroutine(currentPlayerInventoryUI.LoadPlayerInventoryAsset("PlayerInventoryUI", playerUIDataChangeGameEvent));
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
    private void OnDestroy()
    {
        if(currentPlayerInventoryUI != null)
        {
            currentPlayerInventoryUI.ShutPlayerInventoryUIDown();
        }
    }

    public class PlayerInventoryUI
    {
        const int MAX_SLOTS_AVAILABLE = 15;
        const int MAX_SLOTS_PER_ROW = 5;
        
        private GameEvent _gameEvent;
        private Action _onEventCallback;


        public Inventories playerInventoryData { get; private set; }
        public List<GameObject> PlayerInventoryUIGameObjects { get; private set; }
        public List<GameObject> PlayerInventoryRows { get; private set; }
        Dictionary<string, List<GameObject>> slotsForInventoryRows = new Dictionary<string, List<GameObject>>();
        bool InventoryOpen = false;
        public GameObject playerInventoryUI { get; private set; }
        public bool IsReady => playerInventoryUI != null;
        public IEnumerator LoadPlayerInventoryAsset(string assetAddress, GameEvent gameEvent)
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

                _gameEvent = gameEvent;
                _onEventCallback = IntializeInventoryUI;
                _gameEvent.RegisterListener(_onEventCallback);

                PlayerInventoryUIGameObjects = new List<GameObject>();
                PlayerInventoryRows = new List<GameObject>();

                if(InventoryManager.Instance.IsReady)
                {
                    IntializeInventoryUI();
                }
                else
                {
                    InventoryManager.Instance.OnInventoriesLoaded += IntializeInventoryUI;
                }

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
        void IntializeInventoryUI()
        {
            InventoryManager.Instance.OnInventoriesLoaded -= IntializeInventoryUI;
            playerInventoryData = InventoryManager.Instance.GetInventoryByName("PlayerInventory");
            if (playerInventoryData == null) 
            {
                Debug.LogError("Player Inventory not Found!");
                return;
            }

            GetAllPlayerInventoryUIGameObjects();
            GetPlayerInventoryRows();
            IntializeRowsBasedOnAvailableInventorySlots();
        }
        void GetAllPlayerInventoryUIGameObjects()
        {
            foreach (Transform childTransform in playerInventoryUI.transform)
            {
                PlayerInventoryUIGameObjects.Add(childTransform.gameObject);
                Debug.Log(childTransform.gameObject.name);
            }

        }
        void GetPlayerInventoryRows()
        {
            PlayerInventoryUIGameObjects.ForEach(gameObject =>
            {
                //Getting the rows 
                if (gameObject.CompareTag("InventoryRow"))
                {
                    PlayerInventoryRows.Add(gameObject);
                    gameObject.SetActive(false);
                }
            });
        }
        void IntializeRowsBasedOnAvailableInventorySlots()
        {
            Debug.Log(playerInventoryData.GetMaxInventoryCapacity());

            int playerInventoryCapcity = playerInventoryData.GetMaxInventoryCapacity();
            if ( playerInventoryCapcity > MAX_SLOTS_AVAILABLE)
                playerInventoryCapcity = MAX_SLOTS_AVAILABLE;
            if (playerInventoryCapcity < 0)
                playerInventoryCapcity = 0;
            
            int activeRows = playerInventoryCapcity / MAX_SLOTS_PER_ROW;
            int slotsRemaining = playerInventoryCapcity % MAX_SLOTS_PER_ROW;
            if (slotsRemaining > 0)
                activeRows += 1;

            Debug.Log($"active rows {activeRows} slots reamaining {slotsRemaining}");
            for(int i = 0; i < activeRows; i++)
            {
                PlayerInventoryRows[i].SetActive(true);
                List<GameObject> slotsForCurrentRow = new List<GameObject>();
                foreach(Transform childTransform in PlayerInventoryRows[i].transform)
                {
                    slotsForCurrentRow.Add(childTransform.gameObject);
                }
                slotsForInventoryRows.Add(PlayerInventoryRows[i].name, slotsForCurrentRow);

                int slotsToActivate;
                if (i < activeRows - 1)
                {
                    // Full row
                    slotsToActivate = MAX_SLOTS_PER_ROW;
                }
                else
                {
                    // Last row might not be full
                    slotsToActivate = (slotsRemaining > 0) ? slotsRemaining : MAX_SLOTS_PER_ROW;
                }

                // Activate only the slots we need
                for (int j = 0; j < slotsToActivate; j++)
                {
                    slotsForCurrentRow[j].SetActive(true);
                }

            }
          
        }
        public void ShutPlayerInventoryUIDown()
        {
            _gameEvent.UnregisterListener(_onEventCallback);
        }
    }

}

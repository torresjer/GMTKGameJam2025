using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : Singleton<UIManager>
{
    GameObject InventoryUI;
    List<GameObject> InventoryRows;
    List<GameObject> InventorySlots;
    bool InventoryOpen = false;
    float lastInventoryInputValue = 0;
    float currentInventoryInputValue = 0;


    private void OnEnable()
    {
        InventoryUI = GameObject.FindGameObjectWithTag("InventoryUI");
        InventoryUI.SetActive(InventoryOpen);
        InventoryRows = new List<GameObject>();
        InventorySlots = new List<GameObject>();
    }

    private void Update()
    {
        currentInventoryInputValue = PlayerInput.Instance.GetPlayerInteractionInput().x;

        // The Left axis of the InteractionInput is Tab (mapped to -1.0)
        if (InventoryUI != null)
        {
            bool pressed = currentInventoryInputValue < -0.5f && lastInventoryInputValue >= -0.5f;
            // only fires once when the axis goes below -0.5f

            if (pressed)
            {
                InventoryOpen = !InventoryOpen;
                if (InventoryOpen)
                {
                    foreach (Transform childTransform in InventoryUI.transform) 
                    { 
                        InventoryRows.Add(childTransform.gameObject);
                        Debug.Log(childTransform.gameObject.name);
                    }
                }

                InventoryUI.SetActive(InventoryOpen);

            }

            lastInventoryInputValue = currentInventoryInputValue;
        }
    }

}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractableDetector : MonoBehaviour
{
    [SerializeField] List<Sprite> playerIndicatatorSprites;

    Sprite currentPlayerIndicatorSprite { get; set; }
    SpriteRenderer playerIndicatorSpriteRender { get; set; }

    GameObject currentInteractableObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIndicatorSpriteRender = GameObject.FindGameObjectWithTag("PlayerIndicator").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        CheckIfInteractingWhileInTrigger();
    }
    void CheckIfInteractingWhileInTrigger()
    {
        //The up axis of the InteractionInput is the E Key which is listed on the Input Map.
        if (currentInteractableObject != null && PlayerInput.Instance.GetPlayerInteractionInput().y > 0)
        {
            Debug.Log("Interacted!");
            Destroy(currentInteractableObject);
            currentInteractableObject = null; // clear reference
            playerIndicatorSpriteRender.sprite = null;
        }
    }

        
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("InteractableObject"))
        {
            Debug.Log("triggered");
            currentInteractableObject = other.gameObject;
            SetPlayerIndicator();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject == currentInteractableObject)
        {
            currentInteractableObject = null;
            playerIndicatorSpriteRender.sprite = null;
        }
        
    }
    void SetPlayerIndicator()
    {
        if ((playerIndicatatorSprites != null) && (playerIndicatatorSprites.Count > 0))
                currentPlayerIndicatorSprite = playerIndicatatorSprites[0];

            playerIndicatorSpriteRender.sprite = currentPlayerIndicatorSprite;
    }
}

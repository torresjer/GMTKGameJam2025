using UnityEngine;

public class PlayerInput : Singleton<PlayerInput>
{
    Vector2 currentPlayerInteractionInput = Vector2.zero;
    Vector2 currentPlayerMovementInput = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerMovementInput = new Vector2(InputManager.Instance.GetMovementInput().x, InputManager.Instance.GetMovementInput().y);
        currentPlayerInteractionInput = new Vector2(InputManager.Instance.GetInteractionInput().x, InputManager.Instance.GetInteractionInput().y);
    }

    public Vector2 GetPlayerMovementInput() {  return currentPlayerMovementInput; }
    public Vector2 GetPlayerInteractionInput() { return currentPlayerInteractionInput; }
}

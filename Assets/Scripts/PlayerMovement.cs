using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputManager _MovementInput;
    [SerializeField] float _playerMovementSpeed;
    Vector2 currentPlayerInput = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
        UpdateMovement();
    }
    void CheckForInput()
    {
        currentPlayerInput = new Vector2(_MovementInput.GetMovementInput().x, _MovementInput.GetMovementInput().y);

    }
    void UpdateMovement()
    { 
        Vector3 movementInput = new Vector3(currentPlayerInput.x,currentPlayerInput.y, 0); ;

        if (movementInput.magnitude > 1.0f)
            movementInput.Normalize();

        Vector3 changeInMovement = (transform.right * movementInput.x * _playerMovementSpeed + transform.up * movementInput.y * _playerMovementSpeed) * Time.deltaTime;
        Debug.Log(changeInMovement);
        Vector3 nextPos = transform.position + changeInMovement;

        transform.position = GameManager.Instance.ClampPosToLvlBounds(nextPos);
        GameManager.Instance.SetPlayerPos(transform.position);
    }

   
}

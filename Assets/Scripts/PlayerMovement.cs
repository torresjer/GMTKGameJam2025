using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputManager _MovementInput;

    [SerializeField] float _playerMovementSpeed;

    Animator playerAnimator;
    SpriteRenderer playerRenderer;
    Vector2 currentPlayerInput = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerAnimator != null ) 
        playerAnimator.runtimeAnimatorController = AnimationManager.Instance.GetPlayerAnimator();
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
        if(currentPlayerInput!= Vector2.zero)
        {
            if(playerAnimator != null)
                playerAnimator.SetBool("IsWalking", true);
            if(playerRenderer != null && currentPlayerInput.x < 0)
            {
                Debug.Log("triggered");
                playerRenderer.flipX = true;
            }
            else
            {
                playerRenderer.flipX = false;
            }
                
        }
        else
        {
            if (playerAnimator != null)
                playerAnimator.SetBool("IsWalking", false);
            if (playerRenderer != null)
                playerRenderer.flipX = false;
        }
    }
    void UpdateMovement()
    { 
        Vector3 movementInput = new Vector3(currentPlayerInput.x,currentPlayerInput.y, 0); ;

        if (movementInput.magnitude > 1.0f)
        {
            movementInput.Normalize();
        }
     
        Vector3 changeInMovement = (transform.right * movementInput.x * _playerMovementSpeed + transform.up * movementInput.y * _playerMovementSpeed) * Time.deltaTime;
        Debug.Log(changeInMovement);
        Vector3 nextPos = transform.position + changeInMovement;

        transform.position = GameManager.Instance.ClampPosToLvlBounds(nextPos);
        
        GameManager.Instance.SetPlayerPos(transform.position);
    }

   
}

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float _playerMovementSpeed;

    Animator playerAnimator;
    SpriteRenderer playerRenderer;
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
       
        if(PlayerInput.Instance.GetPlayerMovementInput() != Vector2.zero)
        {
            if(playerAnimator != null)
                playerAnimator.SetBool("IsWalking", true);
            if(playerRenderer != null && PlayerInput.Instance.GetPlayerMovementInput().x < 0)
            {
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
        Vector3 movementInput = new Vector3(PlayerInput.Instance.GetPlayerMovementInput().x, PlayerInput.Instance.GetPlayerMovementInput().y, 0); ;

        if (movementInput.magnitude > 1.0f)
        {
            movementInput.Normalize();
        }
     
        Vector3 changeInMovement = (transform.right * movementInput.x * _playerMovementSpeed + transform.up * movementInput.y * _playerMovementSpeed) * Time.deltaTime;
        Vector3 nextPos = transform.position + changeInMovement;

        transform.position = GameManager.Instance.ClampPosToLvlBounds(nextPos);
        
        GameManager.Instance.SetPlayerPos(transform.position);
    }

   
}

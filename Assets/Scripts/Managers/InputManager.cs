using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputManager : Singleton<InputManager>
{

    [SerializeField] bool IsLeftClickPressed;
    [SerializeField] bool IsRightClickPressed;
    [SerializeField] Vector2 MouseScreenPositon;
    [SerializeField] Vector3 MouseWorldPosition;


    [SerializeField] Vector2 InteractionInputs = Vector2.zero;
    [SerializeField] Vector2 MovementInput = Vector2.zero;
    [SerializeField] Vector2 ScrollWheelInput = Vector2.zero;

    InputMap _Input = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        _Input = new InputMap();

        _Input.PlayerControls.Enable();

        _Input.PlayerControls.Movement.performed += SetMovementInput;
        _Input.PlayerControls.Movement.canceled += SetMovementInput;

        _Input.PlayerControls.Interactions.performed += SetInteractionInput;
        _Input.PlayerControls.Interactions.canceled += SetInteractionInput;

        _Input.PlayerControls.ScrollWheel.performed += SetScrollWheelInput;
        _Input.PlayerControls.ScrollWheel.canceled += SetScrollWheelInput;

    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseInput();
        Debug.DrawRay(MouseWorldPosition, Vector3.up * 0.5f, Color.red);
    }
    private void OnDisable()
    {

        _Input.PlayerControls.Movement.performed -= SetMovementInput;
        _Input.PlayerControls.Movement.canceled -= SetMovementInput;

        _Input.PlayerControls.Interactions.performed -= SetInteractionInput;
        _Input.PlayerControls.Interactions.canceled -= SetInteractionInput;

        _Input.PlayerControls.ScrollWheel.performed -= SetScrollWheelInput;
        _Input.PlayerControls.ScrollWheel.canceled -= SetScrollWheelInput;
    }
    // Update is called once per frame
    public bool GetisLeftClickPressed() { return IsLeftClickPressed; }
    public bool GetisRightClickPressed() { return IsRightClickPressed; }
    public Vector3 GetMouseWorldPosition() { return MouseWorldPosition; }
    public Vector2 GetMovementInput() { return MovementInput; }
    public Vector2 GetInteractionInput() { return InteractionInputs; }
    public Vector2 GetScrollWheelInput() { return ScrollWheelInput; }
    void CheckMouseInput()
    {
        if (Mouse.current != null)
        {
            IsLeftClickPressed = Mouse.current.leftButton.wasPressedThisFrame;
            IsRightClickPressed = Mouse.current.rightButton.wasPressedThisFrame;
            MouseScreenPositon = Mouse.current.position.ReadValue();
            MouseWorldPositionCalculations();

        }
    }
    void MouseWorldPositionCalculations()
    {
        Vector3 screenPos = new Vector3(MouseScreenPositon.x, MouseScreenPositon.y, 0f);
        MouseWorldPosition = Camera.main.ScreenToWorldPoint(screenPos);
        MouseWorldPosition.z = 0f; 

    }
    void SetMovementInput(InputAction.CallbackContext ctx)
    {
        MovementInput = ctx.ReadValue<Vector2>();
    }
    void SetInteractionInput(InputAction.CallbackContext ctx)
    {
        InteractionInputs = ctx.ReadValue<Vector2>();
    }
    void SetScrollWheelInput(InputAction.CallbackContext ctx)
    {
        ScrollWheelInput = ctx.ReadValue<Vector2>();
    }

}


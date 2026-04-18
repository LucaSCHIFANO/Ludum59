using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RobotCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D bc;

    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 currentJoystickPosition;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    private bool isJumping;
    private bool isJumpingRecorded;
    private bool isJumpingLast;
    [SerializeField] private float jumpTime;
    private float currentJumpTime;

    [SerializeField] private float gravityJump;
    [SerializeField] private float gravityFall;

    [SerializeField] float extraHeightBelow;
    [SerializeField] private LayerMask ground;


    private int inputId;
    
    [SerializeField] private CurrentMode currentMode;
    private List<InputClass> inputList = new List<InputClass>();

    public delegate void ModeHandler(CurrentMode mode);
    public event ModeHandler ModeChange;

    private CurrentState currentState;

    public delegate void StateHandler(CurrentState mode);
    public event StateHandler StateChange;

    public enum CurrentMode
    {
        Looking,
        Recording,
        Playing,
        RealTimePlaying,
    }

    public enum CurrentState
    {
        Alive,
        Dead,
        Win,
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
        inputId = 0;
    }

    private void FixedUpdate()
    {
        switch (currentMode)
        {
            case CurrentMode.Recording:
                RecordInput();
                break;
            case CurrentMode.Playing:
                ReadInput();
                break;
            case CurrentMode.RealTimePlaying:
                Movement(currentJoystickPosition.x);
                Jump(ref isJumping);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        
    }


    private void Movement(float currentJoystickX)
    {

        float joystickX = currentJoystickX;
        if (joystickX != 0) joystickX = Mathf.Sign(joystickX);

        float horizontalMovement = 0;
        float verticalMovement = 0;

        horizontalMovement = joystickX * speed;
        verticalMovement = rb.linearVelocity.y;


        rb.linearVelocity = new Vector2(horizontalMovement, verticalMovement);


    }

    private void Jump(ref bool _isJumping)
    {
        if (currentJumpTime <= 0) _isJumping = false;
        
        if (_isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            currentJumpTime -= Time.deltaTime;
            rb.gravityScale = gravityJump;
        }
        else rb.gravityScale = gravityFall;
    }

    public bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center,
            bc.bounds.size, 0f, Vector2.down, extraHeightBelow, ground);

        if (raycastHit.collider != null)
        {
            return raycastHit.collider != null;
        }

        return false;
    }

    private void ChangeMode(CurrentMode newMode)
    {
        currentMode = newMode;
        ModeChange.Invoke(currentMode);
    }

    private void ChangeState(CurrentState newState)
    {
       currentState = newState;
        StateChange.Invoke(currentState);
    }

    #region Inputs
    private void RecordInput()
    {
        inputList.Add(new InputClass(currentJoystickPosition.x, isJumping, !isJumpingLast && isJumping));
        isJumpingLast = isJumping;
    }

    private void ReadInput()
    {
        if (inputId < inputList.Count)
        {
            Movement(inputList[inputId].joystickXPosition);

            if (inputList[inputId].jumpPerformed && IsGrounded()) 
                currentJumpTime = jumpTime;
            
            if (!inputList[inputId].jumpButtonPressed && isJumpingLast)
                currentJumpTime = 0;

            isJumpingRecorded = inputList[inputId].jumpButtonPressed;

            Jump(ref isJumpingRecorded);

            isJumpingLast = inputList[inputId].jumpButtonPressed;
            inputId++;
        }
        else if(currentState != CurrentState.Dead)
        {
            ChangeState(CurrentState.Dead); 
        }
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        currentJoystickPosition = new Vector2(context.ReadValue<float>(), 0);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            isJumping = true;
            if (currentMode == CurrentMode.RealTimePlaying && IsGrounded())
                currentJumpTime = jumpTime;
        }
        else if (context.canceled)
        {
            isJumping = false;
            if (currentMode == CurrentMode.RealTimePlaying)
                currentJumpTime = 0;
        }
    }

    public void EndRecordingInput(InputAction.CallbackContext context)
    {
        if (context.performed && currentMode == CurrentMode.Looking)
        {
            ChangeMode(CurrentMode.Recording);
        }
        else if(context.performed && currentMode == CurrentMode.Recording)
        {
            ChangeMode(CurrentMode.Playing);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
            ChangeState(CurrentState.Win);
        else if (collision.tag == "Death")
            ChangeState(CurrentState.Dead);
    }
}

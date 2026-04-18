using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RobotCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

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


    [SerializeField] private CurrentMode currentMode;
    private List<InputClass> inputList = new List<InputClass>();

    private int inputId;

    enum CurrentMode
    {
        Recording,
        Playing,
        RealTimePlaying,
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        if (currentJumpTime < 0) _isJumping = false;
        
        if (_isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            currentJumpTime -= Time.deltaTime;
            rb.gravityScale = gravityJump;
        }
        else rb.gravityScale = gravityFall;



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

            if (inputList[inputId].jumpPerformed) 
            { 
                currentJumpTime = jumpTime;
            }
            isJumpingRecorded = inputList[inputId].jumpButtonPressed;

            Jump(ref isJumpingRecorded);
            inputId++;
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
            currentJumpTime = jumpTime;
        }
        else if (context.canceled) isJumping = false;
    }

    public void EndRecordingInput(InputAction.CallbackContext context)
    {
        if (context.performed && currentMode == CurrentMode.Recording)
        {
            currentMode = CurrentMode.Playing;
        }else if(context.performed && currentMode == CurrentMode.Playing)
        {
            currentMode = CurrentMode.Recording;
        }
    }
    #endregion
}

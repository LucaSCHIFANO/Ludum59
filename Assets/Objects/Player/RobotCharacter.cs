using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RobotCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D bc;

    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

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

    [Header("Antenna")]
    [SerializeField] private Animator antennaAnim;

    
    private int inputId;

    //Events & States
    [SerializeField] private CurrentMode currentMode;
    private List<InputClass> inputList = new List<InputClass>();

    public delegate void ModeHandler(CurrentMode mode);
    public event ModeHandler    ModeChange;

    private CurrentState currentState;
    private bool stopRecording;

    public delegate void StateHandler(CurrentState mode);
    public event StateHandler StateChange;

    //different animation if you die by traps (not implemented)
    private bool isDeadByHazard = false;
    private float winTimer;

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
        if (currentState != CurrentState.Alive)
            return;

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
        SetAnimations(); 
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
        
        StartCoroutine(ChangeModeCall(newMode));
    }

    IEnumerator ChangeModeCall(CurrentMode newMode)
    {
        switch (newMode)
        {
            case CurrentMode.Recording:
                antennaAnim.Play("AntennaSpawn");
                stopRecording = false;
                break;
            case CurrentMode.Playing:
                antennaAnim.Play("AntennaReceive");
                stopRecording = true;
                ModeChange?.Invoke(newMode);
                yield return new WaitForSeconds(0.5f);
                antennaAnim.Play("AntennaNo");
                break;
            default:
                break;
        }
        currentMode = newMode;
        ModeChange?.Invoke(currentMode);
    }

    private void ChangeState(CurrentState newState)
    {
       currentState = newState;
       StateChange?.Invoke(currentState);
    }

    #region Inputs
    private void RecordInput()
    {
        if (stopRecording)
            return;

        inputList.Add(new InputClass(currentJoystickPosition.x, isJumping, !isJumpingLast && isJumping));
        isJumpingLast = isJumping;

        if(inputList.Count > 5000)
        {
            ChangeMode(CurrentMode.Playing);
        }
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
            Death(false);
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

    public void ShowTimingInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.ShowInfo(true);
        }
        else if (context.canceled)
        {
            GameManager.Instance.ShowInfo(false);
        }
    }

    public void QuitInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(currentMode == CurrentMode.Playing)
                LevelManager.Instance.LoadCurrentScene(true);
            else
                LevelManager.Instance.LoadMainMenu(true);
        }
    }
    #endregion

    private void SetAnimations()
    {
        if (currentMode == CurrentMode.Recording)
        {
            anim.Play("PlayerIdle2");
        }
        else
        {
            switch (currentState)
            {
                case CurrentState.Alive:
                    var running = false;
                    if (rb.linearVelocityX > 0)
                    {
                        sr.flipX = false;
                        running = true;
                    }
                    else if (rb.linearVelocityX < 0)
                    {
                        sr.flipX = true;
                        running = true;
                    }

                    if (IsGrounded())
                    {
                        if (running)
                            anim.Play("PlayerRun");
                        else anim.Play("PlayerIdle");
                    }
                    else
                    {
                        anim.Play("PlayerJump");
                    }
                    break;

                case CurrentState.Dead:
                    /*if(isDeadByHazard)
                    {
                        anim.Play("PlayerDeath");
                    }else*/
                    if (IsGrounded())
                    {
                        anim.Play("PlayerDeath2");
                    }
                    break;

                case CurrentState.Win:
                    winTimer += Time.deltaTime;
                    if (winTimer <= 0.5f)
                        anim.Play("PlayerWin");
                    else
                        anim.Play("PlayerWin2");
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit" && currentState == CurrentState.Alive)
            Victory(collision);
        else if (collision.tag == "Death" && currentState == CurrentState.Alive)
            Death(true);
    }

    public void Victory(Collider2D collision)
    {
        ChangeState(CurrentState.Win);
        transform.position = collision.gameObject.transform.position;
        rb.linearVelocity = Vector2.zero;
    }

    public void Death(bool _isDeadByHazard)
    {
        isDeadByHazard = _isDeadByHazard;
        rb.linearVelocity = Vector2.zero;
        ChangeState(CurrentState.Dead);
    }
}

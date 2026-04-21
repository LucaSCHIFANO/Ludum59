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
    [SerializeField] private SOSound jumpSound;

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

    /// <summary>
    /// Will be executed 50 per second.
    /// The mode will determine which action will be done
    /// </summary>
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

    /// <summary>
    /// Only manages the player animations
    /// </summary>
    void Update()
    {
        SetAnimations(); 
    }

    /// <summary>
    /// Receive an input and move the character accordingly
    /// </summary>
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

    /// <summary>
    //// Receive an input and makes the character jump if needed
    /// </summary>
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

    /// <summary>
    /// Check if the character is on the ground with a small raycast at the edge of the collider
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Allow the player to change mode
    /// </summary>
    private void ChangeMode(CurrentMode newMode)
    {
        StartCoroutine(ChangeModeCall(newMode));
    }

    /// <summary>
    /// The Coroutine is here to handle the animations and to send info through delegate (with invoke)
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Allow the player to change state and to send info through delegate (with invoke)
    /// </summary>
    private void ChangeState(CurrentState newState)
    {
       currentState = newState;
       StateChange?.Invoke(currentState);
    }

    #region Inputs
    /// <summary>
    /// In Recording mode, will store inputs in <see cref="InputClass"/>. Cannot exced 5000 (100 seconds);
    /// </summary>
    private void RecordInput()
    {
        if (stopRecording)
            return;

        inputList.Add(new InputClass(currentJoystickPosition.x, isJumping, !isJumpingLast && isJumping));
        isJumpingLast = isJumping;

        if(inputList.Count > 5000) // better to use a field x)
        {
            ChangeMode(CurrentMode.Playing);
        }
    }

    /// <summary>
    /// In Playing mode, will read inputs in <see cref="InputClass"/> and 
    /// send them in the <see cref="Movement(float)"/> and <see cref="Jump(ref bool)"/> functions
    /// </summary>
    private void ReadInput()
    {
        if (inputId < inputList.Count)
        {
            Movement(inputList[inputId].joystickXPosition);

            if (inputList[inputId].jumpPerformed && IsGrounded())
            {
                SoundManager.Instance.Play(jumpSound);
                currentJumpTime = jumpTime;
            }
            
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

    /// <summary>
    /// Store movements input to be used later in <see cref="Movement(float)"/> or <see cref="ReadInput"/>
    /// </summary>
    /// <param name="context"></param>
    public void MovementInput(InputAction.CallbackContext context)
    {
        currentJoystickPosition = new Vector2(context.ReadValue<float>(), 0);
    }

    /// Store jump input to be used later in <see cref="Jump(ref bool)"/> or <see cref="ReadInput"/>
    public void JumpInput(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            isJumping = true;
            if (currentMode == CurrentMode.RealTimePlaying && IsGrounded())
            {
                currentJumpTime = jumpTime;
                SoundManager.Instance.Play(jumpSound);
            }
        }
        else if (context.canceled)
        {
            isJumping = false;
            if (currentMode == CurrentMode.RealTimePlaying)
                currentJumpTime = 0;
        }
    }

    /// Allow the player to change state

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

    /// Allow the player to show the info on the hazards in the level
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

    /// Allow the player return to the main menu
    public void QuitInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LevelManager.Instance.LoadMainMenu(true);
        }
    }

    /// Allow the player to reset the current level
    public void ResetInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentMode == CurrentMode.Playing || currentMode == CurrentMode.Recording)
                LevelManager.Instance.LoadCurrentScene(true);
        }
    }
    #endregion

    /// <summary>
    /// Change player animation
    /// </summary>
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
    
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GroundChecker))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PoseCharacterController))]
[RequireComponent(typeof(CeilingChecker))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterAudioController))]
public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController Instance;

    private Animator animator;
    private GroundChecker groundChecker;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private PoseCharacterController poseController;    
    private CeilingChecker ceilingChecker;
    private PlayerInput playerInput;
    private CharacterAudioController audioController;

    [Header("Drag these")]
    [SerializeField] private Transform itemPivotTransform;
    
    [Header("Characteristics")] 
    [SerializeField] private float speed;
    [SerializeField] private float controllableAcceleration;
    [SerializeField] private float uncontrollableAcceleration;
    [SerializeField] private float controllableDeceleration;
    [SerializeField] private float uncontrollableDeceleration;
    [SerializeField] private float lyingDeceleration;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpHoldTime;
    [SerializeField] private float duckToLieSpeedTreshold;
    [SerializeField] private float platformIgnoreCollisionTime;
    [SerializeField] private int playerLayer;
    [SerializeField] private int ignorePlatformLayer;

    public Action OnStartMoving;
    public Action OnEndMoving;

    private Coroutine jumpHoldCoroutine;

    private float movementHorizontal;

    private bool isDuck = false;
    private bool isMoving;
    private bool wasMoving = false;
    private bool isJumping;
    private bool isFacingRight = true;
    private bool isAbleToMove = true;
    

    void Flip()
    {
        if (poseController.pose == Pose.LYING) return;
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        groundChecker.FlipHorizontally();
        
    }
    void Animate()
    {
        if (isFacingRight && movementHorizontal < 0 || !isFacingRight && movementHorizontal > 0)
        {
            Flip();
        }
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsGrounded", groundChecker.IsGrounded);
    }

    void OnBecomeGrounded()
    {
        if (isMoving == true) OnStartMoving.Invoke();
        if (poseController.pose == Pose.DEFAULT) animator.SetTrigger("ToDefault");
        else if (poseController.pose == Pose.DUCK)
        {
            if (Mathf.Abs(movementHorizontal) > duckToLieSpeedTreshold)
            {
                SetPose(Pose.LYING);
            }
        }
    }

    void OnBecomeNotGrounded()
    {
        OnEndMoving.Invoke();
        if (poseController.pose == Pose.DEFAULT) animator.SetTrigger("ToJump");
    }
    void OnCeilingHit()
    {
        if (jumpHoldCoroutine == null) return;
        StopCoroutine(jumpHoldCoroutine);
    }
    IEnumerator JumpHoldCoroutine()
    {
        for (float timePassed = 0f;
            timePassed < jumpHoldTime && isJumping;
            timePassed += Time.deltaTime)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpVelocity);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator IgnorePlatformsForSecondsCoroutine(float seconds)
    {
        gameObject.layer = ignorePlatformLayer;
        yield return new WaitForSeconds(seconds);
        gameObject.layer = playerLayer;
    }
    
    public void OnMoveInput(CallbackContext context)
    {
        movementHorizontal = context.ReadValue<float>();
        isMoving = movementHorizontal != 0f;
        if (isMoving && !wasMoving && groundChecker.IsGrounded) OnStartMoving.Invoke();
        else if (!isMoving && wasMoving) OnEndMoving.Invoke();
        wasMoving = isMoving;

        if(context.action.activeControl.name != "x")        //FIXME
        {
            PlayerGrabController.Instance.throwDirection.x = movementHorizontal;
        }
    }

    public void OnJumpInput(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Jump();

            if (context.action.activeControl.name != "x")       //FIXME
            {
                PlayerGrabController.Instance.throwDirection.y = 1;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            EndJump();

            if (context.action.activeControl.name != "x")       //FIXME
            {
                PlayerGrabController.Instance.throwDirection.y = 0;
            }
        }
    }

    public void OnDuckInput(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnGetDown();

            if (context.action.activeControl.name != "x")       //FIXME
            {
                PlayerGrabController.Instance.throwDirection.y = -1;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnGetUp();
            if (context.action.activeControl.name != "x")       //FIXME
            {
                PlayerGrabController.Instance.throwDirection.y = 0;
            }
        }
    }
    void EndJump()
    {
        isJumping = false;
    }
    void Jump()
    {
        if (!groundChecker.IsGrounded || isJumping) return;
        if (isDuck && groundChecker.OnPlatform)
        {
            StartCoroutine(IgnorePlatformsForSecondsCoroutine(platformIgnoreCollisionTime));
            return;
        }
        isJumping = true;
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpVelocity);               //FIXME
        jumpHoldCoroutine = StartCoroutine(JumpHoldCoroutine());
        audioController.OnJump();
        if(poseController.pose == Pose.DEFAULT) animator.SetTrigger("ToJump");
    }
    void Start()
    {
        Instance = this;

        animator = GetComponent<Animator>();
        groundChecker = GetComponent<GroundChecker>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        poseController = GetComponent<PoseCharacterController>();
        ceilingChecker = GetComponent<CeilingChecker>();
        playerInput = GetComponent<PlayerInput>();
        audioController = GetComponent<CharacterAudioController>();

        groundChecker.onBecomeGrounded += OnBecomeGrounded;
        groundChecker.onBecomeNotGrounded += OnBecomeNotGrounded;
        ceilingChecker.onCeilingHit += OnCeilingHit;
        OnStartMoving += audioController.OnStartMoving;
        OnEndMoving += audioController.OnEndMoving;
        playerInput.actions.FindActionMap("MapSwitcher").Enable();
    }
    
    void OnGetDown()
    {
        isDuck = true;
        if (poseController.pose == Pose.DEFAULT)
        {
            if (!groundChecker.IsGrounded || !isMoving)
            {
                SetPose(Pose.DUCK);
            }
            else if (isMoving && groundChecker.IsGrounded) 
            {
                SetPose(Pose.LYING);
            }
        }
    }

    void OnGetUp()
    {
        isDuck = false;
        SetPose(Pose.DEFAULT);
    }
    void Update()
    {
        if (groundChecker.IsGrounded && poseController.pose == Pose.DUCK && 
            Mathf.Abs(rigidBody2D.velocity.x) > duckToLieSpeedTreshold)
        {
            SetPose(Pose.LYING);
        }
        Animate();
    }

    private void FixedUpdate()
    {
        if (isAbleToMove && movementHorizontal != 0)
        {
            float acceleration = groundChecker.IsGrounded ? controllableAcceleration : uncontrollableAcceleration;
            float targetSpeed = movementHorizontal * speed;
            if (targetSpeed < 0 && rigidBody2D.velocity.x > targetSpeed ||
                targetSpeed > 0 && rigidBody2D.velocity.x < targetSpeed)
            {
                rigidBody2D.velocity = Vector2.MoveTowards(rigidBody2D.velocity,
                    new Vector2(targetSpeed, rigidBody2D.velocity.y), acceleration * Time.fixedDeltaTime);
            }
        }
        else   //If player doesn't move or is not able to move
        {
            float deceleration;
            if (poseController.pose == Pose.DUCK && !groundChecker.IsGrounded) //if duck in air - deceleration is low
            {
                deceleration = uncontrollableDeceleration;
            }
            else
            {
                deceleration = controllableDeceleration;
            }
            if (poseController.pose == Pose.LYING) deceleration = lyingDeceleration;    //If is lying - apply lying acceleration
            rigidBody2D.velocity = Vector2.MoveTowards(rigidBody2D.velocity,
                new Vector2(0f, rigidBody2D.velocity.y), deceleration * Time.fixedDeltaTime);
        }
    }
    
    void SetZRotation(float angle)
    {
        rigidBody2D.rotation = angle;
    }
    
    private void SetPose(Pose poseToSet)
    {
        switch (poseToSet)
        {
            case Pose.DEFAULT:
                if (poseController.pose == Pose.DEFAULT) break;
                poseController.pose = Pose.DEFAULT;
                SetZRotation(0f);
                if(groundChecker.IsGrounded)
                    animator.SetTrigger("ToDefault");
                else 
                    animator.SetTrigger("ToJump");
                isAbleToMove = true;
                break;
            case Pose.DUCK:
                if (poseController.pose == Pose.DUCK) break;
                poseController.pose = Pose.DUCK;
                SetZRotation(0f);
                animator.SetTrigger("ToDuck");
                isAbleToMove = false; 
                break;
            case Pose.LYING:
                if (poseController.pose == Pose.LYING) break;
                poseController.pose = Pose.LYING;
                if(isFacingRight) SetZRotation(90f);
                else SetZRotation(-90f);
                animator.SetTrigger("ToLie");
                isAbleToMove = false;
                break;
        }
        poseController.ApplyPoseData(poseController.GetCurrentPoseData());
        if(isFacingRight != groundChecker.isFasingRight)
            groundChecker.FlipHorizontally();
    }
    public void OnMenuClose()
    {
        playerInput.actions.FindActionMap("UI").Disable();
        playerInput.actions.FindActionMap("Player").Enable();
    }
    public void OnMenuOpen()
    {
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions.FindActionMap("Player").Disable();
    }
    public void OnEscapeButtonPressed(CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if(EscapeMenuController.Instance.isOpened)
            {
                EscapeMenuController.Instance.CloseMenu();
                OnMenuClose();
            }
            else
            {
                EscapeMenuController.Instance.OpenMenu();
                OnMenuOpen();
            }
        }
    }
    public void OnRestartButtonPressed(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            GameController.Instance.RestartCurrentLevel();
        }
    }
}

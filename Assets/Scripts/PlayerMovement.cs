using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private Player _player;
    private PlayerControllers _controllers;
    private CharacterController _characterController;
    private Animator _animator;

    [Header("Movement info")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private float speed;
    private Vector3 movementDir;
    private float verticalVelocity;
    private bool isRunning;

    [Header("Aim info")]
    [SerializeField] private LayerMask aimLayerMask;
    [FormerlySerializedAs("aimTransform")] [SerializeField] private Transform aim;
    private Vector3 lookingDirection;
    
    
    private Vector2 moveInput;
    private Vector2 aimInput;

    private void AssignedInputSystem()
    {
        _controllers = _player.controllers;

        _controllers.Character.Movement.performed +=
            ctx => moveInput = ctx.ReadValue<Vector2>();
        _controllers.Character.Movement.canceled +=
            _ => moveInput = Vector2.zero;

        _controllers.Character.Aim.performed +=
            ctx => aimInput = ctx.ReadValue<Vector2>();
        _controllers.Character.Aim.canceled +=
            _ => aimInput = Vector2.zero;

        _controllers.Character.Run.performed += _ => isRunning = true;
        _controllers.Character.Run.canceled += _ => isRunning = false;
    }
    

    private void Start()
    {
        _player = GetComponent<Player>();   
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        speed = walkSpeed;
        AssignedInputSystem();
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardMouse();
        AnimationController();
    }

    private void AnimationController()
    {
        float xVelocity = Vector3.Dot(movementDir.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDir.normalized, transform.forward);

        _animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnim = isRunning && movementDir.magnitude > 0;
        _animator.SetBool("isRunning", playRunAnim);
    }

    private void AimTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, aimLayerMask))
        {
            lookingDirection = hit.point - transform.position;
            lookingDirection.y = 0;
            lookingDirection.Normalize();

            transform.forward = lookingDirection;
            aim.position = new Vector3(hit.point.x, transform.position.y + 1, hit.point.z);
        }
    }

    private void ApplyMovement()
    {
        movementDir = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDir.magnitude > 0)
        {
            speed = isRunning ? runSpeed : walkSpeed;
            _characterController.Move(Time.deltaTime * speed * movementDir);
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded == false)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDir.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }
}

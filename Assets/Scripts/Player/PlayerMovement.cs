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
    [SerializeField] private float turnSpeed;
    private float _speed;
    private float _verticalVelocity;

    private Vector3 _movementDir;
    public Vector2 moveInput { get; private set; }

    private bool _isRunning;

    private void AssignedInputSystem()
    {
        _controllers = _player.controllers;

        _controllers.Character.Movement.performed +=
            ctx => moveInput = ctx.ReadValue<Vector2>();
        _controllers.Character.Movement.canceled +=
            _ => moveInput = Vector2.zero;

        _controllers.Character.Run.performed += _ => _isRunning = true;
        _controllers.Character.Run.canceled += _ => _isRunning = false;
    }


    private void Start()
    {
        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _speed = walkSpeed;
        AssignedInputSystem();
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        AnimationController();
    }

    private void AnimationController()
    {
        float xVelocity = Vector3.Dot(_movementDir.normalized, transform.right);
        float zVelocity = Vector3.Dot(_movementDir.normalized, transform.forward);

        _animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnim = _isRunning && _movementDir.magnitude > 0;
        _animator.SetBool("isRunning", playRunAnim);
    }

    private void ApplyRotation()
    {
        Vector3 lookingDirection = _player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0;
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = 
            Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
        
    }

    private void ApplyMovement()
    {
        _movementDir = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (_movementDir.magnitude > 0)
        {
            _speed = _isRunning ? runSpeed : walkSpeed;
            _characterController.Move(Time.deltaTime * _speed * _movementDir);
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded == false)
        {
            _verticalVelocity -= 9.81f * Time.deltaTime;
            _movementDir.y = _verticalVelocity;
        }
        else
        {
            _verticalVelocity = -.5f;
        }
    }
}

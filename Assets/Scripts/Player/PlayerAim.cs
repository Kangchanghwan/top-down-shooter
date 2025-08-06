using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private PlayerControllers _controllers;

    [Header("Aim Visual - Laser")] [SerializeField]
    private LineRenderer aimLaser;
    
    [Header("Aim control")] [SerializeField]
    private Transform aim;

    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingToTarget;
    
    [Header("Camera control")]
    [SerializeField] private Transform cameraTarget;
    [Range(.5f, 1f)] [SerializeField]
    private float minCameraDistance = 1.5f;
    [Range(1f, 3f)] [SerializeField]
    private float maxCameraDistance = 4;
    [SerializeField] private float cameraSensetivity = 5f;
    
    [Space]
    [SerializeField] private LayerMask aimLayerMask;

    private Vector2 _mouseInput;
    private RaycastHit _lastKnownMouseHit;

    void Start()
    {
        _player = GetComponent<Player>();
        AssignInputEvents();
    }
    void Update()
    {
        if (Input.GetKeyDown((KeyCode.P)))
        {
            isAimingPrecisly = !isAimingPrecisly;
        }
        if (Input.GetKeyDown((KeyCode.L)))
        {
            isLockingToTarget = !isLockingToTarget;
        }

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }
    private void UpdateAimVisuals()
    {
        aimLaser.enabled = _player.weapon.WeaponReady();
        if (aimLaser.enabled == false) return;

        WeaponModel weaponModel = _player.weaponVisuals.CurrentWeaponModel();
        
        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);
        
        
        float gunDistance = _player.weapon.CurrentWeapon().gunDistance;
        float laserTipLength = .5f;
        
        Transform gunPoint = _player.weapon.GunPoint();
        Vector3 laserDirection = _player.weapon.BulletDirection();
        
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out var hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0;
        }
        
        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }
    private void UpdateCameraPosition()
    {
        cameraTarget.position =
            Vector3.Lerp(cameraTarget.position, DesieredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }
    private void UpdateAimPosition()
    {
        Transform target = Target();
        if (target != null && isLockingToTarget)
        {
            aim.position = target.position;
            return;
        }

        if (target?.GetComponent<Renderer>() != null)
        {
            aim.position = target.GetComponent<Renderer>().bounds.center;
        }
        else
        {
            aim.position = GetMouseHitInfo().point;
        }

        if (!isAimingPrecisly)
        {
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
        }
    }
    private Vector3 DesieredCameraPosition()
    {
        float actualMaxCameraDistance = _player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;
        
        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        float distanceToDesieredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
        float clampDistance = Mathf.Clamp(distanceToDesieredPosition, minCameraDistance, actualMaxCameraDistance);

        desiredCameraPosition = transform.position + aimDirection * clampDistance;
        desiredCameraPosition.y = transform.position.y + 1;

        return desiredCameraPosition;
    }
    public Transform Target()
    {
        Transform target = null;

        if (GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }
    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mouseInput);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, aimLayerMask))
        {
            _lastKnownMouseHit = hit;
            return hit;
        }
        return _lastKnownMouseHit;
    }
    public bool CanAimPrecisly() => isAimingPrecisly;
    public Transform Aim() => aim;
    private void AssignInputEvents()
    {
        _controllers = _player.controllers;

        _controllers.Character.Aim.performed +=
            ctx => _mouseInput = ctx.ReadValue<Vector2>();
        _controllers.Character.Aim.canceled +=
            _ => _mouseInput = Vector2.zero;
    }
}

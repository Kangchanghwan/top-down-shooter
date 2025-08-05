using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator _anim;
    private Player _player;

    [SerializeField] private WeaponModel[] weaponModels;
    
    [Header("Rig ")] 
    [SerializeField] private float rigWeightIncreaseRate;
    private bool _shouldIncreaseRigWeight;
    private Rig _rig;

    
    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    private bool _shouldIncreaseLeftHandIKWeight;

    private bool _isGrabbingWeapon;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _rig = GetComponentInChildren<Rig>();
        _player = GetComponentInParent<Player>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWeaponSwitch();
        UpdateRigWeight();
        UpdateLeftHandIkWeight();
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;
        WeaponType weaponType = _player.weapon.CurrentWeapon().weaponType;

        foreach (var weapon in weaponModels)
        {
            if (weapon.weaponType == weaponType)
            {
                weaponModel = weapon;
            } 
        }

        return weaponModel;
    }
    
    public void PlayReloadAnimation()
    {
        if (_isGrabbingWeapon) return;
        _anim.SetTrigger("Reload");
        ReduceRigWeight();
    }

    private void UpdateLeftHandIkWeight()
    {
        if (_shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;
            if (leftHandIK.weight >= 1)
            {
                _shouldIncreaseLeftHandIKWeight = false;
            }
        }
    }

    private void UpdateRigWeight()
    {
        if (_shouldIncreaseRigWeight)
        {
            _rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (_rig.weight >= 1)
            {
                _shouldIncreaseRigWeight = false;
            }
        }
    }

    private void ReduceRigWeight()
    {
        _rig.weight = 0.2f;
    }

    public void SetBusyGrabbingWeapon(bool busy)
    {
        _isGrabbingWeapon = busy;
        _anim.SetBool("isGrabbingWeapon", _isGrabbingWeapon);
    }
    public void ReturnRigWeightToOne() => _shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeightToOne() => _shouldIncreaseLeftHandIKWeight = true;
    

    private void SwitchOn()
    {
        SwitchOffWeaponModels();
        CurrentWeaponModel().gameObject.SetActive(true);
        AttachLeftHand();
    }

    private void SwitchOffWeaponModels()
    {
        foreach (var gun in weaponModels)
        {
            gun.gameObject.SetActive(false);   
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _anim.layerCount; i++)
        {
            _anim.SetLayerWeight(i, 0);
        }
        _anim.SetLayerWeight(layerIndex, 1);   
    }

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0;
        ReduceRigWeight();
        _anim.SetFloat("WeaponGrabType", ((float)(grabType)));
        _anim.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeapon(true);
    }
    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }  
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn();
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn();
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
    }
}

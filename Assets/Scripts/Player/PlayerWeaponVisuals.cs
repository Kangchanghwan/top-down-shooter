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
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _rig = GetComponentInChildren<Rig>();
        _player = GetComponent<Player>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
    }
    void Update()
    {
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
    
    public void SetBusyGrabbingWeapon(bool busy)
    {
        _isGrabbingWeapon = busy;
        _anim.SetBool("isGrabbingWeapon", _isGrabbingWeapon);
    }
    
    public void SwitchOnCurrentWeaponModel()
    {
        var currentWeaponModel = CurrentWeaponModel();
        int animationIndex = (int)currentWeaponModel.holdType;
        SwitchAnimationLayer(animationIndex);
        currentWeaponModel.gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        foreach (var gun in weaponModels)
        {
            gun.gameObject.SetActive(false);   
        }
    }
    
    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _anim.layerCount; i++)
        {
            _anim.SetLayerWeight(i, 0);
        }
        _anim.SetLayerWeight(layerIndex, 1);   
    }

    public void PlayWeaponEquipAnimation()
    {
        GrabType grabType = CurrentWeaponModel().grabType;
        leftHandIK.weight = 0;
        ReduceRigWeight();
        _anim.SetFloat("WeaponGrabType", (float)grabType);
        _anim.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeapon(true);
    }
    
    
    #region Animation Rigging Methods
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
    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }

    public void ReturnRigWeightToOne() => _shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeightToOne() => _shouldIncreaseLeftHandIKWeight = true;
    #endregion
  
}

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator _anim;
    private Player _player;

    [SerializeField] private WeaponModel[] weaponModels;
    [SerializeField] private BackUpWeaponModel[] backUpWeaponModels;
    
    [Header("Rig ")] 
    [SerializeField] private float rigWeightIncreaseRate;
    private bool _shouldIncreaseRigWeight;
    private Rig _rig;

    
    [Header("Left Hand IK")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    private bool _shouldIncreaseLeftHandIKWeight;

    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _rig = GetComponentInChildren<Rig>();
        _player = GetComponent<Player>();
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        backUpWeaponModels = GetComponentsInChildren<BackUpWeaponModel>(true);
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

    public void PlayFireAnimation() => _anim.SetTrigger("Fire");
    public void PlayReloadAnimation()
    {
        float  reloadSpeed = _player.weapon.CurrentWeapon().reloadSpeed;
        _anim.SetFloat("ReloadSpeed", reloadSpeed);
        _anim.SetTrigger("Reload");
        
        ReduceRigWeight();
    }
    
    public void SwitchOnCurrentWeaponModel()
    {
        var currentWeaponModel = CurrentWeaponModel();
        int animationIndex = (int)currentWeaponModel.holdType;
        
        SwitchOffWeaponModels();
        SwitchOffBackUpWeaponModels();
        if (_player.weapon.HasOnlyOneWeapon() == false)
        {
            SwitchOnBackUpWeaponModel();
        }
        
        SwitchAnimationLayer(animationIndex);
        currentWeaponModel.gameObject.SetActive(true);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        foreach (var weapon in weaponModels)
        {
            weapon.gameObject.SetActive(false);   
        }
    }

    private void SwitchOffBackUpWeaponModels()
    {
        foreach (var weapon in backUpWeaponModels)
        {
            weapon.gameObject.SetActive(false);   
        } 
    }

    public void SwitchOnBackUpWeaponModel()
    {
        WeaponType weaponType = _player.weapon.BackUpWeapon().weaponType;

        foreach (var weapon in backUpWeaponModels)
        {
            if (weapon.weaponType == weaponType)
            {
                weapon.gameObject.SetActive(true);
            }
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
        EquipType equipType = CurrentWeaponModel().equipAnimationType;

        float equipmentSpeed = _player.weapon.CurrentWeapon().equipSpeed;
        
        leftHandIK.weight = 0;
        ReduceRigWeight();
        _anim.SetTrigger("EquipWeapon");
        _anim.SetFloat("EquipType", (float)equipType);
        _anim.SetFloat("EquipSpeed", equipmentSpeed);
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

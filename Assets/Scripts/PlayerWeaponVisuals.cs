using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class PlayerWeaponVisuals : MonoBehaviour
{
    private Animator _anim;
    private Transform _currentGun;
    
    [SerializeField] private Transform[] gunsTransforms;
    
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform rivolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform rifle;

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
        SwitchOn(pistol);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && _isGrabbingWeapon == false)
        {
            _anim.SetTrigger("Reload");
            ReduceRigWeight();
        }

        UpdateRigWeight();
        UpdateLeftHandIkWeight();
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
        _anim.SetBool("BusyGrabbingWeapon", _isGrabbingWeapon);
    }
    public void ReturnRigWeightToOne() => _shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeightToOne() => _shouldIncreaseLeftHandIKWeight = true;
    

    private void SwitchOn(Transform gun)
    {
        SwitchOffGuns();
        gun.gameObject.SetActive(true);
        _currentGun = gun;
        
        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {

        foreach (var gun in gunsTransforms)
        {
            gun.gameObject.SetActive(false);   
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = _currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
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
            SwitchOn(pistol);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }  
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn(rivolver);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn(autoRifle);
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn(shotgun);
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);

        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn(rifle);
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
    }
}

public enum GrabType {SideGrab, BackGrab}

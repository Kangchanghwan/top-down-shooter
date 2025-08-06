using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals _visuals;
    private PlayerWeaponController _weaponController;
    void Start()
    {
        _visuals = GetComponentInParent<PlayerWeaponVisuals>();
        _weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadIsOver()
    {
        _visuals.ReturnRigWeightToOne();
        _weaponController.CurrentWeapon().RefillBullets();
        
        _weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        _visuals.ReturnRigWeightToOne();
        _visuals.ReturnLeftHandIKWeightToOne();
    }
    public void WeaponEquipIsOver()
    {
        _weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => _visuals.SwitchOnCurrentWeaponModel();
}

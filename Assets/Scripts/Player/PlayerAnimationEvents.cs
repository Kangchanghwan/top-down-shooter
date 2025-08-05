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
    }

    public void ReturnRig()
    {
        _visuals.ReturnRigWeightToOne();
        _visuals.ReturnLeftHandIKWeightToOne();
    }
    public void WeaponGrabIsOver()
    {
        _visuals.SetBusyGrabbingWeapon(false);
    }
}

using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals _visuals;
    void Start()
    {
        _visuals = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void ReloadIsOver()
    {
        _visuals.ReturnRigWeightToOne();
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

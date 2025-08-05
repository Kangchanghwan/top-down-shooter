using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f;

    private Player _player;
    
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private Weapon SecondWeapon;
    
    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefeb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField]
    private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;
    
    void Start()
    {
        _player = GetComponent<Player>();
        AssignInputEvents();
        
        currentWeapon.bulletsInMagazine = currentWeapon.totalReserveAmmo;
    }

    #region Slots management
    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            print("No slots avalible");
            return;
        }

        weaponSlots.Add(newWeapon);
    }
    private void DropWeapon()
    {
        if (weaponSlots.Count <= 1)
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);
        currentWeapon = weaponSlots[0];
    }
    private void EquipWeapon(int index)
    {
        currentWeapon = weaponSlots[index];
    }
    #endregion
    
    private void Shoot()
    {
        if (currentWeapon.CanShoot() == false)
        {
            return;
        }
        
        GameObject newBullet = Instantiate(bulletPrefeb, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));
        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.linearVelocity = BulletDirection() * bulletSpeed;
        
        Destroy(newBullet, 10f);
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
    public Vector3 BulletDirection()
    {
        Transform aim = _player.aim.Aim();
        
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (_player.aim.CanAimPrecisly() == false &&
            _player.aim.Target() == null)
        {
            direction.y = 0;
        }
        
        // weaponHolder.LookAt(aim);
        // gunPoint.LookAt(aim); TODO : find a better place for it.
        return direction;
    }
    public Transform GunPoint() => gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;
    #region  inputEvent

    private void AssignInputEvents()
    {
        PlayerControllers controllers = _player.controllers;
        controllers.Character.Fire.performed += _ => Shoot();

        controllers.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controllers.Character.EquipSlot2.performed += _ => EquipWeapon(1);

        controllers.Character.DropCurrentWeapon.performed += _ => DropWeapon();
        controllers.Character.Reload.performed += _ =>
        {
            if (currentWeapon.CanReload())
            {
                _player.weaponVisuals.PlayReloadAnimation();
            }
        };
    }

    #endregion
   
}

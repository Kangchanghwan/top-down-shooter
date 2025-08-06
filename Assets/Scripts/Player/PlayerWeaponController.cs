using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f;

    private Player _player;
    
    private bool _weaponReady;
    private bool _isShooting;
    
    [SerializeField] private Weapon currentWeapon;
    
    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefeb;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField]
    private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;
    
    void Start()
    {
        _player = GetComponent<Player>();
        AssignInputEvents();
        Invoke("EquipStartWeapon", .1f);
    }

    void Update()
    {
        if (_isShooting)
        {
            Shoot();
        }
    }
    
    private void EquipStartWeapon() => EquipWeapon(0);

    #region Slots management Pickup/Drop/Equip/Ready
    public void PickupWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            print("No slots avalible");
            return;
        }

        weaponSlots.Add(newWeapon);
        _player.weaponVisuals.SwitchOnBackUpWeaponModel();
    }
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
        {
            return;
        }

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }
    private void EquipWeapon(int index)
    {
        SetWeaponReady(false);
        currentWeapon = weaponSlots[index];

        _player.weaponVisuals.PlayWeaponEquipAnimation();
    }
    
    #endregion
    
    private void Shoot()
    {
        if (currentWeapon.CanShoot() == false || WeaponReady() == false) return;
        if (currentWeapon.shootType == ShootType.Single) _isShooting = false;

        GameObject newBullet = ObjectPool.instance.GetBullet();
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);
        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.linearVelocity = BulletDirection() * bulletSpeed;
        
        ObjectPool.instance.ReturnBullet(newBullet);
        _player.weaponVisuals.PlayFireAnimation();
    }
    public Vector3 BulletDirection()
    {
        Transform aim = _player.aim.Aim();
        
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (_player.aim.CanAimPrecisly() == false &&
            _player.aim.Target() == null)
        {
            direction.y = 0;
        }
        
        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;

    public Transform GunPoint() => _player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;
    public void SetWeaponReady(bool ready) => _weaponReady = ready;
    public bool WeaponReady() => _weaponReady;
    public Weapon BackUpWeapon()
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon != currentWeapon)
            {
                return weapon;
            }
        }

        return null;
    }
    #region  inputEvent

    private void AssignInputEvents()
    {
        PlayerControllers controllers = _player.controllers;
        controllers.Character.Fire.performed += _ => _isShooting = true;
        controllers.Character.Fire.canceled += _ => _isShooting = false;

        controllers.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controllers.Character.EquipSlot2.performed += _ => EquipWeapon(1);

        controllers.Character.DropCurrentWeapon.performed += _ => DropWeapon();
        controllers.Character.Reload.performed += _ =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };
    }

    private void Reload()
    {
        SetWeaponReady(false);
        _player.weaponVisuals.PlayReloadAnimation();
    }

    #endregion
   
}

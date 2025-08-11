using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private const float REFERENCE_BULLET_SPEED = 20f;

    private Player _player;
    
    private bool _weaponReady;
    private bool _isShooting;
    [SerializeField] private WeaponData defaultWeapon;
    [SerializeField] private Weapon currentWeapon;

    [Header("Bullet details")] [SerializeField]
    private float bulletImpactForce = 100;
    [SerializeField] private GameObject bulletPrefeb;
    [SerializeField] private float bulletSpeed;
    
    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")] [SerializeField]
    private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    [SerializeField] private GameObject weaponPickUpPrefab;
    
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            currentWeapon.ToggleBurst();
        }
    }

    private void EquipStartWeapon()
    {
        PickupWeapon(new Weapon(defaultWeapon));
        EquipWeapon(0);
    }

    #region Slots management Pickup/Drop/Equip/Ready
    public void PickupWeapon(Weapon newWeapon)
    {

        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }
        
        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            
            _player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            
            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);
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

        CreateWeaponOnTheGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickUpPrefab);
        droppedWeapon.GetComponent<PickUpWeapon>().SetUpPickUpWeapon(currentWeapon, transform);
    }

    private void EquipWeapon(int index)
    {
        if (index >= weaponSlots.Count) return;
        
        SetWeaponReady(false);
        currentWeapon = weaponSlots[index];

        _player.weaponVisuals.PlayWeaponEquipAnimation();
        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);
    }
    
    #endregion
    
    private void Shoot()
    {
        if (currentWeapon.CanShoot() == false || WeaponReady() == false) return;
      
        _player.weaponVisuals.PlayFireAnimation();
        if (currentWeapon.shootType == ShootType.Single) _isShooting = false;
        if (currentWeapon.burstActive == true)
        {
            StartCoroutine(BurstFire());
            return;
        }
        FireSingleBullet();
    }

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);
            if (i >= currentWeapon.bulletsPerShot)
            {
                SetWeaponReady(true);
            }
        }
    }
    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;
        
        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefeb);
        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);
        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance, bulletImpactForce);
        
        Vector3 bulletDir = currentWeapon.ApplySpread(BulletDirection());
        
        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.linearVelocity = bulletDir * bulletSpeed;
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

    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
            {
                return weapon;
            }
        }
        return null;
    }

    public Transform GunPoint() => _player.weaponVisuals.CurrentWeaponModel().gunPoint;
    public Weapon CurrentWeapon() => currentWeapon;
    public void SetWeaponReady(bool ready) => _weaponReady = ready;
    public bool WeaponReady() => _weaponReady;

    
    #region  inputEvent

    private void AssignInputEvents()
    {
        PlayerControllers controllers = _player.controllers;
        controllers.Character.Fire.performed += _ => _isShooting = true;
        controllers.Character.Fire.canceled += _ => _isShooting = false;

        controllers.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controllers.Character.EquipSlot2.performed += _ => EquipWeapon(1);
        controllers.Character.EquipSlot3.performed += _ => EquipWeapon(2);
        controllers.Character.EquipSlot4.performed += _ => EquipWeapon(3);
        controllers.Character.EquipSlot5.performed += _ => EquipWeapon(4);

        controllers.Character.DropCurrentWeapon.performed += _ => DropWeapon();
        controllers.Character.Reload.performed += _ =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        controllers.Character.ToggleWeaponMode.performed += _ => currentWeapon.ToggleBurst();
    }

    private void Reload()
    {
        SetWeaponReady(false);
        _player.weaponVisuals.PlayReloadAnimation();
    }

    #endregion
   
}

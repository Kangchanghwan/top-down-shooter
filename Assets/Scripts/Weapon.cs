using UnityEngine;
using UnityEngine.Serialization;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}

[System.Serializable]
public class Weapon
{
    public WeaponType weaponType;


    #region Regular mode variables
    public ShootType shootType;
    public int bulletsPerShot { get; private set; }

    private float _defaultFireRate;
    public float fireRate = 1; // bullets per second
    private float _lastShootTime;
    #endregion
    #region Burst mode  variables
    private bool _burstAvalible;
    public bool burstActive;

    private int _burstBulletsPerShot;
    private float _burstFireRate;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    #region Weapon generic info variables
    
    public float reloadSpeed { get; private set; } // how fast charcater reloads weapon    
    public float equipmentSpeed { get; private set; } // how fast character equips weapon
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion
    #region Weapon spread variables
    [Header("Spread ")] 
    private float baseSpread = 1;
    private float maximumSpread = 3;
    private float currentSpread = 2;

    private float spreadIncreaseRate = .15f;

    private float _lastSpreadUpdateTime;
    private float _spreadCooldown = 1;

    #endregion

    public Weapon(WeaponData weaponData)
    {

        bulletsInMagazine = weaponData.bulletsInMagazine;
        magazineCapacity = weaponData.magazineCapacity;
        totalReserveAmmo = weaponData.totalReserveAmmo;

        fireRate = weaponData.fireRate;
        weaponType = weaponData.weaponType;

        bulletsPerShot = weaponData.bulletsPerShot;
        shootType = weaponData.shootType;


        _burstAvalible = weaponData.burstAvalible;
        burstActive = weaponData.burstActive;
        _burstBulletsPerShot = weaponData.burstBulletsPerShot;
        _burstFireRate = weaponData.burstFireRate;
        burstFireDelay = weaponData.burstFireDelay;


        baseSpread = weaponData.baseSpread;
        maximumSpread = weaponData.maxSpread;
        spreadIncreaseRate = weaponData.spreadIncreaseRate;


        reloadSpeed = weaponData.reloadSpeed;
        equipmentSpeed = weaponData.equipmentSpeed;
        gunDistance = weaponData.gunDistance;
        cameraDistance = weaponData.cameraDistance;

        

        _defaultFireRate = fireRate;
    }
    
    #region Spread Methods
    public Vector3 ApplySpread(Vector3 orinDir)
    {
        UpdateSpread();
        
        float randomizedValue = Random.Range(-currentSpread, currentSpread);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRotation * orinDir;
    }
    
    private void UpdateSpread()
    {
        if (Time.time > _lastSpreadUpdateTime + _spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        _lastSpreadUpdateTime = Time.time;
    }
    
    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }


    #endregion

    #region Burst Method

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (_burstAvalible == false) return;
        burstActive = !burstActive;

        if (burstActive)
        {
            bulletsPerShot = _burstBulletsPerShot;
            fireRate = _burstFireRate;
        }
        else
        {
            bulletsPerShot = 1;
            fireRate = _defaultFireRate;
        }
    }
    
    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > _lastShootTime + 1 / fireRate)
        {
            _lastShootTime = Time.time;
            return true;
        }
        return false;
    }

    public bool CanReload()
    {
        if (bulletsInMagazine != magazineCapacity || totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }

    public bool HaveEnoughBullets() => bulletsInMagazine > 0;
   
    public void RefillBullets()
    {
        //totalReserveAmmo += bulletsInMagazine;
        int bulletsToReload = magazineCapacity;
        if (bulletsToReload > totalReserveAmmo)
        {
            bulletsToReload = totalReserveAmmo;
        }

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;
        if (totalReserveAmmo < 0)
        {
            totalReserveAmmo = 0;
        }
    }
}

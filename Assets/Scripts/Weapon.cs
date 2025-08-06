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
    public int bulletPerShot;
    
    [Header("Shooting spesifics")] 
    public ShootType shootType;
    public float defaultFireRate;
    public float fireRate = 1;
    private float _lastShootTime;

    [Header("Burst fire")] public bool burstModeAvalible;
    public bool burstActive;
    public int burstBulletsPerShot;
    public int burstFireRate;
    public float burstFireDelay = .1f;
    
    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Range(1, 3)] public float reloadSpeed = 1;
    [Range(1, 3)] public float equipSpeed = 1;
    [Range(2, 12)] public float gunDistance = 4;
    [Range(3, 8)] public float cameraDistance = 6;
    
    [Header("Spread ")] 
    public float baseSpread;
    public float currentSpread = 2;
    public float maximumSpread = 3;
    public float spreadIncreaseRate = .15f;
    
    private float _lastSpreadUpdateTime;
    private float _spreadCooldown = 1;
    
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
        if (burstModeAvalible == false) return;
        burstActive = !burstActive;

        if (burstActive)
        {
            bulletPerShot = burstBulletsPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletPerShot = 1;
            fireRate = defaultFireRate;
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

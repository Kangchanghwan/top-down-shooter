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

[System.Serializable]
public class Weapon
{
    public WeaponType WeaponType;
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }
    
    public bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
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

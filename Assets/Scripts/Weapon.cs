using UnityEngine;

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
    public int ammo;
    public int maxAmmo;

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }
    
    public bool HaveEnoughBullets()
    {
        if (ammo > 0)
        {
            ammo--;
            return true;
        }
        return false;
    }
}

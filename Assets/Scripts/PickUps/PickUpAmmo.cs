using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType
{
    SmallBox, BigBox
}

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10, 300)] public int minAmount;
    [Range(10, 300)] public int maxAmount;
}

public class PickUpAmmo: Interactable
{
    [SerializeField] private AmmoBoxType boxType;
    
    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetUpBoxModel();
    }

    private void SetUpBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            if (i == (int)boxType)
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }


    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (boxType == AmmoBoxType.BigBox)
        {
            currentAmmoList = bigBoxAmmo;
        }
        
        foreach (var ammoData in currentAmmoList)
        {
            Weapon weapon = _weaponController.WeaponInSlots(ammoData.weaponType);
            AddBulletToWeapon(weapon, GetBulletAmount(ammoData));
        }
        ObjectPool.instance.ReturnObject(gameObject);
    }



    private int GetBulletAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.maxAmount, ammoData.minAmount);
        float max = Mathf.Max(ammoData.maxAmount, ammoData.maxAmount);
        
        float randomAmmoAmount = Random.Range(min, max);
        return Mathf.RoundToInt(randomAmmoAmount);
    }
    
    private void AddBulletToWeapon(Weapon weapon, int amount)
    {
        if(weapon != null) weapon.totalReserveAmmo += amount;
    }
    
}

using System;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{

    [SerializeField] private WeaponData weaponData;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerWeaponController>()?.PickupWeapon(weaponData);
    }
}

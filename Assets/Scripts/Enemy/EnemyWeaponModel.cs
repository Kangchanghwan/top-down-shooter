using System;
using UnityEngine;

public class EnemyWeaponModel : MonoBehaviour
{
    public EnemyWeaponModelType weaponType;
    public AnimatorOverrideController overrideController;
    public EnemyMeleeWeaponData weaponData;
    
    [SerializeField] private GameObject[] trailEffects;

    private void Awake()
    {
        EnableTrailEffect(false);
    }

    public void EnableTrailEffect(bool enable)
    {
        foreach (var trailEffect in trailEffects)
        {
            trailEffect.SetActive(enable);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Melee Weapon Data")]
public class EnemyMeleeWeaponData : ScriptableObject
{
    public List<AttackDataEnemyMelee> attackData;
    public float turnSpeed = 10;
}

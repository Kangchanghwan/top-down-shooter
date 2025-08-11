using System;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] private int durability;


 
    public void ReduceDurability()
    {
        durability--;

        if (durability <= 0)
        {
            Destroy(gameObject);
        }
    }
}

using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private GameObject bulletImpactFx;
    
    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFx(other);
        ObjectPool.instance.ReturnBullet(gameObject);
    }

    private void CreateImpactFx(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint contact = other.contacts[0];

            GameObject newImpactFx =
                Instantiate(bulletImpactFx, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(newImpactFx, 1f);
        }
    }
}

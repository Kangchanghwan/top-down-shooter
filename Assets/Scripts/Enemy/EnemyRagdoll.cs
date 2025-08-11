using System;
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;
    [SerializeField] private Collider[] ragdollColliders;
    [SerializeField] private Rigidbody[] ragdollRigidBodies;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        
        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        foreach (var rb in ragdollRigidBodies)
        {
            rb.isKinematic = !active;
        }
    }   
    public void CollidersActive(bool active)
    {
        foreach (var collider in ragdollColliders)
        {
            collider.enabled = active;
        }
    }
}

using System;
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;
    private Collider[] _ragdollColliders;
    private Rigidbody[] _ragdollRigidBodies;

    private void Awake()
    {
        _ragdollColliders = GetComponentsInChildren<Collider>();
        _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        
        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        foreach (var rb in _ragdollRigidBodies)
        {
            rb.isKinematic = !active;
        }
    }   
    public void CollidersActive(bool active)
    {
        foreach (var collider in _ragdollColliders)
        {
            collider.enabled = active;
        }
    }
}

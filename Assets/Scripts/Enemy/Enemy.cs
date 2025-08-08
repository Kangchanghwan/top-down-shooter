using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [Header("Idle data")] public float idleTime;
    [Header("Move data")] 
    public float moveSpeed;
    
    [SerializeField] private Transform[] patrolPoints;
    private int _currentPatrolIndex;

    public NavMeshAgent agent;
    
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    private void InitializePatrolPoints()
    {
        foreach (var patrolPoint in patrolPoints)
        {
            patrolPoint.parent = null;
        }
    }

    protected virtual void Update()
    {
        
    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[_currentPatrolIndex].transform.position;
        ++_currentPatrolIndex;

        if (_currentPatrolIndex >= patrolPoints.Length)
        {
            _currentPatrolIndex = 0;
        }

        return destination;
    }
}

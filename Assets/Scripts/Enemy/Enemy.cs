using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoint = 25;
    
    [Header("Idle data")] 
    public float idleTime;
    public float aggresionRange;

    [Header("Move data")] 
    public float moveSpeed;
    public float turnSpeed;
    public float chaseSpeed;
    private bool _manualMovement;
    private bool _manualRotation;
    
    [SerializeField] private Transform[] patrolPoints;
    private int _currentPatrolIndex;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
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

    public virtual void GetHit()
    {
        healthPoint--;
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCourutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCourutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
       yield return new WaitForSeconds(.1f);
       
       rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }

    public void ActivateManualMovement(bool manualMovement) => _manualMovement = manualMovement;
    public bool ManualMovementActive() => _manualMovement;
    public void ActivateManualRotation(bool manualRotation) => _manualRotation = manualRotation;
    public bool ManualRotationActive() => _manualRotation;
    
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
    
    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;
    
    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }
}

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
    private Vector3[] patrolPointsPosition;
    private int _currentPatrolIndex;

    public bool inBattleMode { get; private set; }

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

    protected virtual void Update()
    {
        
    }

    protected bool ShouldEnterBattleMode()
    {
       bool inAgresionRange =  Vector3.Distance(transform.position, player.position) < aggresionRange;
       if (inAgresionRange && !inBattleMode)
       {
           return true;
       }
       return false;
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoint--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCourutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCourutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
       yield return new WaitForSeconds(.1f);
       
       rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    #region Animation events

    public void ActivateManualMovement(bool manualMovement) => _manualMovement = manualMovement;

    public bool ManualMovementActive() => _manualMovement;

    public void ActivateManualRotation(bool manualRotation) => _manualRotation = manualRotation;

    public bool ManualRotationActive() => _manualRotation;

    public virtual void AbilityTrigger() => stateMachine.currentState.AbilityTrigger();

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    #endregion

    #region Patrol logic

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[_currentPatrolIndex];
        ++_currentPatrolIndex;

        if (_currentPatrolIndex >= patrolPoints.Length)
        {
            _currentPatrolIndex = 0;
        }

        return destination;
    }

    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    #endregion
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }

}

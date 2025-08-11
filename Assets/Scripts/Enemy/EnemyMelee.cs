using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex;
    [Range(1, 3)]
    public float animationSpeed;
    public AttackTypeMelee attackType;
}

public enum AttackTypeMelee
{
    Close, Charge
}

public enum EnemyMeleeType
{
    Regular, Shield, Dodge
}
public class EnemyMelee: Enemy
{
        
    public IdleStateMelee idleState { get; private set; }
    public MoveStateMelee moveState { get; private set; }
    public RecoveryStateMelee recoveryState { get; private set; }
    public ChaseStateMelee chaseState { get; private set; }
    public AttackStateMelee attackState { get; private set; }
    public DeadStateMelee deadState { get; private set; }

    [Header("Enemy Settings")]
    public EnemyMeleeType meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float _lastTimeDodge;
    
    [Header("Attack Data")] 
    public AttackData attackData;
    public List<AttackData> attackDatas;
    
    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;
    
    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleStateMelee(this, stateMachine, "Idle");
        moveState = new MoveStateMelee(this, stateMachine, "Move");
        recoveryState = new RecoveryStateMelee(this, stateMachine, "Recovery");
        chaseState = new ChaseStateMelee(this, stateMachine, "Chase");
        attackState = new AttackStateMelee(this, stateMachine, "Attack");
        deadState = new DeadStateMelee(this, stateMachine, "Idle");
    }

    protected override void Start()
    {
        base.Start();
        
        stateMachine.Initialize(idleState);
        InitializeSpeciality();
    }

    protected override void Update()
    {
        base.Update();
        
        stateMachine.currentState.Update();
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }

    private void InitializeSpeciality()
    {
        if (meleeType == EnemyMeleeType.Shield)
        {
            anim.SetFloat("ChaseIndex", 1 );
            shieldTransform.gameObject.SetActive(true);
        }
    }
    
    public override void GetHit()
    {
        base.GetHit();
        if (healthPoint <= 0)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    public void ActiveDodgeRoll()
    {
        if (meleeType != EnemyMeleeType.Dodge) return;
        
        if (stateMachine.currentState != chaseState) return;

        if (Vector3.Distance(transform.position, player.position) < 2f) return;
        
        if (Time.time > _lastTimeDodge + dodgeCooldown)
        {
            _lastTimeDodge = Time.time;
            anim.SetTrigger(("DodgeRoll"));   
        }
    }
    
    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }
}

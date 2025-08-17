using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct AttackDataEnemyMelee
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
    Regular, Shield, Dodge, AxeThrow
}
public class EnemyMelee: Enemy
{
    #region States

    public IdleStateMelee idleState { get; private set; }
    public MoveStateMelee moveState { get; private set; }
    public RecoveryStateMelee recoveryState { get; private set; }
    public ChaseStateMelee chaseState { get; private set; }
    public AttackStateMelee attackState { get; private set; }
    public DeadStateMelee deadState { get; private set; }
    public AbilityStateMelee abilityState { get; private set; }

    #endregion

    public EnemyVisuals visuals { get; private set; }

    [Header("Enemy Settings")]
    public EnemyMeleeType meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float _lastTimeDodge = -10f;
    public Transform axeStartPoint;

    [Header("Axe Throw Ability")] 
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    private float _lastTimeAxeThrown;
    public float axtThrowCooldown;
    
    [Header("Attack Data")] 
    public AttackDataEnemyMelee attackData;
    public List<AttackDataEnemyMelee> attackDatas;
    
    [SerializeField] private Transform hiddenWeapon;
    
    protected override void Awake()
    {
        base.Awake();

        visuals = GetComponent<EnemyVisuals>();

        idleState = new IdleStateMelee(this, stateMachine, "Idle");
        moveState = new MoveStateMelee(this, stateMachine, "Move");
        recoveryState = new RecoveryStateMelee(this, stateMachine, "Recovery");
        chaseState = new ChaseStateMelee(this, stateMachine, "Chase");
        attackState = new AttackStateMelee(this, stateMachine, "Attack");
        deadState = new DeadStateMelee(this, stateMachine, "Idle");
        abilityState = new AbilityStateMelee(this, stateMachine, "AxeThrow");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        ResetCooldown();
        
        InitializePerk();
        visuals.SetupLook();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        
        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode) return;
        
        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
    
    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        moveSpeed = moveSpeed * .6f;
        EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        EnemyWeaponModel currentWeapon = visuals.currentWeaponModel.GetComponent<EnemyWeaponModel>();

        if (currentWeapon.weaponData != null)
        {
            attackDatas = new List<AttackDataEnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }
    
    private void InitializePerk()
    {
        if (meleeType == EnemyMeleeType.AxeThrow)
        {
            visuals.SetUpWeaponType(EnemyWeaponModelType.Throw);
        }
        
        if (meleeType == EnemyMeleeType.Shield)
        {
            anim.SetFloat("ChaseIndex", 1 );
            shieldTransform.gameObject.SetActive(true);
            visuals.SetUpWeaponType(EnemyWeaponModelType.OneHand);
        }

        if (meleeType == EnemyMeleeType.Dodge)
        {
            visuals.SetUpWeaponType(EnemyWeaponModelType.Unarmed);
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


    public void ActiveDodgeRoll()
    {
        if (meleeType != EnemyMeleeType.Dodge) return;
        
        if (stateMachine.currentState != chaseState) return;

        if (Vector3.Distance(transform.position, player.position) < 2f) return;

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge Roll (Not Read-Only)");
        
        if (Time.time > _lastTimeDodge + dodgeAnimationDuration + dodgeCooldown)
        {
            _lastTimeDodge = Time.time;
            anim.SetTrigger(("DodgeRoll"));   
        }
    }

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMeleeType.AxeThrow) return false;
        
        if (Time.time > _lastTimeAxeThrown + axtThrowCooldown)
        {
            _lastTimeAxeThrown = Time.time;
            return true;
        }

        return false;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }
        print(clipName + "Not Found");
        return 0f;
    }
    private void ResetCooldown()
    {
        _lastTimeDodge -= dodgeCooldown;
        _lastTimeAxeThrown -= axtThrowCooldown;
    }


    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    public void EnableWeaponModel(bool active)
    {
        visuals.currentWeaponModel.gameObject.SetActive(active);
    }
}

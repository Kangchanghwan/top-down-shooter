using UnityEngine;

public class AttackStateMelee : EnemyState
{
    private EnemyMelee _enemy;
    private Vector3 _attackDirection;

    private float _attackMoveSpeed;
    

    private const float MAX_ATTACK_DISTANCE = 50f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AttackStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();

        _attackMoveSpeed = _enemy.attackData.moveSpeed;
        _enemy.anim.SetFloat("AttackAnimationSpeed", _enemy.attackData.animationSpeed);
        _enemy.anim.SetFloat("AttackIndex", _enemy.attackData.attackIndex);
        
        _enemy.PullWeapon();
        
        _enemy.agent.isStopped = true;
        _enemy.agent.velocity = Vector3.zero;
        
        _attackDirection = _enemy.transform.position + (_enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (_enemy.ManualRotationActive())
        {
            _enemy.transform.rotation = _enemy.FaceTarget(enemyBase.player.position);
            _attackDirection = _enemy.transform.position + (_enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }

        if (_enemy.ManualMovementActive())
        {
            _enemy.transform.position = Vector3.MoveTowards(
                _enemy.transform.position,
                _attackDirection, 
                _attackMoveSpeed * Time.deltaTime);
        }
        
       
        if (triggerCalled)
        {
            if (_enemy.PlayerInAttackRange())
            {
                stateMachine.ChangeState(_enemy.recoveryState);
            }
            else
            {
                stateMachine.ChangeState(_enemy.chaseState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}

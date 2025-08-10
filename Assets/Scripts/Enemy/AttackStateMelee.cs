using UnityEngine;

public class AttackStateMelee : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 _attackDirection;

    private const float MAX_ATTACK_DISTANCE = 50f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AttackStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        
        enemy.PullWeapon();
        
        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;
        
        _attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.ManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                _attackDirection, 
                enemy.attackMoveSpeed * Time.deltaTime);
        }
        
       
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}

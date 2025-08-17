using UnityEngine;

public class ChaseStateMelee : EnemyState
{
    private float lastTimeUpdatedDestination;
    private EnemyMelee enemy;
    
    public ChaseStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }
    public override void Enter()
    {
        base.Enter();
        enemy.agent.speed = enemy.chaseSpeed;
        enemy.agent.isStopped = false;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        
        enemy.FaceTarget(GetNextPathPoint());
        
        if (CanUpdateDestination())
        {
            enemy.agent.destination = enemy.player.transform.position;
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time > lastTimeUpdatedDestination + .25f)
        {
            lastTimeUpdatedDestination = Time.time;
            return true;
        }
        return false;
    }
}

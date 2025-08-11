using UnityEngine;
using UnityEngine.AI;

public class MoveStateMelee : EnemyState
{
    
    private EnemyMelee enemy;
    private Vector3 destination;
    
    public MoveStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }
    
    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.moveSpeed;
        
        destination = enemy.GetPatrolDestination();
        enemy.agent.SetDestination(destination);
    }

    public override void Update()
    {
        base.Update();

        // if (enemy.PlayerInAggresionRange())
        // {
        //     stateMachine.ChangeState(enemy.recoveryState);
        //     return;
        // }
        //
        // enemy.FaceTarget(GetNextPathPoint());
        //
        // if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
        // {
        //     stateMachine.ChangeState(enemy.idleState);
        // }
    }



    public override void Exit()
    {
        base.Exit();
    }
}

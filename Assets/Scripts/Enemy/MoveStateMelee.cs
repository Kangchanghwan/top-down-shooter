using UnityEngine;

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
        this.destination = enemy.GetPatrolDestination();
    }

    public override void Update()
    {
        base.Update();
        enemy.agent.SetDestination(destination);

        if (enemy.agent.remainingDistance <= 1)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

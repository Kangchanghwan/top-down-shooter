using UnityEngine;

public class RecoveryStateMelee : EnemyState
{
    private EnemyMelee enemy;
    
    public RecoveryStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as EnemyMelee;
    }
    
    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
    }

    public override void Update()
    {
        base.Update();
        enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);
        if (triggerCalled == true)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

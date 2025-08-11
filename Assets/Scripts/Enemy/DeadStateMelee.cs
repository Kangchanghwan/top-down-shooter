using UnityEngine;

public class DeadStateMelee: EnemyState
{
    private EnemyMelee _enemy;
    private EnemyRagdoll _ragdoll;

    private bool _interactionDisabled;
        
    public DeadStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemyMelee;
        _ragdoll = _enemy.GetComponent<EnemyRagdoll>();
    }
    
    public override void Enter()
    {
        base.Enter();
        _interactionDisabled = false;
            
        _enemy.anim.enabled = false;
        _enemy.agent.isStopped = true;

        _ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0 && _interactionDisabled == false)
        {
            _interactionDisabled = true;
            _ragdoll.RagdollActive(false);
            _ragdoll.CollidersActive(false);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

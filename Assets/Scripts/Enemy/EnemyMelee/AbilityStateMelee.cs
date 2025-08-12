using UnityEngine;

public class AbilityStateMelee: EnemyState
{
    private EnemyMelee _enemy;
    private Vector3 _movementDirection;

    private const float MAX_MOVEMENT_DISTANCE = 20f;

    private float _moveSpeed;
    
    public AbilityStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemyMelee;
    }
    
    public override void Enter()
    {
        base.Enter();
        
        _enemy.EnableWeaponModel(true);

        _movementDirection = _enemy.transform.position + (_enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        _moveSpeed = _enemy.moveSpeed;
    }

    public override void Update()
    {
        base.Update();
        
        if (_enemy.ManualRotationActive())
        {
            _enemy.FaceTarget(enemyBase.player.position);
            _movementDirection = _enemy.transform.position + (_enemy.transform.forward * MAX_MOVEMENT_DISTANCE);
        }
        
        if (_enemy.ManualMovementActive())
        {
            _enemy.transform.position = Vector3.MoveTowards(
                _enemy.transform.position,
                _movementDirection, 
                _moveSpeed * Time.deltaTime);
        }
        
        if (triggerCalled)
        {
            stateMachine.ChangeState(_enemy.recoveryState);
        }
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        GameObject newAxe = ObjectPool.instance.GetObject(_enemy.axePrefab);
       
        newAxe.transform.position = _enemy.axeStartPoint.position;
        newAxe.GetComponent<EnemyAxe>().AxeSetUp(_enemy.axeFlySpeed, _enemy.player, _enemy.axeAimTimer);

    }

    public override void Exit()
    {
        base.Exit();
        _enemy.moveSpeed = _moveSpeed;
        _enemy.anim.SetFloat("RecoveryIndex", 0);
    }
}

using UnityEngine;

internal class RushTowardPlayerState : EnemyState
{
    public RushTowardPlayerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    int animHash;

    public override void Enter()
    {
        bool isPolice = stateMachine.enemyType == EnemyStateMachine.EnemyType.Police;
        animHash = isPolice ? stateMachine.EntityInfo.AnimIsRushing : stateMachine.EntityInfo.AnimIsRunning;

        stateMachine.EntityInfo.PhysicAnimator.SetBool(animHash, true);
        stateMachine.EntityInfo.Agent.isStopped = false;
        stateMachine.EntityInfo.Agent.speed = 15f;
    }

    public override void Tick()
    {
        //Corregir el error que los deja en el lugar
        if (stateMachine.EntityInfo.Agent.isStopped)
            stateMachine.EntityInfo.Agent.isStopped = false;
        CheckTransitions();
        Move();
    }
    public override void Exit()
    {
        stateMachine.EntityInfo.PhysicAnimator.SetBool(animHash, false);
        stateMachine.EntityInfo.Agent.speed = 10f;
    }

    private void Move()
    {
        if (SceneInfo.Instance.PlrCharacter == null) return;
        stateMachine.MovementVector = SceneInfo.Instance.PlrCharacter.position;
    }

    private void CheckTransitions()
    {
        bool isClose = stateMachine.GetDistanceFromPlayer() <= stateMachine.info.attackRadius;
        bool isOnSight = Vector3.Angle(stateMachine.EntityInfo.Char.transform.forward, stateMachine.GetDirectionToPlayer()) <= stateMachine.info.attackAngle;

        if (isClose && isOnSight)
        {
            stateMachine.SetState(new AttackState(stateMachine));
        }
    }
}
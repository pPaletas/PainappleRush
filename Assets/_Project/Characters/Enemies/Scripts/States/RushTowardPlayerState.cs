using UnityEngine;

internal class RushTowardPlayerState : EnemyState
{
    public RushTowardPlayerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.EntityInfo.PhysicAnimator.SetBool(stateMachine.EntityInfo.AnimIsRushing, true);
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
        stateMachine.EntityInfo.PhysicAnimator.SetBool(stateMachine.EntityInfo.AnimIsRushing, false);
        stateMachine.EntityInfo.Agent.speed = 10f;
    }

    private void Move()
    {
        stateMachine.MovementVector = SceneInfo.sceneInfo.PlrCharacter.position;
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
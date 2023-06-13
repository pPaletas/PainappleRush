using UnityEngine;

internal class GuardState : EnemyState
{
    private float _randomTime;

    public GuardState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.EntityInfo.Movement.SetAgent(false);
        _randomTime = Random.Range(stateMachine.info.secondsToAttack.x, stateMachine.info.secondsToAttack.y);
        stateMachine.EntityInfo.PhysicAnimator.SetBool(stateMachine.EntityInfo.AnimIsRunning, false);
    }

    public override void Tick()
    {
        _randomTime -= Time.deltaTime;
        CheckTransitions();
        RotateTowardsPlayer();
    }

    public override void Exit()
    {
        stateMachine.EntityInfo.Movement.SetAgent(true);
    }

    private void CheckTransitions()
    {
        if (stateMachine.GetDistanceFromPlayer() > stateMachine.info.stoppingDistance && stateMachine.EntityInfo.PunchComboCooldown.IsStopped)
        {
            stateMachine.SetState(new ApproachPlayerState(stateMachine));
        }
        else if (_randomTime <= 0f)
        {
            stateMachine.SetState(new RushTowardPlayerState(stateMachine));
        }

        bool isClose = stateMachine.GetDistanceFromPlayer() <= stateMachine.info.attackRadius;
        bool isOnSight = Vector3.Angle(stateMachine.EntityInfo.Char.transform.forward, stateMachine.GetDirectionToPlayer()) <= stateMachine.info.attackAngle;
    }

    private void RotateTowardsPlayer()
    {
        if (stateMachine.GetDirectionToPlayer() == Vector3.zero) return;

        Transform pivot = stateMachine.EntityInfo.Char.transform;
        Quaternion targetRotation = Quaternion.LookRotation(stateMachine.GetDirectionToPlayer(), Vector3.up);
        Quaternion currentRotation = pivot.rotation;

        pivot.rotation = Quaternion.Slerp(currentRotation, targetRotation, stateMachine.info.rotationSmoothness * Time.deltaTime);
    }
}
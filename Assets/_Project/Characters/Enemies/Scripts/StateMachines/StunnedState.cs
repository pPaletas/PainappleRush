public class StunnedState : EnemyState
{
    public StunnedState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.EntityInfo.Agent.isStopped = true;
    }

    public override void Tick() { }

    public override void Exit() { }
}
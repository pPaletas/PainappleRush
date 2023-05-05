internal class RushTowardPlayerState : EnemyState
{
    public RushTowardPlayerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() { }

    public override void Tick()
    {
        Move();
    }
    public override void Exit() { }

    private void Move()
    {
        stateMachine.MovementVector = SceneInfo.sceneInfo.PlrCharacter.position;
    }

    private void CheckTransitions()
    {
        
    }
}
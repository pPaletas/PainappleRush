using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : EntityMovement
{
    private NavMeshAgent _agent;

    public override void SetAgent(bool active)
    {
        _agent.isStopped = !active;
    }

    public override void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }

    protected override void Start()
    {
        base.Start();
        _agent = GetComponent<NavMeshAgent>();
        usePivot = false;
    }

    protected override void Update()
    {
        base.Update();

        if (_agent.enabled && canMove)
        {
            _agent.destination = input.MovementVector;
        }
        else if (!canMove)
        {
            _agent.destination = transform.position;
        }
    }

    protected override void DashUpdate()
    {
        if (isDashing)
        {
            _agent.Move(dashDirection * dashCurrentSpeed * Time.deltaTime);

            dashCurrentTime -= Time.deltaTime;
            if (dashCurrentTime <= 0f)
            {
                StopDash();
            }
        }
    }

    protected override void RotateTowardsMovement() { }
    protected override void ProcessInput() { }
}
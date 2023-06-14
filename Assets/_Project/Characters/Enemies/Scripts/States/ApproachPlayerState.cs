using UnityEngine;

public class ApproachPlayerState : EnemyState
{
    private Vector3 _currentAlternativeDestiny;
    private float targetAngle;
    private float currentAngle = 0f;

    public ApproachPlayerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.EntityInfo.PhysicAnimator.SetBool(stateMachine.EntityInfo.AnimIsRunning, true);
        stateMachine.EntityInfo.Agent.isStopped = false;
    }

    public override void Tick()
    {
        FixStuckGlitch();
        CheckTransitions();
        Move();
        CheckIfDash();
    }

    public override void Exit() { }

    private void FixStuckGlitch()
    {
        if (stateMachine.EntityInfo.Agent.isStopped)
            stateMachine.EntityInfo.Agent.isStopped = false;

        if (!stateMachine.EntityInfo.Movement.canMove)
            stateMachine.EntityInfo.Movement.canMove = true;
    }

    private bool IsSomeoneBlocking(float distance)
    {
        Vector3 from = stateMachine.EntityInfo.Char.transform.position;

        Vector3 directionToPlayer = stateMachine.GetDirectionToPlayer();
        // bool isSomeoneBlocking = Physics.SphereCast(from, 0.5f, directionToPlayer, out hit, distance, stateMachine.defaultMask);

        Collider[] others = Physics.OverlapSphere(from, distance, stateMachine.info.avoidanceLayer);
        bool isSomeoneBlocking = false;

        foreach (Collider other in others)
        {
            float angleDif = Mathf.Acos(Vector3.Dot(directionToPlayer, (other.transform.position - from).normalized)) * Mathf.Rad2Deg;

            if (angleDif <= 30f)
            {
                isSomeoneBlocking = true;
                break;
            }
        }

        return isSomeoneBlocking;
    }

    // Genera un conjunto de raycast en circulo para hallar algún espacio por donde meterse
    private Vector3 FindSpace(out float angle)
    {
        float rayLength = 20f;
        float separationAngle = 90f / (stateMachine.info.raycastCheckRes);
        float currentAngle = separationAngle;

        Vector3 myPos = stateMachine.EntityInfo.Char.transform.position;
        float lastDist1 = 0f;
        float lastDist2 = 0f;

        for (int i = 0; i < stateMachine.info.raycastCheckRes; i++)
        {
            Vector3 direction1 = Quaternion.AngleAxis(currentAngle, Vector3.up) * stateMachine.GetDirectionToPlayer();
            Vector3 direction2 = Quaternion.AngleAxis(-currentAngle, Vector3.up) * stateMachine.GetDirectionToPlayer();

            RaycastHit hit;

            // bool ray1 = Physics.Raycast(myPos, direction1, out hit, rayLength, stateMachine.info.hurtboxLayer);
            bool ray1 = Physics.SphereCast(myPos, 4f, direction1, out hit, rayLength, stateMachine.info.avoidanceLayer);
            lastDist1 = hit.distance;
            // bool ray2 = Physics.Raycast(myPos, direction2, out hit, rayLength, stateMachine.info.hurtboxLayer);
            bool ray2 = Physics.SphereCast(myPos, 4f, direction2, out hit, rayLength, stateMachine.info.avoidanceLayer);
            lastDist2 = hit.distance;

            if (!ray1 && !ray2)
            {
                if (lastDist1 < lastDist2)
                {
                    angle = currentAngle;
                    return direction1;
                }
                else if (lastDist2 < lastDist1)
                {
                    angle = -currentAngle;
                    return direction2;
                }
            }
            else if (!ray1 || !ray2)
            {
                if (!ray1)
                {
                    angle = currentAngle;
                    return direction1;
                }
                if (!ray2)
                {
                    angle = -currentAngle;
                    return direction2;
                }
            }
            currentAngle += separationAngle;
        }

        angle = 60f;
        return Vector3.zero;
    }

    private void Move()
    {
        if (SceneInfo.Instance.PlrCharacter == null) return;

        stateMachine.MovementVector = SceneInfo.Instance.PlrCharacter.position;

        if (IsSomeoneBlocking(15f))
        {
            float angle;
            Vector3 dir = FindSpace(out angle);
            targetAngle = angle;
        }
        else
        {
            targetAngle = 0f;
        }

        Vector3 vector = Quaternion.AngleAxis(currentAngle, Vector3.up) * stateMachine.GetDirectionToPlayer() * stateMachine.GetDistanceFromPlayer();
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, stateMachine.info.targetChangeSpeed * Time.deltaTime);
        stateMachine.MovementVector = stateMachine.EntityInfo.Char.transform.position + vector;
    }

    private void CheckIfDash()
    {
        if (stateMachine.enemyType == EnemyStateMachine.EnemyType.Turtoise) return;

        bool isClose = stateMachine.GetDistanceFromPlayer() <= stateMachine.info.attackRadius;
        bool isOnSight = Vector3.Angle(stateMachine.EntityInfo.Char.transform.forward, stateMachine.GetDirectionToPlayer()) <= stateMachine.info.attackAngle;
        stateMachine.IsDashing = false;

        if (stateMachine.GetDistanceFromPlayer() >= stateMachine.info.distanceToDash && !IsSomeoneBlocking(10f))
        {
            stateMachine.IsDashing = true;
        }

        if (isClose && isOnSight && stateMachine.EntityInfo.PunchComboCooldown.IsStopped)
        {
            stateMachine.SetState(new AttackState(stateMachine));
        }
    }

    private void RotateTowardsPlayer()
    {
        Transform pivot = stateMachine.EntityInfo.Char.transform;
        Quaternion targetRotation = Quaternion.LookRotation(stateMachine.GetDirectionToPlayer(), Vector3.up);
        Quaternion currentRotation = pivot.rotation;

        pivot.rotation = Quaternion.Slerp(currentRotation, targetRotation, stateMachine.info.rotationSmoothness * Time.deltaTime);
    }

    private void CheckIfCanAttack()
    {
        bool isClose = stateMachine.GetDistanceFromPlayer() <= stateMachine.info.attackRadius;
        bool isOnSight = Vector3.Angle(stateMachine.EntityInfo.Char.transform.forward, stateMachine.GetDirectionToPlayer()) <= stateMachine.info.attackAngle;

        if (isClose && !isOnSight)
        {
            RotateTowardsPlayer();
        }
        else if (isClose && isOnSight)
        {
            stateMachine.SetState(new AttackState(stateMachine));
        }
    }

    private void CheckTransitions()
    {
        // if (stateMachine.enemyType == EnemyStateMachine.EnemyType.Turtoise)
        // {
        //     // CheckIfCanAttack();
        //     // stateMachine.SetState(new GuardState(stateMachine));
        // }
        if (stateMachine.GetDistanceFromPlayer() <= stateMachine.info.stoppingDistance && !stateMachine.EntityInfo.Movement.Dashing)
        {
            stateMachine.SetState(new GuardState(stateMachine));
        }
    }
}
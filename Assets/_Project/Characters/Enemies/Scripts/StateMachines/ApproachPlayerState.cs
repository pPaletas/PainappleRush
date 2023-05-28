using UnityEngine;

public class ApproachPlayerState : EnemyState
{
    private Vector3 _currentAlternativeDestiny;
    private float targetAngle;
    private float currentAngle = 0f;

    public ApproachPlayerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() { }

    public override void Tick()
    {
        Move();
        CheckIfDash();
        CheckTransitions();
    }

    public override void Exit() { }

    private bool IsSomeoneBlocking(float distance)
    {
        Vector3 from = stateMachine.EntityInfo.Char.transform.position;
        RaycastHit hit;

        bool isSomeoneBlocking = Physics.SphereCast(from, 4f, stateMachine.GetDirectionToPlayer(), out hit, distance, stateMachine.info.avoidanceLayer);

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
        stateMachine.MovementVector = SceneInfo.sceneInfo.PlrCharacter.position;

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
        stateMachine.IsDashing = false;

        if (stateMachine.GetDistanceFromPlayer() >= stateMachine.info.distanceToDash && !IsSomeoneBlocking(10f))
        {
            stateMachine.IsDashing = true;
        }
    }

    private void CheckTransitions()
    {
        if (stateMachine.GetDistanceFromPlayer() <= stateMachine.info.stoppingDistance && !stateMachine.EntityInfo.Movement.Dashing)
        {
            stateMachine.SetState(new GuardState(stateMachine));
        }
    }
}
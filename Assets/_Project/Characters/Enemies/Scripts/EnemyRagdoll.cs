using UnityEngine;

public class EnemyRagdoll : RagdollSystem
{
    protected override void SetUpRagdoll()
    {
        base.SetUpRagdoll();
        entityInfo.Agent.isStopped = true;
        spine.transform.SetParent(null);
        entityInfo.CharacterCollider.enabled = false;
    }

    protected override bool WakeUp()
    {
        if (entityInfo.Hurtbox.enabled)
        {
            entityInfo.Hurtbox.enabled = false;

            if (isKO)
            {
                SetJointsActive(false);
            }
        }

        wakeUpCurrentStunTime -= Time.deltaTime;

        return wakeUpCurrentStunTime <= 0f;
    }

    protected override void StopRagdoll()
    {
        entityInfo.Char.transform.position = GetParentPosAfterRagdoll();

        Vector3 fwd = spine.transform.forward;
        fwd.y = 0f;
        fwd.Normalize();
        entityInfo.Char.transform.forward = fwd;

        spine.transform.SetParent(_root);
        entityInfo.Agent.isStopped = false;
        entityInfo.CharacterCollider.enabled = true;
        entityInfo.Hurtbox.enabled = true;

        entityInfo.PhysicAnimator.enabled = true;
    }
}
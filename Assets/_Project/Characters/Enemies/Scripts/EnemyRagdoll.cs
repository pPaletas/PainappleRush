using UnityEngine;

public class EnemyRagdoll : RagdollSystem
{
    protected override void SetUpRagdoll()
    {
        base.SetUpRagdoll();
        spine.transform.SetParent(null);
        entityInfo.CharacterCollider.enabled = false;
        entityInfo.StateMachine.SetState(new StunnedState(entityInfo.StateMachine));
    }

    protected override bool WakeUp()
    {
        if (entityInfo.Hurtbox.enabled)
        {
            if (isKO)
            {
                SetJointsActive(false);
            }
        }
        entityInfo.Hurtbox.enabled = true;
        entityInfo.CharacterCollider.enabled = true;

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

        entityInfo.StateMachine.SetState(new ApproachPlayerState(entityInfo.StateMachine));

        base.StopRagdoll();
    }
}
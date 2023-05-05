using UnityEngine;

public class PlayerRagdoll : RagdollSystem
{
    protected override void SetUpRagdoll()
    {
        base.SetUpRagdoll();
        entityInfo.Input.canRead = false;
        entityInfo.CharacterCollider.enabled = false;
        spine.transform.SetParent(null);
        entityInfo.CharacterController.enabled = false;
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
        entityInfo.Input.canRead = true;
        entityInfo.CharacterCollider.enabled = true;
        entityInfo.Hurtbox.enabled = true;
        entityInfo.CharacterController.enabled = true;

        base.StopRagdoll();
    }
}
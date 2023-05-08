using UnityEngine;

public class PlayerRagdoll : RagdollSystem, IRemoteCallable
{
    public string Name => "PlayerRagdoll";

    public void RemoteInvoke(object[] parameters)
    {
        bool enable = (bool)parameters[0];

        if (enable)
        {
            entityInfo.PhysicAnimator.SetBool(anim_isRagdoll, true);
            entityInfo.PhysicAnimator.enabled = false;
            entityInfo.Input.canRead = false;
            // entityInfo.CharacterCollider.enabled = false;
            spine.transform.SetParent(null);
        }
        else
        {
            entityInfo.PhysicAnimator.SetBool(anim_isRagdoll, false);
            entityInfo.Char.transform.position = GetParentPosAfterRagdoll();
            Vector3 currentPos = spine.position;

            Vector3 fwd = spine.transform.forward;
            fwd.y = 0f;
            fwd.Normalize();

            entityInfo.Char.transform.forward = fwd;

            spine.transform.SetParent(_root);
            spine.position = currentPos;

            entityInfo.Input.canRead = true;
            // entityInfo.CharacterCollider.enabled = true;
            entityInfo.Hurtbox.enabled = true;

            entityInfo.PhysicAnimator.enabled = true;
            entityInfo.PlrNetwork.photonView.TransferOwnership(entityInfo.PlrNetwork.photonView.CreatorActorNr);
        }
    }

    protected override void SetUpRagdoll()
    {
        base.SetUpRagdoll();
        if (!entityInfo.isMultiplayer)
        {
            entityInfo.PhysicAnimator.enabled = false;
            entityInfo.Input.canRead = false;
            entityInfo.CharacterCollider.enabled = false;
            spine.transform.SetParent(null);
        }
        else
        {
            entityInfo.PlrNetwork.RemoteCall(Name, true);
        }
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
        if (!entityInfo.isMultiplayer)
        {
            entityInfo.Char.transform.position = GetParentPosAfterRagdoll();

            Vector3 fwd = spine.transform.forward;
            fwd.y = 0f;
            fwd.Normalize();

            entityInfo.Char.transform.forward = fwd;

            spine.transform.SetParent(_root);
            entityInfo.PlrNetwork.RemoteCall(Name, 0, true);

            entityInfo.Input.canRead = true;
            entityInfo.CharacterCollider.enabled = true;
            entityInfo.Hurtbox.enabled = true;
            // if (entityInfo.PlrNetwork.Photonview.IsMine) entityInfo.CharacterController.enabled = true;

            base.StopRagdoll();
            entityInfo.PhysicAnimator.enabled = false;
        }
        else
        {
            entityInfo.PlrNetwork.RemoteCall(Name, false);
        }

    }
}
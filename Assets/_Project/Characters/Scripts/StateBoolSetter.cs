using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class StateBoolSetter : StateMachineBehaviour
{
    private int stateBoolHash = Animator.StringToHash("IsInCombo");

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.SetBool(stateBoolHash, true);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.SetBool(stateBoolHash, false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        bool isNextAPunch = animator.GetCurrentAnimatorStateInfo(0).IsTag("PunchCombo");

        if (!isNextAPunch) animator.SetBool(stateBoolHash, false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
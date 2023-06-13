using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    public AttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.IsPunching = true;
        // stateMachine.EntityInfo.PunchCombo.punchStarted += OnPunchStarted;
        // stateMachine.EntityInfo.PunchCombo.punchEnded += OnComboEnded;
        stateMachine.EntityInfo.PunchCombo.comboTerminated += OnComboTerminated;
    }

    public override void Tick()
    {
    }

    public override void Exit()
    {
        // stateMachine.EntityInfo.PunchCombo.punchStarted -= OnPunchStarted;
        // stateMachine.EntityInfo.PunchCombo.punchEnded -= OnComboEnded;
        stateMachine.EntityInfo.PunchCombo.comboTerminated -= OnComboTerminated;
    }

    private void OnComboTerminated()
    {
        stateMachine.IsPunching = false;
        stateMachine.EntityInfo.PhysicAnimator.ResetTrigger(stateMachine.EntityInfo.AnimPunch);

        // if (stateMachine.enemyType != EnemyStateMachine.EnemyType.Turtoise)
        // {
        stateMachine.SetState(new GuardState(stateMachine));
        // }
        // else
        // {
        //     stateMachine.SetState(new ApproachPlayerState(stateMachine));
        // }
    }

    private void OnPunchStarted(int punchIndex)
    {
        if (punchIndex == 5 || (punchIndex == 1 && stateMachine.enemyType == EnemyStateMachine.EnemyType.Turtoise))
        {
            stateMachine.IsPunching = false;
            stateMachine.EntityInfo.PhysicAnimator.ResetTrigger(stateMachine.EntityInfo.AnimPunch);
        }
    }

    private void OnComboEnded(int punchIndex)
    {
        if (punchIndex == 5 /*&& stateMachine.enemyType != EnemyStateMachine.EnemyType.Turtoise*/)
        {
            stateMachine.SetState(new GuardState(stateMachine));
        // }
        // else
        // {
        //     stateMachine.SetState(new ApproachPlayerState(stateMachine));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompAbility : AbilityBase
{
    private EntityInfo _entityInfo;
    private Hurtbox _hurtbox;

    #region Listeners
    private void OnAnimationStart(int punchIndex)
    {
        if (punchIndex != 8)
        {
            _entityInfo.PhysicAnimator.ResetTrigger(AnimationHash);
        }
        else
        {
            _entityInfo.Movement.canDash = false;
            _entityInfo.PunchCombo.canPunch = false;
            _hurtbox.receiveDamage = false;
        }
    }

    private void OnHitboxStart(int punchIndex)
    {
        if (punchIndex == 8)
            hitbox.gameObject.SetActive(true);
    }

    private void OnHitboxEnd(int punchIndex)
    {
        if (punchIndex == 8)
            hitbox.gameObject.SetActive(false);
    }

    private void OnAnimationEnd(int punchIndex)
    {
        if (punchIndex == 8)
        {
            _entityInfo.PunchCombo.ResetCombo();
            _hurtbox.receiveDamage = true;
            _entityInfo.Movement.canDash = true;
            _entityInfo.PunchCombo.canPunch = true;
        }
    }
    #endregion

    private void Start()
    {
        _entityInfo = GetComponentInParent<EntityInfo>();
        _hurtbox = _entityInfo.Hurtbox.GetComponent<Hurtbox>();

        _entityInfo.PunchCombo.punchStarted += OnAnimationStart;
        _entityInfo.PunchCombo.hitboxStarted += OnHitboxStart;
        _entityInfo.PunchCombo.hitboxEnded += OnHitboxEnd;
        _entityInfo.PunchCombo.punchEnded += OnAnimationEnd;
    }

    private void Update()
    {
        if (((PlayerInput)_entityInfo.Input).FireAbility1)
        {
            _entityInfo.PhysicAnimator.SetTrigger(AnimationHash);
        }
    }

    private void OnDisable()
    {
        _entityInfo.PunchCombo.punchStarted -= OnAnimationStart;
        _entityInfo.PunchCombo.hitboxStarted -= OnHitboxStart;
        _entityInfo.PunchCombo.hitboxEnded -= OnHitboxEnd;
        _entityInfo.PunchCombo.punchEnded -= OnAnimationEnd;
    }
}
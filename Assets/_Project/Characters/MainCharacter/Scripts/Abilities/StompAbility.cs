using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompAbility : AbilityBase
{
    [SerializeField] private GameObject _stompVFX;
    [SerializeField] private AudioSource _inflateSFX;
    [SerializeField] private AudioSource _deflateSFX;
    [SerializeField] private AudioSource _stompSFX;
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
        {
            hitbox.gameObject.SetActive(true);
            _stompVFX.SetActive(true);
            _stompSFX.Play();
        }
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

    private void OnInflate()
    {
        if (_inflateSFX != null) _inflateSFX.Play();
    }

    private void OnDeflate()
    {
        if (_deflateSFX != null) _deflateSFX.Play();
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
        _entityInfo.PunchCombo.inflateStarted += OnInflate;
        _entityInfo.PunchCombo.deflateStarted += OnDeflate;
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
        _entityInfo.PunchCombo.inflateStarted -= OnInflate;
        _entityInfo.PunchCombo.deflateStarted -= OnDeflate;
    }
}
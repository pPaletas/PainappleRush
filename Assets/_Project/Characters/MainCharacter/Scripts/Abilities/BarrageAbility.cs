using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageAbility : AbilityBase
{
    [SerializeField] private GameObject _vfx;
    [SerializeField] private AudioSource _sfx;
    private EntityInfo _entityInfo;
    private Hurtbox _hurtbox;

    #region Listeners
    private void OnAnimationStart(int punchIndex)
    {
        if (punchIndex != 9)
        {
            _entityInfo.PhysicAnimator.ResetTrigger(AnimationHash);
        }
        else
        {
            _hurtbox.receiveDamage = false;
            _entityInfo.Movement.canDash = false;
            _entityInfo.PunchCombo.canPunch = false;
        }
    }

    private void OnHitboxStart(int punchIndex)
    {
        if (punchIndex == 9)
        {

            _vfx.SetActive(true);
            hitbox.gameObject.SetActive(true);
            if (_sfx != null) _sfx.Play();
        }
    }

    private void OnHitboxEnd(int punchIndex)
    {
        if (punchIndex == 9)
        {
            if (_sfx != null) _sfx.Stop();
            hitbox.gameObject.SetActive(false);
        }
    }

    private void OnAnimationEnd(int punchIndex)
    {
        if (punchIndex == 9)
        {
            _entityInfo.PunchCombo.ResetCombo();
            _hurtbox.receiveDamage = true;
            _entityInfo.Movement.canDash = true;
            _entityInfo.PunchCombo.canPunch = true;
            _vfx.SetActive(false);
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
        if (((PlayerInput)_entityInfo.Input).FireAbility3)
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
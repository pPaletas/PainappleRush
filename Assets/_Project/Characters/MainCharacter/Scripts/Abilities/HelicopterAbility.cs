using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterAbility : AbilityBase
{
    [SerializeField] private GameObject _vfx;

    [Header("Sound")]
    [SerializeField] private AudioSource _sfx;
    [SerializeField] private float _audioSmoothness = 5f;

    private EntityInfo _entityInfo;
    private Hurtbox _hurtbox;

    private float _currentPitch = 1f;
    private float _targetPitch = 1.5f;

    #region Listeners
    private void OnAnimationStart(int punchIndex)
    {
        if (punchIndex != 10)
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
        if (punchIndex == 10)
        {
            hitbox.gameObject.SetActive(true);
            _vfx.SetActive(true);
            _sfx.Play();
        }
    }

    private void OnHitboxEnd(int punchIndex)
    {
        if (punchIndex == 10)
        {
            hitbox.gameObject.SetActive(false);
        }
    }

    private void OnAnimationEnd(int punchIndex)
    {
        if (punchIndex == 10)
        {
            _entityInfo.PunchCombo.ResetCombo();
            _hurtbox.receiveDamage = true;
            _entityInfo.Movement.canDash = true;
            _entityInfo.PunchCombo.canPunch = true;
        }
    }
    #endregion

    private void SmoothPitch()
    {
        float nextTrgt = 1;

        if (hitbox.gameObject.activeSelf)
        {
            nextTrgt = 1.5f;
        }

        _sfx.pitch = Mathf.Lerp(_sfx.pitch, nextTrgt, _audioSmoothness * Time.deltaTime);

        if (!hitbox.gameObject.activeSelf && _sfx.pitch <= 1.1f && _sfx.isPlaying)
        {
            _sfx.Stop();
        }
    }

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
        if (((PlayerInput)_entityInfo.Input).FireAbility2)
        {
            _entityInfo.PhysicAnimator.SetTrigger(AnimationHash);
        }

        SmoothPitch();
    }

    private void OnDisable()
    {
        _entityInfo.PunchCombo.punchStarted -= OnAnimationStart;
        _entityInfo.PunchCombo.hitboxStarted -= OnHitboxStart;
        _entityInfo.PunchCombo.hitboxEnded -= OnHitboxEnd;
        _entityInfo.PunchCombo.punchEnded -= OnAnimationEnd;
    }
}
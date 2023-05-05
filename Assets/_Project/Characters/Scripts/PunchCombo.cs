using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCombo : MonoBehaviour
{
    [SerializeField] private float _dashSpeed = 50f;
    [SerializeField] private float _dashTime = 0.2f;
    [HideInInspector] public bool isPunching = false;

    // Self
    private EntityMovement _movement;
    // Parent
    private EntityInfo _parentInfo;
    private CharacterInput _input;
    // Children
    private Animator _anim;
    private PunchComboAnimationsListener _animListener;
    private GameObject _punchHitbox;
    private GameObject _sweepHitbox;
    private GameObject _dashPunchHitbox;
    private GameObject _dashHitbox;
    private bool _isInCombo = false;

    // Animator
    private int _animPunchHash = Animator.StringToHash("Punch");
    private int _animDashPunch = Animator.StringToHash("DashPunch");
    private int _animInComboHash = Animator.StringToHash("IsInCombo");

    private void ReadInput()
    {
        if (_input.IsPunching)
        {
            TriggerPunch();
        }
    }

    private void TriggerPunch()
    {
        if (_parentInfo.PunchComboCooldown.IsStopped && !_movement.Dashing)
        {
            _anim.SetTrigger(_animPunchHash);
            isPunching = true;
        }
        else if (_movement.Dashing)
        {
            _anim.SetTrigger(_animDashPunch);
        }
    }

    private void ResetCombo()
    {
        isPunching = false;
        _anim.ResetTrigger(_animPunchHash);
        _parentInfo.PunchComboCooldown.StartTimer();
        SetMovement(true);
    }

    #region Listeners
    private void AnimationStarted(int anim)
    {
        if (anim <= 5)
        {
            SetMovement(false);
            _movement.SetPivotForward(_input.MovementVector.normalized);
            if (anim != 5) _movement.Dash(_parentInfo.Pivot.forward, _dashSpeed, _dashTime);
        }
    }

    private void AnimationEnded(int anim)
    {
        if (anim == 5)
            ResetCombo();
    }

    private void HitPointReached(int anim)
    {
        if (anim < 5)
        {
            _punchHitbox.SetActive(true);
        }
        else if (anim == 5)
        {
            _sweepHitbox.SetActive(true);
        }
        else if (anim == 6)
        {
            _dashPunchHitbox.SetActive(true);
        }
        else if (anim == 7)
        {
            _dashHitbox.SetActive(true);
        }
    }

    private void HitPointEnded(int anim)
    {
        _punchHitbox.SetActive(false);
        _sweepHitbox.SetActive(false);
        _dashPunchHitbox.SetActive(false);
        _dashHitbox.SetActive(false);
    }
    #endregion

    private void SetMovement(bool move)
    {
        isPunching = !move;
        _movement.canMove = move;
    }

    // Revisa que todavia esté en el estado de puños, sino lo está, ejecuta el cooldown
    private void CheckIfStillInState()
    {
        if (_isInCombo && !_anim.GetBool(_animInComboHash))
            ResetCombo();

        _isInCombo = _anim.GetBool(_animInComboHash);
    }

    private void Awake()
    {
        // Self
        _movement = GetComponent<CharacterMovement>();
        // Parent
        _parentInfo = GetComponentInParent<EntityInfo>();
        _input = GetComponentInParent<CharacterInput>();
        _anim = GetComponentInChildren<Animator>();
        // Children
        _animListener = GetComponentInChildren<PunchComboAnimationsListener>();
        _punchHitbox = transform.Find("Pivot/Hitboxes/PunchHitbox").gameObject;
        _sweepHitbox = transform.Find("Pivot/Hitboxes/SweepHitbox").gameObject;
        _dashPunchHitbox = transform.Find("Pivot/Hitboxes/DashPunchHitbox").gameObject;
        _dashHitbox = transform.Find("Pivot/Hitboxes/DashHitbox").gameObject;

        // Animation stuff
        _animListener.onAnimationStarted += AnimationStarted;
        _animListener.onAnimationEnded += AnimationEnded;
        _animListener.onHitPointReach += HitPointReached;
        _animListener.onHitPointEnd += HitPointEnded;
    }

    private void Update()
    {
        ReadInput();
        CheckIfStillInState();
    }

    private void OnDisable()
    {
        _animListener.onAnimationStarted -= AnimationStarted;
        _animListener.onAnimationEnded -= AnimationEnded;
        _animListener.onHitPointReach -= HitPointReached;
        _animListener.onHitPointEnd -= HitPointEnded;
    }
}
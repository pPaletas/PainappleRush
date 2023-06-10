using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCombo : MonoBehaviour
{
    public event Action<int> punchStarted;
    public event Action<int> punchEnded;
    public event Action<int> hitboxStarted;
    public event Action<int> hitboxEnded;
    public event Action comboTerminated;
    public event Action inflateStarted;
    public event Action deflateStarted;

    [SerializeField] private bool isOnePunch = false;
    [SerializeField] private bool _isEnemy = false;
    [SerializeField] private float _dashSpeed = 50f;
    [SerializeField] private float _dashTime = 0.2f;

    [Header("VFX")]
    [SerializeField] private GameObject _hitGround;
    [Header("SFX")]
    [SerializeField] private AudioSource _punchSFX;
    [SerializeField] private AudioSource _sweepSFX;

    [HideInInspector] public bool canPunch = true;
    [HideInInspector] public bool isPunching = false;
    private bool _isInCombo = false;


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

    // Animator
    private int _animPunchHash = Animator.StringToHash("Punch");
    private int _animDashPunch = Animator.StringToHash("DashPunch");
    private int _animInComboHash = Animator.StringToHash("IsInCombo");
    private Hurtbox _hurtbox;

    public bool IsInCombo { get => _isInCombo; }

    public void ResetCombo()
    {
        isPunching = false;
        _anim.ResetTrigger(_animPunchHash);
        _parentInfo.PunchComboCooldown.StartTimer();
        SetMovement(true);
    }

    private void ReadInput()
    {
        if (_input.IsPunching)
        {
            TriggerPunch();
        }
    }

    private void TriggerPunch()
    {
        if (canPunch && _parentInfo.PunchComboCooldown.IsStopped && !_movement.Dashing && !_parentInfo.HurtboxComponent.isReceivingDamage)
        {
            _anim.SetTrigger(_animPunchHash);
            isPunching = true;
        }
        else if (_movement.Dashing)
        {
            _anim.SetTrigger(_animDashPunch);
        }
    }

    #region Listeners
    private void AnimationStarted(int anim)
    {
        if (anim <= 5)
        {
            SetMovement(false);

            if (_sweepHitbox != null)
            {
                _movement.SetPivotForward(_input.MovementVector.normalized);
                if (anim != 5) _movement.Dash(_parentInfo.Pivot.forward, _dashSpeed, _dashTime);
            }
            else
            {
                if (anim != 5) _movement.Dash(transform.forward, _dashSpeed, _dashTime);
            }

            if (anim == 5)
                _hurtbox.receiveDamage = false;
        }

        punchStarted?.Invoke(anim);
    }

    private void AnimationEnded(int anim)
    {
        if (anim == 5 || (anim == 1 && isOnePunch))
        {
            ResetCombo();
            _hurtbox.receiveDamage = true;
            comboTerminated?.Invoke();
        }

        punchEnded?.Invoke(anim);
    }

    private void HitPointReached(int anim)
    {
        if (anim < 5)
        {
            _punchHitbox.SetActive(true);
            if (_punchSFX != null) _punchSFX.Play();
        }
        else if (anim == 5)
        {
            if (_sweepHitbox != null)
            {
                _sweepHitbox.SetActive(true);
                _hitGround.SetActive(true);
                _sweepSFX.Play();
            }
            else _punchHitbox.SetActive(true);
        }
        else if (anim == 6)
        {
            if (_dashPunchHitbox != null) _dashPunchHitbox.SetActive(true);
        }
        else if (anim == 7)
        {
            if (_dashHitbox != null) _dashHitbox.SetActive(true);
        }

        hitboxStarted?.Invoke(anim);
    }

    private void HitPointEnded(int anim)
    {
        _punchHitbox.SetActive(false);
        if (!_isEnemy)
        {
            _sweepHitbox.SetActive(false);
            _dashPunchHitbox.SetActive(false);
            _dashHitbox.SetActive(false);
        }
        hitboxEnded?.Invoke(anim);
    }

    private void OnHurt()
    {
        ResetCombo();
        comboTerminated?.Invoke();

        _punchHitbox.SetActive(false);
        if (!_isEnemy)
        {
            _sweepHitbox.SetActive(false);
            _dashPunchHitbox.SetActive(false);
            _dashHitbox.SetActive(false);
        }
    }

    private void OnInflate()
    {
        inflateStarted?.Invoke();
    }

    private void OnDeflate()
    {
        deflateStarted?.Invoke();
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
        _movement = GetComponent<EntityMovement>();
        // Parent
        _parentInfo = GetComponentInParent<EntityInfo>();
        _input = GetComponentInParent<CharacterInput>();
        _anim = GetComponentInChildren<Animator>();
        // Children
        _animListener = GetComponentInChildren<PunchComboAnimationsListener>();
        _punchHitbox = transform.Find("Pivot/Hitboxes/PunchHitbox").gameObject;
        if (!_isEnemy)
        {

            _sweepHitbox = transform.Find("Pivot/Hitboxes/SweepHitbox").gameObject;
            _dashPunchHitbox = transform.Find("Pivot/Hitboxes/DashPunchHitbox").gameObject;
            _dashHitbox = transform.Find("Pivot/Hitboxes/DashHitbox").gameObject;
        }

        // Animation stuff
        _animListener.onAnimationStarted += AnimationStarted;
        _animListener.onAnimationEnded += AnimationEnded;
        _animListener.onHitPointReach += HitPointReached;
        _animListener.onHitPointEnd += HitPointEnded;
        _animListener.onInflateStart += OnInflate;
        _animListener.onDeflateStart += OnDeflate;
    }

    private void Start()
    {
        _hurtbox = _parentInfo.Hurtbox.GetComponent<Hurtbox>();
        _hurtbox.hurted += OnHurt;
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
        _animListener.onInflateStart -= OnInflate;
        _animListener.onDeflateStart -= OnDeflate;

        _hurtbox.hurted -= OnHurt;
    }
}
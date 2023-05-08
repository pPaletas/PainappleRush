using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public interface IRemoteCallable
{
    public string Name { get; }
    public void RemoteInvoke(object[] parameters);
}

public class PunchCombo : MonoBehaviour, IRemoteCallable
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
    private bool _isInCombo = false;

    // Animator
    private int _animPunchHash = Animator.StringToHash("Punch");
    private int _animDashPunch = Animator.StringToHash("DashPunch");
    private int _animInComboHash = Animator.StringToHash("IsInCombo");

    public string Name => "PunchCombo";

    public void RemoteInvoke(object[] parameters)
    {
        int type = (int)parameters[0];

        if (type == 0)
        {
            int colliderIndex = (int)parameters[1];
            bool active = (bool)parameters[2];

            switch (colliderIndex)
            {
                case 0:
                    _punchHitbox.SetActive(active);
                    break;
                case 1:
                    _sweepHitbox.SetActive(active);
                    break;
                case 2:
                    _dashPunchHitbox.SetActive(active);
                    break;
            }
        }
        else
        {
            // Le hacemos evento mas bien a esa baina del trigger, porque sino no funciona el malparetas ese
            _anim.SetTrigger((int)parameters[1]);
        }
    }

    protected bool IsSingleOrOwner()
    {
        bool isMultiplayer = _parentInfo.isMultiplayer;
        return !isMultiplayer || (_parentInfo.PlrNetwork.Photonview.IsMine);
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
        if (_parentInfo.PunchComboCooldown.IsStopped && !_movement.Dashing)
        {
            if (!_parentInfo.isMultiplayer) _anim.SetTrigger(_animPunchHash);
            else _parentInfo.PlrNetwork.RemoteCall(Name, 1, _animPunchHash);


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

    private void SetCollider(int index, bool active)
    {
        // if (!_parentInfo.isMultiplayer)
        switch (index)
        {
            case 0:
                _punchHitbox.SetActive(active);
                break;
            case 1:
                _sweepHitbox.SetActive(active);
                break;
            case 2:
                _dashPunchHitbox.SetActive(active);
                break;
        }
        // else if (_parentInfo.PlrNetwork.Photonview.IsMine) _parentInfo.PlrNetwork.RemoteCall(Name, 0, index, active);
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
            SetCollider(0, true);
        }
        else if (anim == 5)
        {
            SetCollider(1, true);
        }
        else if (anim == 6)
        {
            SetCollider(2, true);
        }
    }

    private void HitPointEnded(int anim)
    {
        SetCollider(0, false);
        SetCollider(1, false);
        SetCollider(2, false);
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

        // Animation stuff
        // Aseguremonos de que los eventos solo sean llamados desde el cliente (owner)
        if (IsSingleOrOwner())
        {
            _animListener.onAnimationStarted += AnimationStarted;
            _animListener.onAnimationEnded += AnimationEnded;
            _animListener.onHitPointReach += HitPointReached;
            _animListener.onHitPointEnd += HitPointEnded;
        }
    }

    private void Update()
    {
        ReadInput();
        CheckIfStillInState();
    }

    private void OnDisable()
    {
        if (IsSingleOrOwner())
        {
            _animListener.onAnimationStarted -= AnimationStarted;
            _animListener.onAnimationEnded -= AnimationEnded;
            _animListener.onHitPointReach -= HitPointReached;
            _animListener.onHitPointEnd -= HitPointEnded;
        }
    }
}
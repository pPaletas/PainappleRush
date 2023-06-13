using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : EntityMovement
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private AudioSource _audio1;
    [SerializeField] private AudioSource _audio2;
    private CharacterController _cc;
    private PunchComboAnimationsListener _listener;

    protected override void Start()
    {
        base.Start();
        _cc = GetComponent<CharacterController>();
        _listener = GetComponentInChildren<PunchComboAnimationsListener>();

        _listener.onStep += HandleStep;
    }

    private void HandleStep(int audio)
    {
        if (audio == 0)
            _audio1.Play();
        else
            _audio2.Play();
    }

    protected override void Update()
    {
        base.Update();

        parentInfo.PhysicAnimator.SetBool(anim_isRunning, movementVector.sqrMagnitude != 0f);
        Vector3 finalVector = movementVector + (Vector3.down * 50f);

        if (canMove && _cc.enabled) _cc.Move(finalVector * Time.deltaTime);
    }

    protected override void DashUpdate()
    {
        if (isDashing)
        {
            if (_cc.enabled) _cc.Move(dashDirection * dashCurrentSpeed * Time.deltaTime);
            dashCurrentTime -= Time.deltaTime;

            if (dashCurrentTime <= 0f)
            {
                StopDash();
            }
        }
    }

    private void KeepPlayerOnGround()
    {
        if (Physics.CheckSphere(transform.position, 0, _groundMask) && (canMove || canDash))
        {
            _cc.Move(Vector3.down * 50f);
        }
    }
}
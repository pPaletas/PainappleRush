using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : EntityMovement
{
    [SerializeField] private LayerMask _groundMask;
    private CharacterController _cc;

    protected override void Start()
    {
        base.Start();
        _cc = GetComponent<CharacterController>();
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
            _cc.Move(dashDirection * dashCurrentSpeed * Time.deltaTime);
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
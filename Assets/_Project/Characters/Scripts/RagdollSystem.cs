using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSystem : MonoBehaviour
{
    [SerializeField] protected float wakeUpTime = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isArmatureRoot = false;
    [SerializeField] protected List<ConfigurableJoint> hardLimbs = new List<ConfigurableJoint>();

    protected Rigidbody spine;
    protected List<ConfigurableJoint> limbs = new List<ConfigurableJoint>();
    protected Transform _root;

    private List<Rigidbody> _limbsRb = new List<Rigidbody>();
    private List<Vector3> _limbsOrigPos = new List<Vector3>();

    protected EntityInfo entityInfo;
    protected bool isKO = false;

    // Stun state
    private bool _isStuned = false;
    private bool _isWakingUp = false;
    protected float wakeUpCurrentStunTime;
    private float _stunCurrentStunTime;

    private float _rootDrive = 5000f;
    private float _defaultDrive = 3000f;

    // Animation
    protected int anim_isRagdoll = Animator.StringToHash("IsRagdoll");

    public bool IsStuned { get => _isStuned; }

    public void Push(Vector3 force, float stunTime)
    {
        _isStuned = true;
        _stunCurrentStunTime = stunTime;
        SetRagdoll(true);
        spine.AddForce(force, ForceMode.VelocityChange);
        wakeUpCurrentStunTime = 0;
    }

    public void PushKO(Vector3 force, float stunTime)
    {
        _isStuned = true;
        _stunCurrentStunTime = stunTime;
        SetRagdoll(true);
        SetJointsActive(true);
        spine.AddForce(force, ForceMode.Impulse);
        wakeUpCurrentStunTime = 2f;
    }

    public void DestroyRoot()
    {
        Destroy(spine.gameObject);
    }

    protected virtual void SetUpRagdoll()
    {
        // entityInfo.PhysicAnimator.SetBool(anim_isRagdoll, true);
        // entityInfo.PhysicAnimator.SetTrigger("Hurt");
        entityInfo.PhysicAnimator.Play("Idle");
        entityInfo.PhysicAnimator.enabled = false;
    }
    protected virtual bool WakeUp() { return true; }
    protected virtual void StopRagdoll()
    {
        // entityInfo.PhysicAnimator.SetBool(anim_isRagdoll, false);
        entityInfo.PhysicAnimator.enabled = true;
        entityInfo.HurtboxComponent.isReceivingDamage = false;
    }

    protected void SetJointsActive(bool ko)
    {
        isKO = ko;

        foreach (ConfigurableJoint joint in limbs)
        {
            JointDrive xDrive = joint.angularXDrive;
            JointDrive yzDrive = joint.angularYZDrive;
            xDrive.positionSpring = !ko ? _defaultDrive : 0f;
            yzDrive.positionSpring = !ko ? _defaultDrive : 0f;

            if (joint.gameObject == spine.gameObject)
            {
                xDrive.positionSpring = !ko ? _rootDrive : 0f;
                yzDrive.positionSpring = !ko ? _rootDrive : 0f;
            }

            joint.angularXDrive = xDrive;
            joint.angularYZDrive = xDrive;
        }

        foreach (ConfigurableJoint joint in hardLimbs)
        {
            JointDrive xDrive = joint.angularXDrive;
            JointDrive yzDrive = joint.angularYZDrive;
            xDrive.positionSpring = _defaultDrive;
            yzDrive.positionSpring = _defaultDrive;

            joint.angularXDrive = xDrive;
            joint.angularYZDrive = xDrive;
        }
    }

    protected Vector3 GetParentPosAfterRagdoll()
    {
        RaycastHit hit;
        Ray ray = new Ray(spine.position + Vector3.up * 0.2f, Vector3.down);

        if (Physics.Raycast(ray, out hit, 2f, groundMask))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    private void SetRagdoll(bool ragdoll)
    {
        if (ragdoll) SetUpRagdoll();

        foreach (Rigidbody limb in _limbsRb)
        {
            limb.isKinematic = !ragdoll;
            limb.useGravity = ragdoll;
        }
    }

    private void SetupRigidbodies()
    {
        foreach (Rigidbody limb in _limbsRb)
        {
            limb.solverIterations = 12;
            limb.solverVelocityIterations = 12;
            limb.maxAngularVelocity = 20f;
        }
    }

    private bool IsOnGround()
    {
        Ray ray = new Ray(spine.position + Vector3.up * 0.2f, Vector3.down);

        if (Physics.Raycast(ray, 2f, groundMask))
        {
            return true;
        }

        return false;
    }

    private void UpdateRagdollState()
    {
        if (_isStuned)
        {
            _stunCurrentStunTime -= Time.deltaTime;

            if (_stunCurrentStunTime <= 0f && entityInfo && IsOnGround())
            {
                _isWakingUp = true;
                _isStuned = false;
            }
        }
        else if (_isWakingUp)
        {
            bool wokeUp = WakeUp();

            if (wokeUp)
            {
                _isWakingUp = false;
                StopRagdoll();
                SetRagdoll(false);
            }
        }
    }

    private void Awake()
    {
        Transform physicBody = transform.Find("Pivot/PhysicBody");
        entityInfo = GetComponentInParent<EntityInfo>();

        if (physicBody != null)
        {
            string armaturePath = "Pivot/PhysicBody/Armature";
            _root = !isArmatureRoot ? transform.Find(armaturePath + "/Root").transform : transform.Find(armaturePath).transform;
            spine = _root.GetChild(0).GetComponent<Rigidbody>();
            limbs.AddRange(_root.GetComponentsInChildren<ConfigurableJoint>());
            _limbsRb.AddRange(_root.GetComponentsInChildren<Rigidbody>());

            foreach (Rigidbody limb in _limbsRb)
            {
                _limbsOrigPos.Add(limb.transform.localPosition);
            }
        }

        SetupRigidbodies();
    }

    private void Update()
    {
        UpdateRagdollState();
    }
}
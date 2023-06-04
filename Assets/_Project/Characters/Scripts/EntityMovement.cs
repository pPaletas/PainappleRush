using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] protected float speed = 5f;
    [SerializeField] private float _rotationSmoothness = 15f;
    [Header("Dash")]
    [SerializeField] protected float dashSpeed = 50f;
    [SerializeField] protected float dashTime = 0.2f;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canDash = true;

    protected Vector3 movementVector;
    protected bool usePivot = true;

    // Dash interno
    protected bool isDashing = false;
    protected float dashCurrentSpeed;
    protected float dashCurrentTime;
    protected Vector3 dashDirection;
    protected bool enableMovementOnFinish;

    // Parent
    protected CharacterInput input;
    protected EntityInfo parentInfo;

    // Children
    private Transform _pivot;
    private Timer _dashCooldown;

    // Animator
    protected int anim_isRunning = Animator.StringToHash("IsRunning");
    protected int anim_Dash = Animator.StringToHash("Dash");

    private bool _dashing = false;

    public bool Dashing { get => _dashing; }

    public void SetPivotForward(Vector3 normalized)
    {
        if (normalized.sqrMagnitude != 0f)
        {
            if (usePivot)
                _pivot.forward = normalized;
            else
                transform.forward = normalized;
        }
    }

    public void Dash(Vector3 direction, float speed, float time, bool enableMovementOnFinish = false)
    {
        if (!isDashing)
        {
            isDashing = true;
            dashCurrentSpeed = speed;
            dashCurrentTime = time;
            SetPivotForward(direction);
            dashDirection = direction;
            Debug.DrawRay(transform.position, direction, Color.yellow, 2f);
            this.enableMovementOnFinish = enableMovementOnFinish;
            canMove = false;
        }
    }

    public virtual void SetAgent(bool active) { }

    public virtual void SetSpeed(float speed) { }

    protected virtual void StopDash()
    {
        isDashing = false;
        dashCurrentSpeed = 0f;
        dashCurrentTime = 0f;
        dashDirection = Vector3.zero;
        canMove = true && enableMovementOnFinish;
        parentInfo.PhysicAnimator.ResetTrigger(anim_Dash);

        _dashCooldown.StartTimer();
    }

    protected virtual void DashUpdate() { }

    protected virtual void Start()
    {
        // Parent
        input = GetComponentInParent<CharacterInput>();
        parentInfo = GetComponentInParent<EntityInfo>();
        // Children
        _pivot = transform.Find("Pivot");
        _dashCooldown = parentInfo.DashCooldownTimer;
    }

    protected virtual void Update()
    {
        movementVector = Vector3.zero;

        // bool isStunned = _parentInfo.Ragdoll.IsStunned;
        CheckIfCanDash();
        DashUpdate();

        if (canMove /*&& !isStunned*/)
        {
            if (input.MovementVector != Vector3.zero) RotateTowardsMovement();
            ProcessInput();
        }

        SetDashingVariable();
    }

    protected virtual void RotateTowardsMovement()
    {
        Quaternion targetRotation = Quaternion.LookRotation(input.MovementVector.normalized, Vector3.up);
        Quaternion currentRotation = _pivot.rotation;

        _pivot.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSmoothness * Time.deltaTime);
    }

    // Determina como se va a procesar el input
    protected virtual void ProcessInput()
    {
        movementVector = input.MovementVector.normalized * speed;
    }

    // Actualiza la variable _dashing para que pueda ser interpretada como que el jugador está haciendo dash
    private void SetDashingVariable()
    {
        if (!isDashing && _dashing) _dashing = false;
    }

    private void CheckIfCanDash()
    {
        if (input.IsDashing && _dashCooldown.IsStopped && canDash)
        {
            // Recordar que el MovementVector de los enemigos, es la posición del jugador
            Vector3 dashDir = usePivot ? input.MovementVector.normalized : (input.MovementVector - transform.position).normalized;
            if (dashDir.sqrMagnitude == 0f && _pivot) dashDir = _pivot.forward;
            Dash(dashDir, dashSpeed, dashTime, true);
            parentInfo.PhysicAnimator.SetTrigger(anim_Dash);
            _dashing = true;
        }
    }
}
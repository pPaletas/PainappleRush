using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityInfo : MonoBehaviour
{
    // Timers
    private Timer _dashCooldownTimer;
    private Timer _dashTimer;
    private Timer _punchDashTimer;
    private Timer _punchComboCooldown;

    // Transforms
    private Transform _pivot;

    // Others
    private CharacterInput _input;
    private GameObject _char;
    private Rigidbody _rb;
    private RagdollSystem _ragdoll;
    private NavMeshAgent _agent;
    private CharacterController _cc;
    private EntityMovement _movement;
    private CapsuleCollider _characterCollider;
    private CapsuleCollider _hurtbox;
    private Hurtbox _hurtboxComponent;
    private Health _health;
    private Animator _physicAnimator;
    private Animator _fakeAnimator;
    private EnemyStateMachine _stateMachine;
    private PunchCombo _punchCombo;

    // Animator
    private int _animIsRunning = Animator.StringToHash("IsRunning");
    private int _animIsRushing = Animator.StringToHash("IsRushing");
    private int _animPunch = Animator.StringToHash("Punch");

    // Timers
    public Timer DashCooldownTimer { get => _dashCooldownTimer; }
    public Timer DashTimer { get => _dashTimer; }
    public Timer PunchDashTimer { get => _punchDashTimer; }
    public Timer PunchComboCooldown { get => _punchComboCooldown; }

    // Transforms
    public Transform Pivot { get => _pivot; }

    // Others
    public GameObject Char { get => _char; }
    public Rigidbody Rb { get => _rb; }
    public RagdollSystem Ragdoll { get => _ragdoll; }
    public NavMeshAgent Agent { get => _agent; }
    public EntityMovement Movement { get => _movement; }
    public CapsuleCollider CharacterCollider { get => _characterCollider; }
    public CapsuleCollider Hurtbox { get => _hurtbox; }
    public Health Health { get => _health; }
    public Animator PhysicAnimator { get => _physicAnimator; }
    public Animator FakeAnimator { get => _fakeAnimator; }
    public CharacterInput Input { get => _input; }
    public CharacterController CharacterController { get => _cc; }
    public EnemyStateMachine StateMachine { get => _stateMachine; }
    public PunchCombo PunchCombo { get => _punchCombo; }

    // Animator
    public int AnimIsRunning { get => _animIsRunning; }
    public int AnimIsRushing { get => _animIsRushing; }
    public int AnimPunch { get => _animPunch; }
    public Hurtbox HurtboxComponent { get => _hurtboxComponent; }

    private void Awake()
    {
        // Timers
        _dashCooldownTimer = transform.Find("Timers/DashCooldown").GetComponent<Timer>();
        _dashTimer = transform.Find("Timers/DashTime").GetComponent<Timer>();
        _punchDashTimer = transform.Find("Timers/PunchDashTime").GetComponent<Timer>();
        _punchComboCooldown = transform.Find("Timers/PunchComboCooldown").GetComponent<Timer>();

        // Transforms
        _pivot = transform.Find("Character/Pivot");

        // Others
        _input = GetComponent<CharacterInput>();
        _char = transform.Find("Character").gameObject;
        _hurtbox = GetComponentInChildren<Hurtbox>().GetComponent<CapsuleCollider>();
        _hurtboxComponent = GetComponentInChildren<Hurtbox>();
        _rb = transform.Find("Character").GetComponent<Rigidbody>();
        _ragdoll = transform.Find("Character").GetComponent<RagdollSystem>();
        _agent = transform.Find("Character").GetComponent<NavMeshAgent>();
        _cc = transform.Find("Character").GetComponent<CharacterController>();
        _movement = _char.GetComponent<EntityMovement>();
        _characterCollider = _char.GetComponent<CapsuleCollider>();
        _health = _char.GetComponent<Health>();
        _physicAnimator = _pivot.GetComponentInChildren<Animator>(false);
        _fakeAnimator = GetComponentInChildren<Animator>(false);
        _stateMachine = GetComponent<EnemyStateMachine>();
        _punchCombo = _char.GetComponent<PunchCombo>();
    }
}
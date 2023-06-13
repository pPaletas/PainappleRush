using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;

    public EnemyState(EnemyStateMachine stateMachine) { this.stateMachine = stateMachine; }

    public abstract void Enter();
    public abstract void Tick();
    public virtual void LateTick() { }
    public abstract void Exit();
}

[System.Serializable]
public struct StateMachineInfo
{
    [Header("Movement")]
    public float distanceToDash;
    public float stoppingDistance;
    public Vector2 secondsToAttack;
    [Header("Standing Still")]
    public float rotationSmoothness;
    [Header("Avoidance")]
    public LayerMask avoidanceLayer;
    public int raycastCheckRes;
    public float targetChangeSpeed;
    [Header("Attack")]
    public float attackRadius;
    public float attackAngle;
}

public class EnemyStateMachine : CharacterInput
{
    public enum EnemyType { Police, Turtoise, RIOT }

    public EnemyType enemyType;
    public StateMachineInfo info;

    private EnemyState _currentState;
    private EntityInfo _entityInfo;
    private GameObject _vfx_bloodExplosion;
    private AudioSource _sfx_bloodExplosion;

    // Debug
    public string currentStateName;

    public EntityInfo EntityInfo { get => _entityInfo; }

    public void SetState(EnemyState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    #region Methods used by states
    public float GetDistanceFromPlayer()
    {
        if (SceneInfo.Instance.PlrCharacter != null)
        {
            Vector3 plrPos = SceneInfo.Instance.PlrCharacter.position;
            Vector3 myPos = EntityInfo.Char.transform.position;

            return Vector3.Distance(plrPos, myPos);
        }
        else
        {
            return 0f;
        }

    }

    public Vector3 GetDirectionToPlayer()
    {
        if (SceneInfo.Instance.PlrRoot == null) return Vector3.zero;

        Vector3 plrPos = SceneInfo.Instance.PlrRoot.position;
        Vector3 myPos = EntityInfo.Char.transform.position;

        return (plrPos - myPos).normalized;
    }
    #endregion

    private void ExplodeOnDeath()
    {
        if (EntityInfo.Health.CurrentHealth <= 0f)
        {
            _vfx_bloodExplosion.transform.SetParent(null);
            _vfx_bloodExplosion.SetActive(true);
            _sfx_bloodExplosion.Play();
            EntityInfo.transform.SetParent(null);
            EnemiesManager.Instance.NotifyEnemyDeath();
            EntityInfo.gameObject.SetActive(false);
            GameObject.Destroy(EntityInfo.gameObject);
            EntityInfo.Ragdoll.DestroyRoot();
        }
    }

    private void Awake()
    {
        _currentState = new ApproachPlayerState(this);
        _entityInfo = GetComponent<EntityInfo>();
    }

    private void Start()
    {
        _currentState.Enter();

        if (enemyType != EnemyType.Turtoise)
        {
            _vfx_bloodExplosion = _entityInfo.transform.Find("Character/Pivot/PhysicBody/Armature/Root/Spine/BloodExplosion").gameObject;
        }
        else
        {
            _vfx_bloodExplosion = _entityInfo.transform.Find("Character/Pivot/PhysicBody/Armature/Root/BloodExplosion").gameObject;
        }

        _sfx_bloodExplosion = _vfx_bloodExplosion.GetComponent<AudioSource>();
    }

    private void Update()
    {
        _currentState?.Tick();
        ExplodeOnDeath();

        currentStateName = _currentState.ToString();
    }

    private void LateUpdate()
    {
        _currentState?.LateTick();
    }
}
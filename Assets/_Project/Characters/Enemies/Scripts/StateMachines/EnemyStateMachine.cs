using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;

    public EnemyState(EnemyStateMachine stateMachine) { this.stateMachine = stateMachine; }

    public abstract void Enter();
    public abstract void Tick();
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
}

public class EnemyStateMachine : CharacterInput
{
    public StateMachineInfo info;

    private EnemyState _currentState;
    private EntityInfo _entityInfo;
    private GameObject _vfx_bloodExplosion;
    private CoinManager _coinManager;

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
        Vector3 plrPos = SceneInfo.sceneInfo.PlrCharacter.position;
        Vector3 myPos = EntityInfo.Char.transform.position;

        return Vector3.Distance(plrPos, myPos);
    }

    public Vector3 GetDirectionToPlayer()
    {
        Vector3 plrPos = SceneInfo.sceneInfo.PlrCharacter.position;
        Vector3 myPos = EntityInfo.Char.transform.position;

        return (plrPos - myPos).normalized;
    }
    #endregion

    private void Awake()
    {
        _currentState = new ApproachPlayerState(this);
        _entityInfo = GetComponent<EntityInfo>();
        _vfx_bloodExplosion = transform.Find("Character/BloodExplosion").gameObject;
        _coinManager = FindObjectOfType<CoinManager>();

    }

    private void Update()
    {
        _currentState?.Tick();

        //TEMP

        if (EntityInfo.Health.CurrentHealth <= 0f)
        {
            _vfx_bloodExplosion.transform.SetParent(null);
            _vfx_bloodExplosion.SetActive(true);
            _coinManager.ImDead(EntityInfo.Char.transform);
            EntityInfo.gameObject.SetActive(false);
            GameObject.Destroy(EntityInfo.gameObject);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Hurtbox : MonoBehaviour
{
    public event Action hurted;

    public string bloodPoolName;
    [Header("Counter")]
    [SerializeField] private GameObject _counterHitbox;
    [SerializeField] private GameObject _counterVFX;
    [SerializeField] private float _counterCooldown = 0.5f;
    [SerializeField] private Image _punchButton;
    [SerializeField] private Sprite _punchSprite;
    [SerializeField] private Sprite _angerSprite;

    [HideInInspector] public bool isReceivingDamage = false;
    [HideInInspector] public bool receiveDamage = true;

    private ParticlesPool _bloodPool;
    private Health _health;
    private EntityMovement _movement;
    private EntityInfo _entityInfo;
    private AudioSource _source;

    // Counter
    private float _timeSinceLastCounter = 1000f;
    private int _hitsReceived = 0;
    private bool _counterAvailable = false;
    private Coroutine restartCoroutine;

    public EntityInfo EntityInfo { get => _entityInfo; }

    public void Hurt(HitData hitdata)
    {
        // Si el hurtbox no está habilitado, o es jugador y esta dasheando
        if (!receiveDamage || (_entityInfo.Agent == null && _entityInfo.Movement.Dashing)) return;

        if (_entityInfo.Agent == null) CheckCounter();

        hurted?.Invoke();

        _health.TakeDamage(hitdata.damage);

        isReceivingDamage = true;

        // Ya explotó, ragdoll no funcionará
        if (_health.CurrentHealth <= 0f)
        {
            return;
        }

        Vector3 force = hitdata.force;

        if (force.normalized == Vector3.down)
            force = (transform.position - hitdata.info.Pivot.transform.position).normalized * hitdata.force.magnitude;

        if (hitdata.damage > 0f)
        {
            SpawnBloodParticle(force);
            _source.Play();
        }

        if (hitdata.pushType == 0)
        {
            // _movement.Dash(info.Pivot.forward, 30f, 0.05f);
            _entityInfo.Ragdoll.Push(force, 0.8f);
        }
        else
        {
            _entityInfo.Ragdoll.PushKO(force, 2f);
        }
    }

    public void Counter()
    {
        if (_counterAvailable)
        {
            _timeSinceLastCounter = 0f;
            _counterHitbox.SetActive(true);
            _counterVFX.SetActive(true);
            receiveDamage = false;

            StartCoroutine(DisableCounterHitbox());

            _counterAvailable = false;
            _punchButton.sprite = _punchSprite;
        }
    }

    private IEnumerator RestartCounter()
    {
        yield return new WaitForSeconds(0.5f);
        _counterAvailable = false;
        _hitsReceived = 0;
        _timeSinceLastCounter = 0f;
        _punchButton.sprite = _punchSprite;
    }

    private IEnumerator DisableCounterHitbox()
    {
        yield return new WaitForSeconds(0.01f);
        _counterHitbox.SetActive(false);
        receiveDamage = true;
    }

    private void SpawnBloodParticle(Vector3 force)
    {
        if (_bloodPool == null) return;

        GameObject particle = _bloodPool.Pool.Get();
        particle.transform.position = transform.position + force.normalized;

        Quaternion vectorToRot = Quaternion.LookRotation(force.normalized, Vector3.up);
        particle.transform.rotation = vectorToRot;

        particle.SetActive(true);
    }

    private void CheckCounter()
    {
        if (restartCoroutine != null && !_counterAvailable) StopCoroutine(restartCoroutine);

        _hitsReceived++;
        bool restCor = true;

        if (_hitsReceived >= 5 && _timeSinceLastCounter >= _counterCooldown)
        {
            _timeSinceLastCounter = 0f;
            _hitsReceived = 0;
            _counterAvailable = true;
            _punchButton.sprite = _angerSprite;
        }

        if (restCor) restartCoroutine = StartCoroutine(RestartCounter());
        restCor = !_counterAvailable;
    }

    private void TickCounterCooldown()
    {
        if (_timeSinceLastCounter < _counterCooldown)
        {
            _timeSinceLastCounter += Time.deltaTime;
        }
    }

    private void Awake()
    {
        _entityInfo = GetComponentInParent<EntityInfo>();
        _health = GetComponentInParent<Health>();
        _movement = GetComponentInParent<EntityMovement>();
        _source = GetComponent<AudioSource>();

        _bloodPool = GameObject.Find("ParticlePools/" + bloodPoolName).GetComponent<ParticlesPool>();
    }

    private void Update()
    {
        TickCounterCooldown();
    }
}
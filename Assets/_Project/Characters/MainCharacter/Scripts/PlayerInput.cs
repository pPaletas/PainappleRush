using System;
using System.Collections;
using UnityEngine;

public enum ButtonType { PunchButton, DashButton }

public class PlayerInput : CharacterInput
{
    [SerializeField] private bool _canRead = true;
    [SerializeField] private DeathScreen _deathScreen;

    private bool _isDeath = false;

    [Header("Player Only Input")]
    private bool fireAbility1;
    private bool fireAbility2;
    private bool fireAbility3;

    private FloatingJoystick _joystick;
    private EntityInfo _entityInfo;
    private GameObject _vfx_bloodExplosion;
    private AudioSource _sfx_bloodExplosion;

    public bool FireAbility1 { get => fireAbility1; }
    public bool FireAbility2 { get => fireAbility2; }
    public bool FireAbility3 { get => fireAbility3; }

    #region Callbacks
    public void OnButtonPress(int buttonType)
    {
        if (!_canRead || !canRead) return;

        if (buttonType == 0)
        {
            isPunching = true;
        }
        else if (buttonType == 1)
        {
            isDashing = true;
        }
        else if (buttonType == 2)
        {
            fireAbility1 = true;
        }
        else if (buttonType == 3)
        {
            fireAbility2 = true;
        }
        else if (buttonType == 4)
        {
            fireAbility3 = true;
        }
    }

    private void OnDeathScreenVisible()
    {
        GameObject.Destroy(_entityInfo.gameObject);
    }
    #endregion

    private void ExplodeOnDeath()
    {
        if (_entityInfo.Health.CurrentHealth <= 0f && !_isDeath)
        {
            _isDeath = true;
            _vfx_bloodExplosion.transform.SetParent(null);
            _vfx_bloodExplosion.SetActive(true);
            _sfx_bloodExplosion.Play();

            _deathScreen.gameObject.SetActive(true);
            _deathScreen.ShowDeathScreen();
            _entityInfo.Char.SetActive(false);
            _entityInfo.Ragdoll.DestroyRoot();
        }
    }

    private void Awake()
    {
        _joystick = FindObjectOfType<FloatingJoystick>();
        _entityInfo = GetComponent<EntityInfo>();
        _vfx_bloodExplosion = _entityInfo.transform.Find("Character/Pivot/PhysicBody/Armature/Root/Spine/BloodExplosion").gameObject;
        _sfx_bloodExplosion = _vfx_bloodExplosion.GetComponent<AudioSource>();

        _deathScreen.deathScreenSetVisible += OnDeathScreenVisible;
    }

    private void Update()
    {
        ExplodeOnDeath();

        if (_canRead && canRead)
        {
            movementVector = new Vector3(-_joystick.Direction.y, 0f, _joystick.Direction.x);
        }
    }

    private void LateUpdate()
    {
        isPunching = false;
        isDashing = false;
        fireAbility1 = false;
        fireAbility2 = false;
        fireAbility3 = false;
    }
}
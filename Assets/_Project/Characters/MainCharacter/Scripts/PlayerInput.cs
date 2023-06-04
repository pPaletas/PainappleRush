using UnityEngine;

public enum ButtonType { PunchButton, DashButton }

public class PlayerInput : CharacterInput
{
    [SerializeField] private bool _canRead = true;

    [Header("Player Only Input")]
    private bool fireAbility1;
    private bool fireAbility2;
    private bool fireAbility3;

    private FloatingJoystick _joystick;

    public bool FireAbility1 { get => fireAbility1; }
    public bool FireAbility2 { get => fireAbility2; }
    public bool FireAbility3 { get => fireAbility3; }

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

    private void Awake()
    {
        _joystick = FindObjectOfType<FloatingJoystick>();
    }

    private void Update()
    {
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
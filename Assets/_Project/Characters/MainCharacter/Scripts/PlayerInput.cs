using UnityEngine;

public enum ButtonType { PunchButton, DashButton }

public class PlayerInput : CharacterInput
{
    [SerializeField] private bool _canRead = true;

    private FloatingJoystick _joystick;
    private bool wasPunchingLastFrame = false;
    private bool wasDashingLastFrame = false;

    public void OnButtonPress(int buttonType)
    {
        if(!_canRead || !canRead) return;

        if (buttonType == 0)
        {
            isPunching = true;
        }
        else
        {
            isDashing = true;
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

        if (isPunching && !wasPunchingLastFrame)
        {
            wasPunchingLastFrame = true;
        }
        else if (isPunching && wasPunchingLastFrame)
        {
            isPunching = false;
            wasPunchingLastFrame = false;
        }

        if (isDashing && !wasDashingLastFrame)
        {
            wasDashingLastFrame = true;
        }
        else if (isDashing && wasDashingLastFrame)
        {
            isDashing = false;
            wasDashingLastFrame = false;
        }
    }
}
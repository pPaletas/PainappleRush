using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    protected Vector3 movementVector;
    protected bool isDashing = false;
    protected bool isPunching = false;
    [HideInInspector] public bool canRead = true;

    public Vector3 MovementVector { get => movementVector; set => movementVector = value; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public bool IsPunching { get => isPunching; set => isPunching = value; }
}
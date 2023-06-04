using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    public bool staticAbility = false;
    public int probability = 100;
    public int triggerPunch = 0;
    [SerializeField] protected Collider hitbox;
    [SerializeField] private string _animationName;
    [HideInInspector] public string assignedButtonName;

    public int AnimationHash { get; private set; }

    protected virtual void Awake()
    {
        AnimationHash = Animator.StringToHash(_animationName);
    }
}
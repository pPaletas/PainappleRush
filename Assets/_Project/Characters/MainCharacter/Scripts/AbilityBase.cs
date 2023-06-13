using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBase : MonoBehaviour
{
    public bool staticAbility = false;
    public int probability = 100;
    public int triggerPunch = 0;
    [SerializeField] protected Collider hitbox;
    [SerializeField] private string _animationName;

    [Header("Ability Presentation")]
    [SerializeField] private GameObject _presentationObject;
    [SerializeField] private float _presentationTime = 2f;

    [HideInInspector] public string assignedButtonName;

    public int AnimationHash { get; private set; }

    public void PresentateAbility(Button btnToActivate)
    {
        _presentationObject.SetActive(true);
        StartCoroutine(DisablePresentation(btnToActivate));
    }

    private IEnumerator DisablePresentation(Button btnToActivate)
    {
        yield return new WaitForSeconds(_presentationTime);

        _presentationObject.SetActive(false);
        btnToActivate.gameObject.SetActive(true);
    }

    protected virtual void Awake()
    {
        AnimationHash = Animator.StringToHash(_animationName);
    }
}
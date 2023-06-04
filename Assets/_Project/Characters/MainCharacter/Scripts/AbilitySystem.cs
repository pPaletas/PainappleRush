using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySystem : MonoBehaviour
{
    [SerializeField] private List<Button> _abilityButtons;
    [SerializeField] private List<AbilityBase> _abilities;
    [SerializeField] private float _buttonActiveTime = 0.5f;

    private EntityInfo _entityInfo;

    // Los estaticos son aquellos que siempre estarán activos
    private void CheckIfThereAreStatics()
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            if (_abilities[i].staticAbility) _abilityButtons[i].gameObject.SetActive(true);
        }
    }

    #region Listeners
    private void OnPunchStart(int punchIndex)
    {
        if (punchIndex > 5) return;

        int randomNum = Random.Range(1, 101);

        for (int i = 0; i < _abilities.Count; i++)
        {
            if (_abilities[i].staticAbility || _abilities[i].triggerPunch != punchIndex) continue;

            bool activate = randomNum <= _abilities[i].probability;

            if (activate)
            {
                _abilityButtons[i].gameObject.SetActive(true);
                StartCoroutine(DisableButtonAsync(_abilityButtons[i].gameObject));
            }
        }
    }
    #endregion

    private IEnumerator DisableButtonAsync(GameObject btnGameObject)
    {
        yield return new WaitForSeconds(_buttonActiveTime);

        btnGameObject.SetActive(false);
    }

    private void Start()
    {
        _entityInfo = GetComponentInParent<EntityInfo>();

        _entityInfo.PunchCombo.punchStarted += OnPunchStart;

        CheckIfThereAreStatics();
    }

    private void OnDisable()
    {
        _entityInfo.PunchCombo.punchStarted -= OnPunchStart;
    }
}
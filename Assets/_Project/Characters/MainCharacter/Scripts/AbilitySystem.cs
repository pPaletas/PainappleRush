using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySystem : MonoBehaviour
{
    [SerializeField] private List<int> _newAbilityWaves = new List<int>();
    [SerializeField] private List<Button> _abilityButtons;
    [SerializeField] private List<AbilityBase> _abilities;
    [SerializeField] private float _buttonActiveTime = 0.5f;

    private int _unlockedAbilities = 0;

    private EntityInfo _entityInfo;

    // Los estaticos son aquellos que siempre estarán activos
    // private void CheckIfThereAreStatics()
    // {
    //     for (int i = 0; i < _abilities.Count; i++)
    //     {
    //         if (_abilities[i].staticAbility) _abilityButtons[i].gameObject.SetActive(true);
    //     }
    // }

    #region Listeners
    // private void OnPunchStart(int punchIndex)
    // {
    //     if (punchIndex > 5) return;

    //     int randomNum = Random.Range(1, 101);

    //     for (int i = 0; i < _abilities.Count; i++)
    //     {
    //         if (_abilities[i].staticAbility || _abilities[i].triggerPunch != punchIndex) continue;

    //         bool activate = randomNum <= _abilities[i].probability;

    //         if (activate)
    //         {
    //             _abilityButtons[i].gameObject.SetActive(true);
    //             StartCoroutine(DisableButtonAsync(_abilityButtons[i].gameObject));
    //         }
    //     }
    // }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Intermission && GameplayManager.Instance.CurrentWave == _newAbilityWaves[0] + 1)
        {
            PresentNewAbility();
        }
    }

    #endregion

    private void PresentNewAbility()
    {
        _abilities[_unlockedAbilities].PresentateAbility(_abilityButtons[_unlockedAbilities]);
        _unlockedAbilities++;
    }

    // private IEnumerator DisableButtonAsync(GameObject btnGameObject)
    // {
    //     yield return new WaitForSeconds(_buttonActiveTime);

    //     btnGameObject.SetActive(false);
    // }

    private void Start()
    {
        _entityInfo = GetComponentInParent<EntityInfo>();

        // _entityInfo.PunchCombo.punchStarted += OnPunchStart;
        GameplayManager.Instance.gameStateChanged += OnGameStateChanged;

        // CheckIfThereAreStatics();
    }


    // private void OnDisable()
    // {
    //     _entityInfo.PunchCombo.punchStarted -= OnPunchStart;
    // }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PainappleRushLogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _btn;
    [SerializeField] private GameObject _invalidUsernameTxt;

    private bool _isInvalidUsernameTxtShown = false;

    private IEnumerator ShowInvalidUsernameCoroutine()
    {
        _invalidUsernameTxt.SetActive(true);
        _isInvalidUsernameTxtShown = true;
        yield return new WaitForSeconds(5f);
        _invalidUsernameTxt.SetActive(false);
        _isInvalidUsernameTxtShown = false;
    }

    private bool StoreUsername()
    {
        bool isUsernameValid = !string.IsNullOrEmpty(_inputField.text);

        return isUsernameValid;
    }

    private void JoinRoom()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    private void ShowInvalidUsername()
    {
        if (!_isInvalidUsernameTxtShown)
            StartCoroutine(ShowInvalidUsernameCoroutine());
    }

    private void OnJoinButtonPressed()
    {
        if (StoreUsername())
            JoinRoom();
        else
            ShowInvalidUsername();
    }

    private void Awake()
    {
        _btn.onClick.AddListener(OnJoinButtonPressed);
    }
}
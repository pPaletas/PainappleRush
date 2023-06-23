using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public event Action deathScreenSetVisible;

    [SerializeField] private float _alphaAddition = 0.04f;
    [SerializeField] private float _updateTime = 0.1f;

    private CanvasGroup _group;
    private TextMeshProUGUI _text;
    private AudioSource _audio;

    private bool _canRespawn = false;

    private int _currentSeconds = 10;

    private IEnumerator AsyncShowDeathScreen()
    {
        while (_group.alpha < 1f)
        {
            _group.alpha += _alphaAddition;
            yield return new WaitForSeconds(_updateTime);
        }

        deathScreenSetVisible?.Invoke();
        yield return new WaitForSeconds(1f);
        _text.gameObject.SetActive(true);
        _canRespawn = true;
    }

    private bool GetInputDown()
    {
        bool touchedScreen = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        touchedScreen = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
        touchedScreen = Input.GetMouseButtonDown(0);
#endif

        return touchedScreen;
    }

    private IEnumerator ChangeToNextScene()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowDeathScreen()
    {
        _audio.Play();
        StartCoroutine(AsyncShowDeathScreen());
    }

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        _audio = GetComponent<AudioSource>();
        _text = transform.Find("ReviveText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (GetInputDown())
        {
            StartCoroutine(ChangeToNextScene());
        }
    }
}
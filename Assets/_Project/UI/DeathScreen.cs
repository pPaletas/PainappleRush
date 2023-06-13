using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public event Action deathScreenSetVisible;

    [SerializeField] private float _alphaAddition = 0.04f;
    [SerializeField] private float _updateTime = 0.1f;

    private CanvasGroup _group;
    private TextMeshProUGUI _text;
    private AudioSource _audio;

    private int _currentSeconds = 10;

    public void ShowDeathScreen()
    {
        _audio.Play();
        StartCoroutine(AsyncShowDeathScreen());
    }

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
    }

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        _audio = GetComponent<AudioSource>();
        _text = transform.Find("ReviveText").GetComponent<TextMeshProUGUI>();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUI : MonoBehaviour
{
    [SerializeField] private AudioSource _newWaveAudio;
    [SerializeField] private AudioSource _gameoverAudio;
    [SerializeField] private float _textTime;

    private TextMeshProUGUI _text;

    // Stats
    private Image _watchImage;
    private TextMeshProUGUI _watchText;
    private TextMeshProUGUI _enemiesText;
    private TextMeshProUGUI _waveText;

    private const string TO_INTERMISSION_TEXT = "Wave Cleared!";
    private const string TO_WAVE_TEXT = "Wave ";
    private const string GAMEOVER_TEXT = "Game Over";

    private IEnumerator AsyncHideText()
    {
        yield return new WaitForSeconds(_textTime);
        _text.gameObject.SetActive(false);
    }

    private void OnGameStateChange(GameState state)
    {
        _text.gameObject.SetActive(true);

        if (state == GameState.Intermission)
        {
            _text.text = TO_INTERMISSION_TEXT;
            StartCoroutine(AsyncHideText());
        }
        else if (state == GameState.Wave)
        {
            _text.text = TO_WAVE_TEXT + GameplayManager.Instance.CurrentWave.ToString();
            _newWaveAudio.Play();
            StartCoroutine(AsyncHideText());
        }
        else
        {
            _text.text = GAMEOVER_TEXT;
            _gameoverAudio.Play();
        }
    }

    private void SetDataFromManager()
    {
        if (GameplayManager.Instance.CurrentState == GameState.Wave)
        {
            int timeLeft = GameplayManager.Instance.WaveTime - GameplayManager.Instance.CurrentStateTime;
            _watchImage.fillAmount = ((float)timeLeft / (float)GameplayManager.Instance.WaveTime);
            _watchText.text = timeLeft.ToString();
            _enemiesText.text = EnemiesManager.Instance.GetAmmountOfEnemies().ToString();
            _waveText.gameObject.SetActive(true);
            _waveText.text = "Wave " + GameplayManager.Instance.CurrentWave.ToString();
        }
        else
        {
            _watchImage.fillAmount = 0f;
            _watchText.text = "0";
            _enemiesText.text = "0";
            _waveText.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>(true);

        _watchImage = GameObject.Find("Stats/Watch/WatchFill").GetComponent<Image>();
        _watchText = GameObject.Find("Stats/Watch/Counter").GetComponent<TextMeshProUGUI>();
        _enemiesText = GameObject.Find("Stats/Enemies/Counter").GetComponent<TextMeshProUGUI>();
        _waveText = GameObject.Find("Stats/WaveText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameplayManager.Instance.gameStateChanged += OnGameStateChange;
    }

    private void Update()
    {
        SetDataFromManager();
    }
}
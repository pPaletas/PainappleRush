using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public GameObject lightCoin;
    public GameObject heavyCoin;

    private TextMeshProUGUI _lightText;
    private TextMeshProUGUI _heavyText;

    private int _currentEnemiesKilled = 0;
    private int _collectedLight = 0;
    private int _collectedHeavy = 0;

    public void ImDead(Transform enemyTransform)
    {
        GameObject coin = null;

        if (_currentEnemiesKilled <= 5)
        {
            coin = Instantiate(lightCoin, enemyTransform.transform.position, Quaternion.identity);
        }
        else
        {
            coin = Instantiate(heavyCoin, enemyTransform.transform.position, Quaternion.identity);
            _currentEnemiesKilled = 0;
        }

        coin.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    public void CoinCollected(int type)
    {
        if (type == 0)
        {
            _collectedLight++;
        }
        else _collectedHeavy++;

        _lightText.text = _collectedLight.ToString();
        _heavyText.text = _collectedHeavy.ToString();
    }

    private void Awake()
    {
        _lightText = transform.Find("LightCoin").GetComponentInChildren<TextMeshProUGUI>();
        _heavyText = transform.Find("HeavyCoin").GetComponentInChildren<TextMeshProUGUI>();

        _lightText.text = _collectedLight.ToString();
        _heavyText.text = _collectedHeavy.ToString();
    }
}
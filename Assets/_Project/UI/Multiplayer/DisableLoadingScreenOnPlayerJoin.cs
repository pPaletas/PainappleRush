using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLoadingScreenOnPlayerJoin : MonoBehaviour
{
    [SerializeField] private GameObject _provisionalCam;

    private void PlayerJoined()
    {
        gameObject.SetActive(false);
        _provisionalCam.SetActive(false);
    }

    private void Start()
    {
        NetworkManager.Instance.joinedRoom += PlayerJoined;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.joinedRoom -= PlayerJoined;
    }
}

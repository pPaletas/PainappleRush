using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct OwnerObject
{
    public UnityEngine.Object ownerObject;
    public bool isGameObject;
}

public class PlayerNetworkManager : MonoBehaviour
{
    [Header("Owner Components")]
    [SerializeField] private CharacterController _cc;
    [SerializeField] private CharacterMovement _movement;
    [SerializeField] private PlayerRagdoll _ragdoll;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PunchCombo _punchCombo;
    [SerializeField] private GameObject _vc;
    [SerializeField] private GameObject _cam;
    [SerializeField] private GameObject _canvas;

    private PhotonView _photonview;

    private IRemoteCallable _currentCallable;

    [Header("Player setting")]
    private List<OwnerObject> _objectsToEnableOnJoin;
    private TextMeshPro _mainNickName;

    public PhotonView Photonview { get => _photonview; }

    public void RemoteCall(IRemoteCallable callable, params object[] parameters)
    {
        _currentCallable = callable;
        _photonview.RPC("RemoteCallRPC", RpcTarget.All, parameters);
    }

    [PunRPC]
    private void RemoteCallRPC(object[] parameters)
    {
        _currentCallable.RemoteInvoke(parameters);
    }

    // Activa todos los componentes activos para el owner
    private void ActivateOwnerComponents()
    {
        _cc.enabled = true;
        _movement.enabled = true;
        _ragdoll.enabled = true;
        _input.enabled = true;
        _punchCombo.enabled = true;
        _vc.SetActive(true);
        _cam.SetActive(true);
        _canvas.SetActive(true);
    }

    private void Start()
    {
        _photonview = GetComponent<PhotonView>();
        if (_photonview.IsMine)//soy el cliente maestro
        {
            ActivateOwnerComponents();
            PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
        }
    }
}

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

public class PlayerNetworkManager : MonoBehaviourPun
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

    private EntityInfo _entityInfo;
    private IRemoteCallable[] _callableComponents;

    [Header("Player setting")]
    private List<OwnerObject> _objectsToEnableOnJoin;
    private TextMeshPro _mainNickName;

    public PhotonView Photonview { get => photonView; }

    public void RemoteCall(string callableName, params object[] parameters)
    {
        photonView.RPC("RemoteCallRPC", RpcTarget.All, callableName, (object)parameters);
    }

    [PunRPC]
    private void RemoteCallRPC(object callableName, object[] parameters)
    {
        foreach (IRemoteCallable callable in _callableComponents)
        {
            if (callable.Name == (string)callableName)
            {
                callable.RemoteInvoke(parameters);
            }
        }
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
        if (photonView.IsMine)//soy el cliente maestro
        {
            ActivateOwnerComponents();
            PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
        }
        else
        {
            this.enabled = false;
        }

        _callableComponents = GetComponentsInChildren<IRemoteCallable>(true);
    }
}
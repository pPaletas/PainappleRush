using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct OwnerObject
{
    public Object ownerObject;
    public bool isGameObject;
}

public class PlayerNetworkManager : MonoBehaviour
{
    [Header("Owner Components")]
    [SerializeField] private CharacterController _cc;
    [SerializeField] private CharacterMovement _movement;
    [SerializeField] private PlayerRagdoll _ragdoll;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private GameObject _vc;
    [SerializeField] private GameObject _cam;

    private PhotonView _photonview;

    [Header("Player setting")]
    private List<OwnerObject> _objectsToEnableOnJoin;
    private TextMeshPro _mainNickName;

    // Activa todos los componentes activos para el owner
    private void ActivateOwnerComponents()
    {
        _cc.enabled = true;
        _movement.enabled = true;
        _ragdoll.enabled = true;
        _input.enabled = true;
        _vc.SetActive(true);
        _cam.SetActive(true);
    }

    private void Start()
    {
        _photonview = GetComponent<PhotonView>();
        if (_photonview.IsMine)//soy el cliente maestro
        {
            ActivateOwnerComponents();
            PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
        }
        else
        {
            _mainNickName.text = _photonview.Owner.NickName;
        }
    }
}

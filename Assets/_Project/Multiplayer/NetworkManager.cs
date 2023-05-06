using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    public event Action joinedRoom;

    public string sceneNameToChange;
    private GameObject spawnerPlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        ConnectedToServer();
    }

    void ConnectedToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try connect to server...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server.");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("SalaUno", roomOptions, TypedLobby.Default);

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined on room.");
        base.OnJoinedRoom();
        switch (SceneManager.GetActiveScene().name)
        {
            case "Multiplayer":
                Vector3 posToSet = new Vector3(0, 1, 0);
                spawnerPlayerPrefab = PhotonNetwork.Instantiate("Characters/Player", posToSet, transform.rotation);
                joinedRoom?.Invoke();
                break;
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        sceneNameToChange = "Menu";
        ChangeRoom();
    }

    public void ChangeRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnerPlayerPrefab);
        PhotonNetwork.Disconnect();
        ChangeScene();
    }
    public void ChangeScene()
    {
        PhotonNetwork.LoadLevel(sceneNameToChange);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    
    private Transform[] spawnPoints;
    public int playerLightCount = 0;

    public int p1_id = 0; 

    void Awake()
    {
        Screen.SetResolution(1200, 800, false);
        PhotonNetwork.ConnectUsingSettings();
        PV = GetComponent<PhotonView>();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("ROOM", new RoomOptions { MaxPlayers = 2 }, null);

    public override void OnJoinedRoom()
    {
        CreatePlayer();

    }

    void CreatePlayer()
    {
        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        Vector3 pos = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;
        Quaternion rot = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].rotation; 
        

        GameObject player = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);

    }

    
}

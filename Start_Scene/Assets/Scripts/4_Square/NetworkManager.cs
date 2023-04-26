using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    
    private Transform[] spawnPoints;
    public int playerLightCount = 0;

    public int p1_id = 0; 

    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        PhotonNetwork.ConnectUsingSettings();
        PV = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;

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

        if (SceneManager.GetActiveScene().name == "3-4_Mirror")
        {
            Debug.Log("MirrorPlayer Spawn");
            GameObject player = PhotonNetwork.Instantiate("MirrorPlayer", pos, rot, 0);
            player.name = "MirrorPlayer_" + PhotonNetwork.CurrentRoom.PlayerCount;
            GameObject.Find("PlayerCam").GetComponent<InitCam>().SetPlayerCam(PhotonNetwork.CurrentRoom.PlayerCount);
        }
        else if (SceneManager.GetActiveScene().name == "4_Square")
        {
            Debug.Log("SquarePlayer Spawn");
            GameObject player = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);
        }
    }


}

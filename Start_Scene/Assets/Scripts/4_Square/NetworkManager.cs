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
        Screen.SetResolution(1920, 1080, false);
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
        // 이미 연결되어있다면 재연결 X
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

    public void SceneLoad()
    {   // 거울룸에서 마스터가 방을 우선 나가고 콜백함수 OnLeftRoom() 호출
        if (SceneManager.GetActiveScene().name == "3-4_Mirror" && PhotonNetwork.InRoom)
        {   // 한번만 LeaveRoom()이 호출되어야 에러가 안뜬다고 하여 일단 조건문 처리해놓음
            Debug.Log("leaveRoom");
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {   // 방에 나왔다면 광장 씬 로드
        
        if(SceneManager.GetActiveScene().name == "3-4_Mirror")
            SceneManager.LoadScene("4_Square");
        //SceneManager.LoadSceneAsync("4_Square");
        //PhotonNetwork.LoadLevel("4_Square");
    }

}

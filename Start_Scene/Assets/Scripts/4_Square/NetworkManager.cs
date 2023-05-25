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

    public int p1_id = 0;

    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
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
            SoundManager.instnace.PlayBGM(3);    // SceneNum = 3
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1) UIManager.instnace.RunAnims("isWait");  // 1p창에 새로운 빛을 기다리는중입니다 UI
            GameObject.Find("PlayerCam").GetComponent<InitCam>().SetPlayerMirrorCam(PhotonNetwork.CurrentRoom.PlayerCount);
        }
        else if (SceneManager.GetActiveScene().name == "4_Square")
        {            
            Debug.Log("SquarePlayer Spawn");
            GameObject player = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);
            player.name = "SquarePlayer_" + PhotonNetwork.CurrentRoom.PlayerCount;
            GameObject.Find("PlayerCam").GetComponent<InitCam>().SetPlayerSquareCam(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    public void SceneLoad()
    {   // 거울룸에서 마스터가 방을 우선 나가고 콜백함수 OnLeftRoom() 호출
        if (SceneManager.GetActiveScene().name == "3-4_Mirror" && PhotonNetwork.InRoom)
        {   // LeaveRoom()이 한번만 호출되어야 에러가 안뜸 -> 조건문으로 마스터가 방에 있을때만 실행하면 한번만 호출가능
            // 방에 있다 = 나갈 수 있다 <-> 방에 없다 = 나갈 방이 없다
            Debug.Log("leaveRoom");
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {   // 방에 나왔다면 광장 씬 로드
        if(SceneManager.GetActiveScene().name == "3-4_Mirror")
        {
            if (CinematicBar.instance.blackBars.activeSelf) CinematicBar.instance.HideBars();    // 거울에서 블랙바가 꺼지는 코드가 씹히는 경우가 존재해서 광장 스폰될때 바로 꺼줘버리자
            ScenesManager.instance.SceneNum = 4;
            SceneManager.LoadScene("4_Square");
            videoHandler.instance.SetVideo(4);
            Debug.Log("play video(4) and load Scene(4)");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class InitCam : MonoBehaviourPun
{
    CinemachineVirtualCamera vBack;
    GameObject player;
    bool InCam = false, OutCam = false;

    private void Start()
    {
        vBack = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (vBack == null) vBack = GameObject.Find("PlayerCam").GetComponent<CinemachineVirtualCamera>();

        if (InCam)
        {   // 거북이 트리거 존에 입장
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -35f, Time.deltaTime * 1.5f);
            if(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -34f)
            {
                vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -35f;
                InCam = false;
            }
        }

        if (OutCam)
        {   // 거북이 트리거 존에서 퇴장
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -20f, Time.deltaTime * 1.5f);
            if (vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z > -21f)
            {
                vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -20f;
                OutCam = false;
            }
        }
    }

    public void SetPlayerMirrorCam(int playerCnt)
    {   // 거울에서의 플레이어 개별 시네머신 설정
        if (playerCnt == 1)
        {
            player = GameObject.Find("MirrorPlayer_1");
            vBack.Follow = player.transform;
            vBack.LookAt = player.transform;
        }
        else if (playerCnt == 2)
        {
            player = GameObject.Find("MirrorPlayer_2");
            vBack.Follow = player.transform;
            vBack.LookAt = player.transform;
        }
    }

    public void SetPlayerSquareCam(int playerCnt)
    {   // 광장에서의 플레이어 개별 시네머신 설정
        if (playerCnt == 1)
        {
            player = GameObject.Find("SquarePlayer_1");
            vBack.Follow = player.transform;
        }
        else if (playerCnt == 2)
        {
            player = GameObject.Find("SquarePlayer_2");
            vBack.Follow = player.transform;
        }
    }

    void GetPhotonViewID()
    {   // isMine인 플레이어 오브젝트 가져오기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            if (item.GetPhotonView().IsMine) player = item;
        }
    }

    private void OnTriggerEnter(Collider other)
    {   // 거북이 트리거존에 닿았을 때
        if (other.CompareTag("Player_mesh"))
        {
            GetPhotonViewID();  
            if (other.gameObject.GetComponentInParent<PhotonView>().ViewID == player.GetPhotonView().ViewID)
                InCam = true;
            Debug.Log(InCam);
        }
    }

    private void OnTriggerExit(Collider other)
    {   // 거북이 트리거존에서 나갔을 때
        if (other.CompareTag("Player_mesh"))
        {
            GetPhotonViewID(); 
            if (other.gameObject.GetComponentInParent<PhotonView>().ViewID == player.GetPhotonView().ViewID)
                OutCam = true;
        }
    }
}

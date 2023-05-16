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
        {   // ???? ?? 
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -35f, Time.deltaTime * 1.5f);
            if(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -34f)
            {
                vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -35f;
                InCam = false;
            }
        }

        if (OutCam)
        {   // ???? ??
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
    {   // ?? ??? ???? ??? ??
        Debug.Log(playerCnt);
        if (playerCnt == 1)
        {
            player = GameObject.Find("MirrorPlayer_1");
            vBack.Follow = player.transform;
            vBack.LookAt = GameObject.Find("MirrorPlayer_1").transform;
        }
        else if (playerCnt == 2)
        {
            player = GameObject.Find("MirrorPlayer_2");
            vBack.Follow = player.transform;
            vBack.LookAt = GameObject.Find("MirrorPlayer_2").transform;
        }
    }

    public void SetPlayerSquareCam(int playerCnt)
    {   // ?? ?? ???? ??? ??
        Debug.Log(playerCnt);
        if (playerCnt == 1)
        {
            player = GameObject.Find("MirrorPlayer_1");
            vBack.Follow = player.transform;
        }
        else if (playerCnt == 2)
        {
            player = GameObject.Find("MirrorPlayer_2");
            vBack.Follow = player.transform;
        }
    }

    void GetPhotonViewID()
    {   // isMine? ???? ?? ?? ??
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            if (item.GetPhotonView().IsMine) player = item;
        }
    }

    private void OnTriggerEnter(Collider other)
    {   // ??? ??? ?? ???? ??? ???
        if (other.CompareTag("Player_mesh"))
        {
            GetPhotonViewID();  // isMine? ????? ????
            if (other.gameObject.GetComponentInParent<PhotonView>().ViewID == player.GetPhotonView().ViewID)
                InCam = true;
            Debug.Log(InCam);
        }
    }

    private void OnTriggerExit(Collider other)
    {   // ??? ??? ??? ???? ??? ????
        if (other.CompareTag("Player_mesh"))
        {
            GetPhotonViewID();  // isMine? ????? ????
            if (other.gameObject.GetComponentInParent<PhotonView>().ViewID == player.GetPhotonView().ViewID)
                OutCam = true;
        }
    }
}

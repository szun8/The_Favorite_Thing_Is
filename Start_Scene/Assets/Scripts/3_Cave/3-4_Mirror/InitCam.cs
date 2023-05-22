using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class InitCam : MonoBehaviourPun
{
    CinemachineVirtualCamera vBack; // 이건 거울 back, 광장 side
    CinemachineVirtualCamera vSquareBack; // 이건 광장 back 
    GameObject player;
    bool InCam = false, OutCam = false;     // 거북이 변수
    bool blueCam = false, redCam = false;   // B-2구역 변수
    bool StainCam = false;
    Vector3 statinCamPos = new(795, 20, 0);
    Quaternion stainCamRot = Quaternion.Euler(-10f, 90, 0);

    private void Start()
    {
        vBack = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (vBack == null) vBack = GameObject.Find("PlayerCam").GetComponent<CinemachineVirtualCamera>();
        if (vSquareBack == null && player != null)
        {
            vSquareBack = GameObject.Find("StainedGlassCam").GetComponent<CinemachineVirtualCamera>();
            vSquareBack.Follow = player.transform;
        }

        if (InCam)
        {   // 거북이 트리거 존에 입장
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -35f, Time.deltaTime * 1.5f);
            if(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -34.5f)
            {
                InCam = false;
            }
        }

        else if (OutCam)
        {   // 트리거 존에서 퇴장
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x, 0f, Time.deltaTime * 1.5f);

            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -20f, Time.deltaTime * 1.5f);
            if (vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z > -20.5f)
            {
                OutCam = false;
            }
        }

        else if (blueCam)
        {   // B-2 구역 파란 단상
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x, 18f, Time.deltaTime * 1.5f);

            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -25f, Time.deltaTime * 1.5f);
            if (vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -24.75f)
            {
                blueCam = false;
            }
        }

        else if (redCam)
        {   // B-2 구역 빨간 단상
            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x, -18f, Time.deltaTime * 1.5f);

            vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z =
            Mathf.Lerp(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z, -25f, Time.deltaTime * 1.5f);
            if (vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -24.75f)
            {
                redCam = false;
            }
        }

        else if (StainCam)
        {
            vSquareBack.Follow = null;

            vSquareBack.transform.position = Vector3.Lerp(vSquareBack.transform.position, statinCamPos, Time.deltaTime * 1.5f);
            vSquareBack.transform.rotation = Quaternion.Lerp(vSquareBack.transform.rotation, stainCamRot, Time.deltaTime * 1.5f);
            if(vSquareBack.transform.position.x > 794.75)
            {
                StainCam = false;
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

    GameObject coll_1p;
    private void OnTriggerEnter(Collider other)
    {   // 거북이 트리거존에 닿았을 때
        if (other.CompareTag("Player_mesh"))
        {
            Debug.Log(gameObject.name);
            GetPhotonViewID();  
            if (other.gameObject.GetComponentInParent<PhotonView>().ViewID == player.GetPhotonView().ViewID)
            {
                if (gameObject.name.Contains("TurtleCamTrigger"))
                {
                    InCam = true;
                }
                else if (gameObject.name == "B-2-RedTrigger")
                {
                    redCam = true;
                    Debug.Log("redCam On");
                }
                else if (gameObject.name == "B-2-BlueTrigger")
                {
                    blueCam = true;
                    Debug.Log("blueCam On");
                }
                else if(gameObject.name == "BackCamTrigger")
                {   // 스테인글라스 계단 밟으면 백 뷰로 전환
                    if(vSquareBack == null) vSquareBack = GameObject.Find("StainedGlassCam").GetComponent<CinemachineVirtualCamera>();

                    vSquareBack.Priority = 11;
                    vBack.Priority = 10;
                    Debug.Log("BackCam On");
                }
                else if(gameObject.name == "StainedGlassTrigger")
                {   // 스테인글라스에 더 가깝게 줌인
                    StainCam = true;
                    Debug.Log("StainedGlassTrigger");
                }
            }
            else if(gameObject.name == "B-2-RedTrigger")
            {
                if (coll_1p == null) coll_1p = other.transform.parent.gameObject;
                else if(other.transform.parent.gameObject != coll_1p)
                {
                    if(vBack.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z < -20.5)
                    {
                        OutCam = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {   // 거북이 트리거존에서 나갔을 때
        if (other.CompareTag("Player_mesh"))
        {
            GetPhotonViewID(); 
            if (other.gameObject.GetComponentInParent<PhotonView>().ViewID == player.GetPhotonView().ViewID)
            {
                if (gameObject.name.Contains("TurtleCamTrigger") || gameObject.name.Contains("B-2-"))
                {   // 2P가 파란 단상에서 빠져 나와서 앞으로 전진할 경우 다시 
                    OutCam = true;
                }
            }
        }
    }
}

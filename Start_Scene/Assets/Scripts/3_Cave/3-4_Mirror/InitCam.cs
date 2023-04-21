using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class InitCam : MonoBehaviourPun
{
    CinemachineVirtualCamera vBack;

    void Awake()
    {
        vBack = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetPlayerCam(int playerCnt)
    {
        Debug.Log(playerCnt);
        if (playerCnt == 1)
        {
            vBack.Follow = GameObject.Find("MirrorPlayer_1").transform;
            vBack.LookAt = GameObject.Find("MirrorPlayer_1").transform;
        }
        else if (playerCnt == 2)
        {
            vBack.Follow = GameObject.Find("MirrorPlayer_2").transform;
            vBack.LookAt = GameObject.Find("MirrorPlayer_2").transform;
        }
    }
}

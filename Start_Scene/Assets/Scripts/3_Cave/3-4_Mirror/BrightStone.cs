using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class BrightStone : MonoBehaviourPun, IPunObservable
{
    bool isInvoke = false;
    public bool isBright = false;

    public Material stoneMat;
    public Animator anim;

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        if (isBright)
        {   // 두명이 다 접속하여 4초가 지난후 밝아지는 애니메이션 시작
            anim.enabled = true;
            MirrorMove.isLoad = true;   // 플레이어 스크립트 속 씬 전환 함수 호출
            isBright = false;
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !isInvoke)
        {
            Invoke("InvokeStoneBright", 4f);
        }
    }

    void InvokeStoneBright()
    {
        isBright = true;
        isInvoke = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream - 데이터를 주고 받는 통로

        // 내가 데이터를 보내는 중이라면, -> master client
        if (stream.IsWriting)
        {   // 이 방안에 있는 모든 사용자에게 브로드캐스트
            stream.SendNext(isBright);
            stream.SendNext(isInvoke);
        }

        // 내가 데이터를 받는 중이라면,
        else
        {   // 순서대로 보내면 순서대로 들어옴. 근데 타입캐스팅 해주어야 함
            isBright = (bool)stream.ReceiveNext();
            isInvoke = (bool)stream.ReceiveNext();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BrightStone : MonoBehaviourPun, IPunObservable
{
    public float animTime = 5f; // Fade 애니메이션 재생 시간 (단위:초).    

    float time = 0f;            // Mathf.Lerp 메소드의 시간 값. 한번만 사용해주기에 다시 0으로 초기화 해줄 필요가 없음. 
    bool isInvoke = false, isBright = false;

    public Material stoneMat;
    Color baseColor;

    void Start()
    {
        baseColor = stoneMat.color;
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        //Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        
        if (isBright)
        {
            Debug.Log("is");
            time += Time.deltaTime / animTime;
            //baseColor = new Color(baseColor_r, baseColor_g, baseColor_b);
            stoneMat.color = Color.Lerp(stoneMat.color, baseColor * 7f, time);
            StartCoroutine(Bright());
           
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !isInvoke)
        {
            Invoke("InvokeStoneBright", 4f);
        }
    }

    IEnumerator Bright()
    {
        yield return new WaitForSeconds(2f);
        isBright = false;
        PhotonNetwork.LoadLevel("4_Square");
    }

    private void OnDestroy()
    {
        stoneMat.color = baseColor;
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
            Debug.Log(isBright);
            stream.SendNext(isBright);
            stream.SendNext(isInvoke);
            stream.SendNext(time);
            //stream.SendNext(baseColor_r);
            //stream.SendNext(baseColor_g);
            //stream.SendNext(baseColor_b);
        }

        // 내가 데이터를 받는 중이라면,
        else
        {   // 순서대로 보내면 순서대로 들어옴. 근데 타입캐스팅 해주어야 함
            isBright = (bool)stream.ReceiveNext();
            isInvoke = (bool)stream.ReceiveNext();
            time = (float)stream.ReceiveNext();
            //baseColor_r = (float)stream.ReceiveNext();
            //baseColor_g = (float)stream.ReceiveNext();
            //baseColor_b = (float)stream.ReceiveNext();
        }
    }
}

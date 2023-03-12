using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lamp : MonoBehaviourPunCallbacks
{
    Animator animator;
    GameObject stopPlayer;
    PhotonView PV;
    Rigidbody rigidbody; //전구가 검출할 플레이어의 rigidbody 접근

    CavePlayerMove cavePlayerMove;

    private RaycastHit hit;  //전구에 검출된 플레이어
    private bool isColor = false;  //전구 불 켜져 있는지 여부 

    public bool isPlayer;  // 전구가 플레이어를 검출 했는지 여부 

    void Awake()
    {
        animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        //위로 광선 쏴서 가로등 빛나도록 하기
        Debug.DrawRay(transform.position, -transform.up * 1.0f, Color.blue);
        isPlayer = Physics.Raycast(transform.position, -transform.up, out hit, 1f, LayerMask.GetMask("LightPlayer"));


        if (hit.collider != null && isPlayer && rigidbody == null)
        {
            PV.RPC("SetStopPlayer", RpcTarget.AllBuffered, hit.collider.gameObject.GetPhotonView().ViewID);

            if(CheckGetColor()) PV.RPC("FreezePlayer", RpcTarget.AllBuffered);
        }

    }

    //딸깍 당하는 플레이어 rpc로 다 전달하게 하기
    [PunRPC]
    void SetStopPlayer(int viewID)
    {
        stopPlayer = PhotonView.Find(viewID).gameObject;
        cavePlayerMove = stopPlayer.GetComponent<CavePlayerMove>();
    }

    //플레이어 공중 딸깍 동안 멈추게 하기 
    [PunRPC]
    void FreezePlayer()
    {
        if (!isColor && stopPlayer != null)
        {
            //키 입력을 아예 못받게 해버리자
            stopPlayer.GetComponent<CavePlayerMove>().enabled = false;

            //이거 하면 공중부양
            rigidbody = stopPlayer.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePosition;

            StartCoroutine(LampManager());
        }
    }

    IEnumerator LampManager()
    {
        yield return new WaitForSeconds(1f);
        PV.RPC("PaintAnimation", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(0.5f);
        PV.RPC("UnFreezePlayer", RpcTarget.AllBuffered);
        PV.RPC("PlayerRGB", RpcTarget.AllBuffered);
    }

    //가로등 켜지는 애니메이션 trigger 전달 
    [PunRPC]
    void PaintAnimation()
    {
        animator.SetTrigger("isPaint");
    }

    //플레이어 딸깍 멈췄다가 다시 움직이게 하기 
    [PunRPC]
    void UnFreezePlayer()
    {
        if (!isColor && stopPlayer != null)
        {
            stopPlayer.GetComponent<CavePlayerMove>().enabled = true;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            isColor = true;
        }
    }

    //플레이어의 색 능력 얻게 하기 
    [PunRPC]
    void PlayerRGB()
    {
        if(stopPlayer != null)
        {
            if (gameObject.CompareTag("R_item")) stopPlayer.GetComponent<CavePlayerMove>().getRed = true;

            if (gameObject.CompareTag("G_item")) stopPlayer.GetComponent<CavePlayerMove>().getGreen = true;

            if (gameObject.CompareTag("B_item")) stopPlayer.GetComponent<CavePlayerMove>().getBlue = true;

        }
    }

    // 그 색을 먹은 플레이어라면 못먹게해 
    [PunRPC]
    bool CheckGetColor()
    {
        if ((gameObject.CompareTag("R_item") && !cavePlayerMove.getRed) ||
            (gameObject.CompareTag("G_item") && !cavePlayerMove.getGreen) ||
            (gameObject.CompareTag("B_item") && !cavePlayerMove.getBlue))
            return true;
        else return false;
    }
 
}
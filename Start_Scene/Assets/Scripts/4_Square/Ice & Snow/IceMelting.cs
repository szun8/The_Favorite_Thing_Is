using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IceMelting : MonoBehaviourPun
{
    public Animator animator;
    public int isPlateup; //1이면 얼음 이 위에 있음 

    public bool isLight = false; //플레이어가 불키면 이게 true가 되고 쿵쿵이 스크립트에서 조건으로 쓴다 

    private PhotonView PV;
    private bool isPlayer = false;

    RaycastHit player;

    private bool isSendOne = false; //패킷 반복 수 줄이기 

    void Awake()=> PV = GetComponent<PhotonView>();


    void Update()
    {
        UpDownLay(); //플레이어 감지하는 레이를 발사 

        if (PhotonNetwork.InRoom) CheckLight();

    }

    void UpDownLay() //플레이어 감지 레이 발사 
    {
        if (isPlateup == 1)
        {
            Debug.DrawRay(transform.position, Vector3.down * 2.5f, Color.blue);
            isPlayer = Physics.Raycast(transform.position, Vector3.down, out player, 2.5f, LayerMask.GetMask("Player"));
        }

        else if (isPlateup == 0)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
            isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
        }
    }
    void CheckLight() //플레이어가 있는경우 r을 눌렀을 때 true
    {
        if (isPlayer && player.collider != null)
        {
            if (player.collider.gameObject.GetComponentInParent<MultiPlayerMove>().r_pressed)
            {
                if (!isLight) //단상에서 R을 한번도 안킨 상태에서 키면 
                {
                    PV.RPC("SyncMelt", RpcTarget.AllBuffered, true);
                    isSendOne = true;
                }
            }
            else if (isSendOne) //단상에서 한번 켰다가 끈상태에서 단상 레이 위에 있을 때
            {
                PV.RPC("SyncMelt", RpcTarget.AllBuffered, false);
                isSendOne = false;
            }
        }

        else if (isSendOne) //최초로 켰다가 레이에서 벗어나면 false 
        {
            PV.RPC("SyncMelt", RpcTarget.AllBuffered, false);
            isSendOne = false;
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        //isSendOne을 주는 이유는 안키고 밟았다가 그냥 Exit하면 패킷 안발사 하려고 
        if (collision.gameObject.CompareTag("Player") && isSendOne) PV.RPC("SyncMelt", RpcTarget.AllBuffered, false);
    }


    [PunRPC]
    void SyncMelt(bool value)
    {
        isLight = value;
        animator.SetBool("isMelt", value);

        AnimatorStateInfo curAnim = animator.GetCurrentAnimatorStateInfo(0); //현재 진행중인 애니메이션 상태 가져옴 
        if (curAnim.IsName("IceMelt") && curAnim.normalizedTime >= 0.9f)//애니메이션 이름이 ~~이고, 90%이상 완료된 경우 
        {
            BoxCollider boxCollider = animator.gameObject.GetComponentInChildren<BoxCollider>();
            boxCollider.isTrigger = true;
        }
    }

}

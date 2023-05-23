using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LastPlane : MonoBehaviourPun
{
    PhotonView PV;
    MultiPlayerMove playerMove;

    GameObject Player; //단상에 충돌한 플레이어 
    private bool isStop = false; //단상에 중앙에 닿으면 true가 되서 움직임 제한 변수

    //ray 검출용 
    private bool isPlayer_L, isPlayer = false;
    RaycastHit player_L;
    RaycastHit player;

    public bool[] isLight; // L, R, G, B

    public Transform center;

    private int recent_L = -1; //마지막으로 누른 버튼 확인

    private bool isIn = false;

    private Rigidbody rigid;
   

    void Awake() => PV = GetComponent<PhotonView>();


    void Update()
    {
        Ray();

        if (!isStop && Player != null)
        {
            if (isPlayer && player.collider != null)
            {
                if (player.collider.transform.parent.gameObject == Player)
                    PV.RPC("Stop", RpcTarget.AllBuffered);
            }
            else if (isPlayer_L && player_L.collider != null)
            {
                if(player_L.collider.transform.parent.gameObject == Player)
                    PV.RPC("Stop", RpcTarget.AllBuffered);
            }  
        }

        else if (isStop)
        {
            if (playerMove.l_pressed && recent_L != 0)
            {
                PV.RPC("SyncLight", RpcTarget.AllBuffered, 0);
            }

            else if (playerMove.r_pressed && recent_L != 1)
            {
                PV.RPC("SyncLight", RpcTarget.AllBuffered, 1);
            }

            else if (playerMove.g_pressed && recent_L != 2)
            {
                PV.RPC("SyncLight", RpcTarget.AllBuffered, 2);
            }

            else if (playerMove.b_pressed && recent_L != 3)
            {
                PV.RPC("SyncLight", RpcTarget.AllBuffered, 3);
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //플레이어가 한번도 안 닿은 단상일때 , 닿으면 다른놈 못들어오게
        if(collision.gameObject.CompareTag("Player") && !isIn)
        {
            Player = collision.gameObject;
            playerMove = Player.GetComponent<MultiPlayerMove>();
            isIn = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isIn)
        {
            if (Player == collision.gameObject)
            {
                playerMove.isGlass = false;
                Player = null;
                playerMove = null;
                isIn = false;
            }
        }
    }

    void Ray()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
        isPlayer_L = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player_L, 1.7f, LayerMask.GetMask("LightPlayer"));
        isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
    }

   
    [PunRPC]
    void Stop()
    {
        playerMove.isGlass = true;
        Player.GetComponent<Animator>().SetBool("isWalk", false); 

        Player.GetComponent<Transform>().position = Vector3.Lerp(Player.GetComponent<Transform>().position,
            center.position, 0.03f);

        rigid = Player.GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezeAll;//Position

        Player.GetComponent<MultiPlayerMove>().dir = new Vector3(1, 0, 0);
        

        if (Player.GetComponent<Transform>().position == center.position) isStop = true;

    }


    [PunRPC]
    void SyncLight(int value)
    {
        recent_L = value;

        for (int i = 0; i < isLight.Length; i++)
        {
            if (i == value)
            {
                isLight[i] = true;
            }
            else
            {
                isLight[i] = false;
            }
        }
    }
    
}

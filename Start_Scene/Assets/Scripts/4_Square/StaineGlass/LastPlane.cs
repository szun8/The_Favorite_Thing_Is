using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LastPlane : MonoBehaviourPun
{
    PhotonView PV;
    MultiPlayerMove playerMove;
    Animator animator;

    GameObject PC; //단상에 충돌한 플레이어

    GameObject Player;
    public bool isStop = false; //단상에 중앙에 닿으면 true가 되서 움직임 제한 변수

    //ray 검출용 
    private bool isPlayer_L, isPlayer = false;
    RaycastHit player_L;
    RaycastHit player;

    public bool[] isLight; // L, R, G, B

    public Transform center;

    public int recent_L = -1; //마지막으로 누른 버튼 확인

    private bool isIn = false;

    private Rigidbody rigid;

    private bool isSendOne = false;
   

    void Awake() => PV = GetComponent<PhotonView>();


    void Update()
    {
        Ray();

        if (!isStop && PC != null)
        {
            PV.RPC("SyncPC", RpcTarget.AllBuffered);
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

            else if(!playerMove.l_pressed && !playerMove.r_pressed && !playerMove.g_pressed && !playerMove.b_pressed && !isSendOne)
            {

            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //플레이어가 한번도 안 닿은 단상일때 , 닿으면 다른놈 못들어오게
        if(collision.gameObject.CompareTag("Player") && !isIn)
        {
            PC = collision.gameObject;
            //PV.RPC("SyncPC", RpcTarget.AllBuffered);
            isIn = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isIn)
        {
            if (PC == collision.gameObject)
            {
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
        //1. 플레이어 wasd dir 입력 막아버려  z_free 는 true인 상태 
        playerMove.isGlass = true;

        playerMove.dir = new Vector3(1, 0, 0);

        //2. 멈춰랏 !  // 애니메이션 멈추기
        PV.RPC("SyncAnim", RpcTarget.AllBuffered, false); //animator.SetBool("isWalk", false); 
        
        //2. 움직임 제한 
        rigid = Player.GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezeAll;//Position

        //3. 글라스 바라보게 하고 위치 스르륵 가게
        
        Player.GetComponent<Transform>().position = Vector3.Lerp(Player.GetComponent<Transform>().position,
        center.position, 0.05f);

        //4. 어느정도 스르륵 되면 stop 성공
        if (Vector3.Distance(Player.transform.position, center.position) < 0.001f)
        {
            playerMove.dir = Vector3.zero;
            isStop = true;
        } 

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

    [PunRPC]
    void SyncNoL()
    {
        for(int i = 0; i < isLight.Length; i++)
        {
            isLight[i] = false;
        }
        isSendOne = true;
    } 
    
    [PunRPC]
    void SyncAnim(bool value) 
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        if(currentState.IsName("Idle") == false)
        {
            animator.SetBool("isWalk", false);
            animator.Play("Idle");
        }    
    }

    [PunRPC]
    void SyncPC()
    {
        if (PC != null)
        {
            Player = PC;
            playerMove = Player.GetComponent<MultiPlayerMove>();
            animator = Player.GetComponent<Animator>();
        }
    }

}

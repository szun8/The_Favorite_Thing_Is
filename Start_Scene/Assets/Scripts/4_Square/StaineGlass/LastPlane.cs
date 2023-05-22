using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LastPlane : MonoBehaviourPun
{
    PhotonView PV;
    MultiPlayerMove playerMove;

    private bool isStop = false; //단상에 중앙에 닿으면 true가 되서 움직임 제한 변수

    //ray 검출용 
    private bool isPlayer = false;
    RaycastHit player;

    public bool[] isLight; // L, R, G, B

    private int recent_L = -1; //마지막으로 누른 버튼 확인 

    void Awake() => PV = GetComponent<PhotonView>();


    void Update()
    {
        Ray();

        if (!isStop && isPlayer && player.collider != null) PV.RPC("Stop", RpcTarget.AllBuffered);

        if (isStop)
        {
            if(playerMove.l_pressed && recent_L != 0)
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

    void Ray()
    {
        Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1.7f, Color.blue);
        isPlayer = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out player, 1.7f, LayerMask.GetMask("Player"));
    }

    [PunRPC]
    void Stop()
    {
        isStop = true;
        playerMove = player.collider.gameObject.GetComponentInParent<MultiPlayerMove>();
        playerMove.dir = new Vector3(1, 0, 0);
        player.collider.transform.parent.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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

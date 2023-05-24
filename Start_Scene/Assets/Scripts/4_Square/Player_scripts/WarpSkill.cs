using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WarpSkill : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    MultiPlayerMove playerMove;
    ReverseGravity reverseGravity;
    Transform playerZ;
    Rigidbody rigid;

    private GameObject diePos;

    private bool isSendOne = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        reverseGravity = GetComponent<ReverseGravity>();
        playerMove = GetComponent<MultiPlayerMove>();
        playerZ = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //글라쓰 단상 씬이 아니고, 플레이어가 wasd 움직일수 없고, Z축이 벗어나게 되면 Z축 0으로 맞추기 
        if (!playerMove.isGlass && !gameObject.GetComponent<MultiPlayerMove>().z_free && (playerZ.position.z <= -0.5 || playerZ.position.z >= 0.5))
        {
            playerZ.position = new Vector3(playerZ.position.x, playerZ.position.y, 0);
        }

        //단상 밟기 + 다 얼려있지 않으면 얼려라 
        else if (playerMove.isGlass && (rigid.constraints & RigidbodyConstraints.FreezeAll) != 0)
        {
            rigid.constraints = RigidbodyConstraints.FreezeAll;
        }

        //z_free + freeze Z 되어 있으면 z축 제한 자유롭게 
        else if (gameObject.GetComponent<MultiPlayerMove>().z_free && (rigid.constraints & RigidbodyConstraints.FreezePositionZ) !=0)
        {
            rigid.constraints &= ~RigidbodyConstraints.FreezePositionZ; // 자유로운 z~ 
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine)
        {
            if (other.CompareTag("WarpUp"))
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -179f);
                gameObject.transform.position = other.gameObject.GetComponent<Warp>().outWarp.position;//gameObject.transform.position + new Vector3(0, -4, 0);
                reverseGravity.photonView.RPC("SyncisReversed", RpcTarget.AllBuffered);

            }

            else if (other.CompareTag("WarpDown"))
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 1f);
                gameObject.transform.position = other.gameObject.GetComponent<Warp>().outWarp.position;
                reverseGravity.photonView.RPC("SyncisReversed", RpcTarget.AllBuffered);
            }

            if (other.CompareTag("Dead")) gameObject.transform.position = diePos.transform.position;

            if (other.CompareTag("SavePoint")) diePos = other.gameObject;
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WarpSkill : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    ReverseGravity reverseGravity;
    Transform playerZ;
    Rigidbody rigid;

    private GameObject diePos;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        reverseGravity = GetComponent<ReverseGravity>();
        playerZ = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!gameObject.GetComponent<MultiPlayerMove>().z_free && (playerZ.position.z <= -0.5 || playerZ.position.z >= 0.5))
        {
            playerZ.position = new Vector3(playerZ.position.x, playerZ.position.y, 0);
            Debug.Log(gameObject.GetComponent<MultiPlayerMove>().z_free);
        }

        //freeze Z가 되어 있으면 == 1이다~ 
        else if (gameObject.GetComponent<MultiPlayerMove>().z_free && (rigid.constraints & RigidbodyConstraints.FreezePositionZ) !=0)
        {
            rigid.constraints &= ~RigidbodyConstraints.FreezePositionZ;
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

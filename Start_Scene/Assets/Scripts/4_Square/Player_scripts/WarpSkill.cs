using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WarpSkill : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    ReverseGravity reverseGravity;
    Transform playerZ;

    private GameObject diePos;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        reverseGravity = GetComponent<ReverseGravity>();
        playerZ = GetComponent<Transform>();
    }

    private void Update()
    {
        if (playerZ.position.z <= -0.5 || playerZ.position.z >= 0.5)
            playerZ.position = new Vector3(playerZ.position.x, playerZ.position.y, 0);
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

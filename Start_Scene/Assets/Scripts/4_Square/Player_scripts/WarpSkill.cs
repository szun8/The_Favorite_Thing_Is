using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WarpSkill : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    ReverseGravity reverseGravity;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        reverseGravity = GetComponent<ReverseGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine)
        {
            if (other.gameObject.name == "Warp")
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -179f);
                gameObject.transform.position = gameObject.transform.position + new Vector3(0, -3, 0);
                reverseGravity.photonView.RPC("SyncisReversed", RpcTarget.AllBuffered);
            }

            else if (other.gameObject.name == "WarpUnder")
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 1f);
                gameObject.transform.position = gameObject.transform.position + new Vector3(0, 3, 0);
                reverseGravity.photonView.RPC("SyncisReversed", RpcTarget.AllBuffered);
            }
        }
        
    }
}

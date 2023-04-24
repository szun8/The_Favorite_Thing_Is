using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Koong : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    public bool dieReady = false;


    void Awake() => PV = GetComponent<PhotonView>();


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            PV.RPC("SyncDie", RpcTarget.AllBuffered, true);
            Debug.Log("Meet");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            PV.RPC("SyncDie", RpcTarget.AllBuffered, false);
            Debug.Log("nononono ");
        }
    }


    [PunRPC]
    void SyncDie(bool value) => dieReady = value;
}

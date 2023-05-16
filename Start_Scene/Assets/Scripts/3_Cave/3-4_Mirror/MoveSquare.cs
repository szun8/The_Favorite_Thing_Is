using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveSquare : MonoBehaviourPun
{
    PhotonView PV;
    public bool goSqure = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (goSqure)
        {
            GetComponent<MirrorMove>().enabled = false;
            PhotonNetwork.LoadLevel("4_Square");
            PV.RPC("Sync", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void Sync() => goSqure = false;
}

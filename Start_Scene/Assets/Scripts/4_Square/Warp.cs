using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Warp : MonoBehaviourPunCallbacks
{
    PhotonView PV;


    void Awake() => PV = GetComponent<PhotonView>();

    
    private void OnTriggerEnter(Collider other)
    {
        //Trigger인 경우, 프리팹 하위인 collider 접근은 부모를 안찾아주,, 잘 하자 ,,,
        if (other.gameObject.CompareTag("Player_mesh")) PV.RPC("DestroyWarp", RpcTarget.AllBuffered);

    }

    [PunRPC]
    void DestroyWarp()
    {
        Destroy(gameObject);
    }
}
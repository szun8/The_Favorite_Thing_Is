using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SyncGround : MonoBehaviourPun
{
    PhotonView PV;
    GameObject Player;
    MultiPlayerMove playerMove;
    void Awake() => PV = GetComponent<PhotonView>();

    
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //플레이어가 한번도 안 닿은 단상일때 , 닿으면 다른놈 못들어오게
        if(collision.gameObject.CompareTag("Player"))
        {
            Player = collision.gameObject;
            playerMove = Player.GetComponent<MultiPlayerMove>();
            
            playerMove.z_free = true;
        }
    }
}

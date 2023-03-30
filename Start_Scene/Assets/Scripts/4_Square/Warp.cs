using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Warp : MonoBehaviourPunCallbacks
{
    ReverseGravity reverseGravity;
    PhotonView PV;
    GameObject Player;
    Vector3 PlayerPos;


    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    private void OnTriggerEnter(Collider other)
    {
        

        //Trigger인 경우, 프리팹 하위인 collider 접근은 부모를 안찾아주,, 잘 하자 ,,,
        
        if (other.gameObject.CompareTag("Player_mesh"))
        {
            
            Player = other.transform.parent.gameObject;
            PV.RPC("ChangeGravity", RpcTarget.AllBuffered);

            if (PlayerPos != null)
            {
                Player.transform.position = new Vector3(PlayerPos.x, PlayerPos.y - 3, PlayerPos.z);
                Debug.Log("player warp success!!");
            }

        }
    }

    [PunRPC]
    void ChangeGravity()
    {
        if(Player != null)
        {
            Debug.Log("player not null ");
            reverseGravity = Player.GetComponent<ReverseGravity>();

            if(reverseGravity != null)
            {
                Debug.Log("reverse!!!! ");
                reverseGravity.isReversed = true;

                Player.transform.rotation = Quaternion.Euler(0, 0, -179f); //뒤집기 플레이어

                PlayerPos = Player.transform.position;

                reverseGravity.photonView.RPC("SyncisReversed", RpcTarget.AllBuffered);
            }
 
        }

        else
        {
            Debug.Log("Player is null");
        }
        
    }

    


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WarpUnder : MonoBehaviourPunCallbacks
{
    ReverseGravity reverseGravity;
    PhotonView PV;
    GameObject Player;
    //Vector3 PlayerPos;


    void Awake() => PV = GetComponent<PhotonView>();


    private void OnTriggerEnter(Collider other)
    {


        //Trigger인 경우, 프리팹 하위인 collider 접근은 부모를 안찾아주,, 잘 하자 ,,,

        if (other.gameObject.CompareTag("Player_mesh"))
        {

            Player = other.transform.parent.gameObject;
            PV.RPC("ChangeGravity", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);//이 번호는 RPC 호출 시 해당 RPC 함수를 호출한 클라이언트의 ID를 식별하는 데 사용될 수 있습니다);
            Player.transform.position = (Player.transform.position + new Vector3(0, 3, 0));

            /*if (PlayerPos != null)
            {
                Player.transform.position = new Vector3(PlayerPos.x, PlayerPos.y - 3, PlayerPos.z);
            }*/

        }
    }

    [PunRPC]
    void ChangeGravity(int id)
    {
        if (Player != null && Player.GetComponent<PhotonView>().ViewID == id)
        {
            reverseGravity = Player.GetComponent<ReverseGravity>();
            PhotonView pv = reverseGravity.GetComponent<PhotonView>();

            if (reverseGravity != null)
            {

                Player.transform.rotation = Quaternion.Euler(0, 0, 1f); //뒤집기 플레이어

                //PlayerPos = Player.transform.position;

                pv.RPC("SyncisReversed", RpcTarget.AllBuffered);
            }

        }

    }
}
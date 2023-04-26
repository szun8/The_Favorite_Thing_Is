using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KindPlate : MonoBehaviourPunCallbacks
{

    public bool redObj = false;
    public bool greenObj = false;

    PhotonView PV;
    RaycastHit player;
    bool isPlayer = false;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        isPlayer = Physics.Raycast(transform.position, transform.up, out player, 0.5f);

        if (PhotonNetwork.InRoom) //플레이어가 방안에 있는지 
        {
            if (isPlayer) PV.RPC("CheckRG", RpcTarget.AllBuffered);  //플레이어가 위에 있다면 빛내는지 검출하자 

        }
    }

    [PunRPC]
    void CheckRG() //플레이어 빨강이 눌렸는지에 따른 처리 
    {
        if (player.collider != null) //플레이어가 위에서 검출이되면 
        {
            if (player.collider.GetComponentInParent<MultiPlayerMove>().r_pressed)
            {
                redObj = true;
                greenObj = false;
            }
            else if(player.collider.GetComponentInParent<MultiPlayerMove>().g_pressed)
            {

                redObj = false;
                greenObj = true;

            }
            else
            {

                redObj = false;
                greenObj = false;
            }

        }

    }
}
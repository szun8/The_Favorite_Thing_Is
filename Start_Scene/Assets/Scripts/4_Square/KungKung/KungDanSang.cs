using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KungDanSang : MonoBehaviourPunCallbacks
{
    public int isKungup;
    public GameObject Kung;

    private PhotonView PV;
    private bool isKung = false;

    RaycastHit monster;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        UpDownLay();

        if (PhotonNetwork.InRoom) //플레이어가 방안에 있는지 
        {
            if (isKung) PV.RPC("KungDie", RpcTarget.AllBuffered);  //플레이어가 위에 있다면 빛내는지 검출하자 

        }
    }

    void UpDownLay()
    {
        if (isKungup == 1)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector3.up * 1f, Color.blue);
            isKung = Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.up, out monster, 1f, LayerMask.GetMask("DownKung"));
        }

        else if (isKungup == 0)
        {
            Debug.DrawRay(transform.position, Vector3.down * 2f, Color.blue);
            isKung = Physics.Raycast(transform.position, Vector3.down, out monster, 2f, LayerMask.GetMask("UpKung"));
        }
    }
    [PunRPC]
    void KungDie()
    {
        if(monster.collider != null && monster.collider.gameObject == Kung)
        {
            Kung.GetComponent<Renderer>().material.SetFloat("_SplitValue", Mathf.Lerp(1, 0 , 0.5f));
        }
    }
}

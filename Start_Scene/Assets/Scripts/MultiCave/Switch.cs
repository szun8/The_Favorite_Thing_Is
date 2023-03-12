using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//얘 점프해서 밟으면 계속 들썩거림 . 
public class Switch : MonoBehaviourPunCallbacks, IPunObservable
{
    PhotonView PV;
    private bool isPressed = false;
    public float pressedDistance = 0.2f;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //통신을 보내는 
        if (stream.IsWriting)
        {
            stream.SendNext(isPressed);
            stream.SendNext(transform.position);
        }
        //클론이 통신을 받는 
        else
        {
            isPressed = (bool)stream.ReceiveNext();
            Vector3 position = (Vector3)stream.ReceiveNext();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPressed = true;
            PV.RPC("SyncIsPressed", RpcTarget.AllBuffered, isPressed);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPressed = false;
            PV.RPC("SyncIsPressed", RpcTarget.AllBuffered, isPressed);
        }
    }

    [PunRPC]
    void SyncIsPressed(bool isPressed)
    {
        this.isPressed = isPressed;
        Vector3 position = transform.position;
        if (isPressed) position.y = position.y - pressedDistance;
        else position.y = position.y + pressedDistance;
        transform.position = position;
    }
   

}

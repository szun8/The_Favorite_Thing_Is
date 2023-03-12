using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class R_Switch : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    
    private bool isPressed = false;
    public float pressedDistance = 0.2f;

    private Vector3 startPos, endPos;
    


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        startPos = transform.position;
        endPos = startPos - (Vector3.up * pressedDistance);
        
    }

    private void Update()
    {
        bool isPlayer = Physics.BoxCast(transform.position, Vector3.one,
            Vector3.up, out RaycastHit hit, Quaternion.identity, 1f, LayerMask.GetMask("Player"));

        if (isPlayer)
        {
            PV.RPC(nameof(SyncIsPressed), RpcTarget.AllBuffered, true);
            PV.RPC(nameof(SyncSwitchPosition), RpcTarget.AllBuffered, endPos);
        }

        else
        {
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                PV.RPC(nameof(SyncIsPressed), RpcTarget.AllBuffered, false);
                PV.RPC(nameof(SyncSwitchPosition), RpcTarget.AllBuffered, startPos);
            }
                
        }
        
        
    }

    [PunRPC]
    void SyncIsPressed(bool isPressed)=> this.isPressed = isPressed;

    [PunRPC]
    void SyncSwitchPosition(Vector3 pos)=> transform.position = pos;
}

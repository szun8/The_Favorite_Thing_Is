using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    float hAxis;
    float vAxis;

    Vector3 moveVec;

    void Update()
    {   
        if (PV.IsMine)
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");

            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
            //윗키 오른쪽키 대각선 가는데 대각선이 더 길어 ,,, 그러니 모든 방향 같은 값 갖도록 normalized

            transform.position += moveVec * 7 * Time.deltaTime;
            transform.LookAt(moveVec + transform.position);// 우리가 나아가는 방향으로 보세요 그러니까 회전 됨 
        }
    }

    
}

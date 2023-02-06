using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviour
{
    Rigidbody rigid;

    public PhotonView PV;

    private Vector3 dir = Vector3.zero;// 캐릭터가 나아갈, 바라볼 방향 
    public int JumpForce; //점프력
    public float rotSpeed = 8f; //방향키 반대이동시 몸의 회전 속도 
    public float speed = 8f; //캐릭터 속도

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (PV.IsMine)
        {
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.z = Input.GetAxisRaw("Vertical");
            dir.Normalize(); //대각선 빨라지는거 방지위한 정규화
        }
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            //키 입력이 들어왔으면 ~
            if (dir != Vector3.zero)
            {
                //바라보는 방향 부호 != 가고자할 방향 부호
                if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
                {
                    transform.Rotate(0, 1, 0);
                }
                transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
            }
            rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
        }
    }
}
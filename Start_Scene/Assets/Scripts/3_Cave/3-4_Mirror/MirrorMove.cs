using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MirrorMove : MonoBehaviourPunCallbacks
{
    bool isJump = false; // 바닥 충돌

    Rigidbody rigid;
    Animator animator;
    SkinnedMeshRenderer mesh;

    public Material LightMaterial;  //L머티리얼
    public PhotonView PV;

    // 이동 관련 변수 
    private Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향
    public int JumpForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    // 플레이어 따라다니는 전등
    public Light spotLight;  //spot light
    public GameObject maskLight; // 발광 구
    public bool lightOn = false;
    //플레이어의 l 눌려져있는 것 확인
    public bool l_pressed = false;

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;

        dir.x = Input.GetAxisRaw("Horizontal");
        dir.z = Input.GetAxisRaw("Vertical");

        if (isJump && Input.GetKeyDown("space"))
        {
            isJump = false;
            PV.RPC("SyncMirrorJump", RpcTarget.AllBuffered);
            rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown("l"))
        {
            PV.RPC("SyncMirrorLightPressed", RpcTarget.AllBuffered, 0, true);
            PV.RPC("MirrorLightOn", RpcTarget.AllBuffered);
        }
        if (Input.GetKeyUp("l"))
        {
            PV.RPC("SyncMirrorLightPressed", RpcTarget.AllBuffered, 0, false);
        }
        //모든 버튼이 안눌려 있어야만 빛이 꺼집니다 . 
        if (!l_pressed) PV.RPC("MirrorLightOff", RpcTarget.AllBuffered);
    }

    void FixedUpdate()
    {
        if (!PV.IsMine) return;

        if (dir != Vector3.zero)
        {
            //바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else
        {
            PV.RPC("SyncMirrorAnimation", RpcTarget.AllBuffered, "isWalk", false);
        }
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = true;
        }
    }

    [PunRPC]
    void MirrorLightOn()
    {
        if (l_pressed)
        {
            spotLight.color = Color.white;
            gameObject.transform.GetChild(1).gameObject.layer = 9;
            lightOn = true;
            maskLight.GetComponent<Renderer>().material.SetColor("_Emission", new Color(96f, 93f, 0, 120f));


            PV.RPC("MirrorLightPower", RpcTarget.AllBuffered);

            //maskLight 다른 색됫다가 돌아올 색 적용 코드 필요 
            maskLight.SetActive(lightOn);
        }

    }

    [PunRPC]
    void MirrorLightOff() //L,R,G,B 모든 색을 끄는 용도,, 무조건 디폴트 색으로 돌아가게 해줌 .
    {
        lightOn = false;
        gameObject.transform.GetChild(1).gameObject.layer = 6;

        PV.RPC("MirrorLightPower", RpcTarget.AllBuffered);
        maskLight.SetActive(lightOn);
    }

    [PunRPC]
    void MirrorLightPower()
    {
        if (lightOn) // L 버튼으로 발광 하면 ~ 
        {
            spotLight.intensity = 25f;
        }

        else 
        {
            spotLight.intensity = 0;
        }

    }

    [PunRPC]
    void SyncMirrorAnimation(string animation, bool value) => animator.SetBool(animation, value);

    [PunRPC]
    void SyncMirrorJump()
    {
        animator.SetBool("isWalk", false);
        animator.SetTrigger("isJump");
    }

    [PunRPC]
    void SyncMirrorLightPressed(int rgb, bool value) //Rgb가 눌렸는지 상태 변수를 동기화 
    {
        switch (rgb)
        {
            case 0:
                l_pressed = value;
                break;
        }
    }
}
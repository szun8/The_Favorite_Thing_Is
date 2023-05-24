using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class MirrorMove : MonoBehaviourPunCallbacks
{
    bool isJump = false; // 바닥 충돌

    Rigidbody rigid;
    Animator animator;
    NetworkManager networkManager;

    public Material LightMaterial;  //L머티리얼
    public PhotonView PV;
    public static bool isLoad = false;

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
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;
        //모든 버튼이 안눌려 있어야만 빛이 꺼집니다 . 
        if (!l_pressed) PV.RPC("MirrorLightOff", RpcTarget.AllBuffered);

        if (isLoad)
        {   // 거울스톤이 밝아지며 씬전환 함수 호출되는 곳
            Debug.Log("SceneLoad to 4");
            SceneLoad();
            isLoad = false;
        }
    }

    public void OnMove(InputAction.CallbackContext state)
    {
        if (state.performed)
        {
            dir = state.ReadValue<Vector3>();
        }
    }

    public void OnJump(InputAction.CallbackContext state)
    {
        if (PV.IsMine)
        {
            if (state.performed && isJump)
            {
                isJump = false;
                PV.RPC("SyncMirrorJump", RpcTarget.AllBuffered);
                rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            }
        } 
    }

    public void OnLight(InputAction.CallbackContext state)
    {
        if (PV.IsMine)
        {
            if (state.performed)
            {
                PV.RPC("SyncMirrorLightPressed", RpcTarget.AllBuffered, 0, true);
                PV.RPC("MirrorLightOn", RpcTarget.AllBuffered);
            }
            else if (state.canceled)
            {
                PV.RPC("SyncMirrorLightPressed", RpcTarget.AllBuffered, 0, false);
            }
        }
        
    }

    public void SceneLoad()
    {
        StartCoroutine(SceneLoadCoroutine());
    }

    IEnumerator SceneLoadCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.instnace.stopOut = false;
        SoundManager.instnace.VolumeOutBGM();   // 거울룸 배경음악 stop
        yield return new WaitForSeconds(1.5f);

        if (PhotonNetwork.IsMasterClient)
        {
            networkManager.SceneLoad();
        }
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

            if (!StopToWall())
            {
                rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
                PV.RPC("SyncMirrorAnimation", RpcTarget.AllBuffered, "isWalk", true);
            }
        }
        else
        {
            PV.RPC("SyncMirrorAnimation", RpcTarget.AllBuffered, "isWalk", false);
        }
    }

    bool StopToWall()
    {   // 벽 뚫는 버그 막아버리기!
        RaycastHit[] hits;
        Debug.DrawRay(transform.position+Vector3.up, transform.forward * 1, Color.green);
        hits = Physics.RaycastAll(transform.position+Vector3.up, transform.forward, 1);
        if (hits.Length > 0) return true;
        else return false;
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
            maskLight.GetComponent<Renderer>().material.SetColor("_Emission", new Color(20f, 20f, 20f, 120f));

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

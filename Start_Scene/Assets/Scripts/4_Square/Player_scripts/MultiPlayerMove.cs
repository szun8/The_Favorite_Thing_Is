using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiPlayerMove : MonoBehaviourPunCallbacks
{
    //불러올 스크립트 
    NetworkManager networkManager;
    ReverseGravity reverseGravity;
    WaitingWall wall;   //벽에 플레이어 collision 수를 넘기기 위해서 불러옴

    //컴포넌트 
    SkinnedMeshRenderer mesh;
    public Material[] materials; 
    Rigidbody rigid;
    Animator animator;

    public PhotonView PV;
    

    //이동관련 변수 
    private Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향
    public int JumpForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    // 플레이어 따라다니는 전등
    public Light spotLight;  //spot light
    public GameObject maskLight; // 발광 구

    private bool isGround = false;
    private bool isBridge = false;
    private bool isStone = false; //바닥에 충돌되어 있을 때도 점프 가능하게 하기 위함 
    //private bool isMirrorJump = false;

    //상호작용 
    //private RaycastHit RGBitem;   //일단 남겨두자 플레이어가 바라보는 아이,, 뭐 ,,,, 
    //private bool isItem = false;

    //빛 관련 변수
    public bool lightOn = false;
    //private bool colorLightOn = false; //RGB 발광 위한 bool

    //플레이어가 발광 가로등과 상호작용 해서 능력얻으면 true 
    //public bool getRed = false;
    //public bool getGreen = false;
    //public bool getBlue = false;


    private Material defaultMaterial; //기본 발광 마테리얼 
    private Material lastMaterial;


    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        reverseGravity = GetComponent<ReverseGravity>();
        wall = GameObject.Find("WaitWall").GetComponent<WaitingWall>();


        defaultMaterial = materials[0];
        lastMaterial = defaultMaterial;


        if (PV.IsMine)
        {
            //PV.RPC("StartLight", RpcTarget.AllBuffered);
            Camera.main.GetComponent<CameraMove>().player = gameObject;
            Camera.main.GetComponent<CameraMove>().dist = 22;
            Camera.main.GetComponent<CameraMove>().height = 0.8f; //재인이의 희망은 0.2였다 
            //maskLight.SetActive(false);
        }
        materials =  mesh.materials;

    }


    void Update()
    {
        if (PV.IsMine)
        {
            dir.x = Input.GetAxisRaw("Horizontal");

            //내 밑으로 광선을 쏴서 바닥 레이어랑 닿으면 점프시키기 
            Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), Vector2.down * 1.3f, Color.blue);
            //1:쏘는 위치 2:쏘는 방향 3:해당 레이어 
            isGround = Physics.Raycast(transform.position+new Vector3(0,0.5f,0), Vector2.down, 1.3f, LayerMask.GetMask("Ground"));
            isBridge = Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector2.down, 1.3f, LayerMask.GetMask("Bridge"));

            //아이템이랑 스프링은 잘 모르겠다 나중에
            //isItem = Physics.Raycast(transform.position, transform.forward, out RGBitem, 1.1f, LayerMask.GetMask("Item") );

            //내 앞으로 광선을 쏴서 물체를 검출해보자 
            Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.forward * 1.5f, Color.red);

            if (Input.GetKeyDown("space"))
            {
                //if (isItem && RGBitem.collider != null) GetItem(RGBitem);
                //땅이거나 다리를 밟으면
                if (isGround || isBridge)
                {
                    //뒤집힌 중력인 경우 
                    if (reverseGravity.isReversed)
                    {
                        Debug.Log("jump");
                        rigid.AddForce(Vector2.down * (JumpForce + 3), ForceMode.Impulse);
                    }

                    else
                    {
                        rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
                    }

                }
 
            }

            if (Input.GetKey("l")) PV.RPC("LightOn", RpcTarget.AllBuffered);
            
            if (Input.GetKeyUp("l")) PV.RPC("LightOff", RpcTarget.AllBuffered);
           
        }
        
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            //키 입력이 들어왔으면 ~
            if (dir != Vector3.zero)
            {
                animator.SetBool("isWalk", true);
                PV.RPC("SyncAnimation", RpcTarget.AllBuffered,"isWalk", true);
                //바라보는 방향 부호 != 가고자할 방향 부호
                if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x))
                {
                    transform.Rotate(0, 1, 0);
                }
                if (PV.ViewID != networkManager.p1_id && !reverseGravity.isReversed)
                    transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
                else
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.down), Time.deltaTime * rotSpeed);
            }
            else
            {
                animator.SetBool("isWalk", false);
                PV.RPC("SyncAnimation", RpcTarget.AllBuffered, "isWalk", false);
            }
            
            rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
            
            
        }
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "WaitWall") wall.collisionCount++;

        if (collision.gameObject.CompareTag("Ground")) isStone = true;

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "WaitWall") wall.collisionCount--;

        if (collision.gameObject.CompareTag("Ground")) isStone = false;
    }


    [PunRPC]
    void LightOn()
    {
        lightOn = true;
        PV.RPC("LightPower", RpcTarget.AllBuffered);
        maskLight.SetActive(lightOn);

    }

    [PunRPC]
    void LightOff()
    {
        lightOn = false;
        PV.RPC("LightPower", RpcTarget.AllBuffered);
        maskLight.SetActive(lightOn);
    }

    [PunRPC]
    void LightPower()   //발광의 세기 조절 
    {
        if (lightOn)
        {
            //emission color의 밝기 1.5배 증가 시키기 
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f) * 1.8f);
            spotLight.intensity = 25f;

            networkManager.playerLightCount++; //waitingWall에서 불킨 인원 수 보내주려고
        }

        else
        {
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f));
            spotLight.intensity = 0;
        }

    }

    [PunRPC]
    void SyncAnimation(string animation, bool value)
    {
        animator.SetBool(animation, value);
    }

}

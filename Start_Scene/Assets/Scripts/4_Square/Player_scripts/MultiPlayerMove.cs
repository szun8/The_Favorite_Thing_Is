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
    Material[] PlayerMaterials; //머티리얼 직접 접근 불가로 저장할 공간
    public Material[] LightMaterials;  //L, R, G, B 머티리얼

  

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
    private bool rgb_lightOn = false; //RGB 발광 위한 bool

    //플레이어가 발광 가로등과 상호작용 해서 능력얻으면 true 
    public bool getRed = false;
    public bool getGreen = true;
    public bool getBlue = true;

    //플레이어의 lrgb 눌려져있는 것 확인
    public bool l_pressed = false;
    public bool r_pressed = false;
    public bool g_pressed = false;
    public bool b_pressed = false;



    private Material defaultMaterial; //기본 발광 마테리얼
                                      

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();


        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        reverseGravity = GetComponent<ReverseGravity>();
        //wall = GameObject.Find("WaitWall").GetComponent<WaitingWall>();

        

        defaultMaterial = LightMaterials[0]; //기본 머티리얼은 흰색 body
        

        //플레이어의 eye body 머티리얼 정보 저장 .
        PlayerMaterials = mesh.materials;

        if (PV.IsMine)
        {
            Camera.main.GetComponent<CameraMove>().player = gameObject;
            Camera.main.GetComponent<CameraMove>().dist = 10;//18;
            Camera.main.GetComponent<CameraMove>().height = 0.8f; //재인이의 희망은 0.2였다
            
        }

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
                //땅이거나 다리를 밟으면 + 바닥과 충돌해 있는 경우 
                if (isGround || isBridge || isStone)
                {
                    //뒤집힌 중력인 경우 
                    if (reverseGravity.isReversed)
                    {
                        Debug.Log("jump");
                        rigid.AddForce(Vector2.down * JumpForce/1.2f, ForceMode.Impulse);
                    }
                    else //제대로 된 중력 
                    {
                        rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
                    }

                }
 
            }

            if (Input.GetKey("l"))
            {
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 0, true);
                PV.RPC("LightOn", RpcTarget.AllBuffered);
            }

            //RPC 함수는 최대 인자 2개만 전송가능 Color는 못넘긴다 ~ 벡터로 변환 해줘야 한다 ~ 
            if (Input.GetKey("r") && getRed)
            {
                PV.RPC("RGB_ON", RpcTarget.AllBuffered, 1, new Vector3(1, 0, 0));
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 1, true);
            }


            if (Input.GetKey("g") && getGreen)
            {
                PV.RPC("RGB_ON", RpcTarget.AllBuffered, 2, new Vector3(0, 1, 0));
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 2, true);
            }

            if (Input.GetKey("b") && getBlue)
            {
                PV.RPC("RGB_ON", RpcTarget.AllBuffered, 3, new Vector3(0, 0, 1));
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 3, true);
            }

            if (Input.GetKeyUp("l"))
            {
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 0, false);
            }
            if (Input.GetKeyUp("r"))
            {
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 1, false);
            }
            if (Input.GetKeyUp("g"))
            {
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 2, false);
            }
            if (Input.GetKeyUp("b"))
            {
                PV.RPC("SyncLightPressed", RpcTarget.AllBuffered, 3, false);
            }

            //모든 버튼이 안눌려 있어야만 빛이 꺼집니다 . 
            if (!r_pressed && !g_pressed && !b_pressed && !l_pressed) PV.RPC("LightOff", RpcTarget.AllBuffered);


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
        if (l_pressed)
        {
            spotLight.color = Color.white;
            gameObject.transform.GetChild(1).gameObject.layer = 9;
            lightOn = true;
            rgb_lightOn = false;
            maskLight.GetComponent<Renderer>().material.SetColor("_Emission", new Color(96f,93f,0,120f));
            

            PV.RPC("LightPower", RpcTarget.AllBuffered);

            //maskLight 다른 색됫다가 돌아올 색 적용 코드 필요 
            maskLight.SetActive(lightOn);
        }

    }

    [PunRPC]
    void LightOff() //L,R,G,B 모든 색을 끄는 용도,, 무조건 디폴트 색으로 돌아가게 해줌 .
    {
        lightOn = false;
        rgb_lightOn = false;
        gameObject.transform.GetChild(1).gameObject.layer = 6;

        PlayerMaterials[1] = defaultMaterial;//LightMaterials[0];  원래 디폴트 색깔을 배정 
        mesh.materials = PlayerMaterials;               //배정된 색을 불러와준다

        PV.RPC("LightPower", RpcTarget.AllBuffered);
        maskLight.SetActive(lightOn);
    }

    [PunRPC]
    void LightPower()   //발광의 세기 조절 
    {
        if (lightOn) // L 버튼으로 발광 하면 ~ 
        {
            //emission color의 밝기 1.5배 증가 시키기 
            mesh.materials[1].SetColor("_EmissionColor", PlayerMaterials[1].color * 1.5f);
            spotLight.intensity = 25f;
            networkManager.playerLightCount++; //waitingWall에서 불킨 인원 수 보내주려고
        }

        else        //L,R,G,B 가 다 꺼진 경우에는 기본 밝기 
        {
            mesh.materials[1].SetColor("_EmissionColor", PlayerMaterials[1].color / 1.5f);
            spotLight.intensity = 0;
        }

    }

    [PunRPC]
    void SyncAnimation(string animation, bool value)
    {
        animator.SetBool(animation, value);
    }

    //Photon은 Color를 몰라 ,,즉 포톤은 칼라를 직렬화 하지 못해 Vector로 color를 변환하기  
    [PunRPC]
    void RGB_ON(int matIdx, Vector3 color)  //RGB의 색을 띄게 해준다 
    {

        lightOn = false;
        rgb_lightOn = true;
        gameObject.transform.GetChild(1).gameObject.layer = 6;//9; //오브젝트 [1]번째 자식 옵젝트 접근

        PlayerMaterials[1] = LightMaterials[matIdx]; //색깔을 배정 
        mesh.materials = PlayerMaterials;         //배정된 색을 불러옴 

        

        Color RGBcolor = new Color(color.x, color.y, color.z);
        spotLight.color = RGBcolor;
        spotLight.intensity = 25f;

        maskLight.GetComponent<Renderer>().material.SetColor("_Emission", RGBcolor * 120f);
        maskLight.SetActive(rgb_lightOn);

    }

    [PunRPC]
    void SyncLightPressed(int rgb, bool value) //Rgb가 눌렸는지 상태 변수를 동기화 
    {
        switch (rgb)
        {
            case 0:
                l_pressed = value;
                break;
            case 1:
                r_pressed = value;
                break;
            case 2:
                g_pressed = value;
                break;
            case 3:
                b_pressed = value;
                break;
                
        }
    }
    
    

}

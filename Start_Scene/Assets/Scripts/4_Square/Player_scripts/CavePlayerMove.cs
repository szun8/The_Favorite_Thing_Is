using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CavePlayerMove : MonoBehaviourPunCallbacks
{
    NetworkManager networkManager;
    ReverseGravity reverseGravity;
    WaitingWall wall;   //벽에 플레이어 collision 수를 넘기기 위해서 불러옴 

    MeshRenderer mesh;
    Rigidbody rigid;
    public PhotonView PV;
    public Material[] material; //플레이어의 L,R,G,B material
    
    //캐릭터 이동 관련 변수
    public int JumpForce; //점프력 5
    public float rotSpeed; //방향키 반대이동시 몸의 회전 속도  8
    public float speed; //캐릭터 속도 8
    private Vector3 dir = Vector3.zero;// 캐릭터가 나아갈, 바라볼 방향

    // 플레이어 따라다니는 전등
    public Light pLight1;  //point light
    public Light pLight2;  //spot light

    //캐릭터의 바닥 레이어 검출
    private bool isGround = false;
    private bool isBridge = false;
    private bool isMirrorJump = false;

    private RaycastHit RGBitem;   //일단 남겨두자 플레이어가 바라보는 아이,, 뭐 ,,,, 
    private bool isItem = false;

    //빛 관련 변수
    private bool lightOn = false; //awake에서 false로 바꿔줄 거라서 true
    private bool colorLightOn = false; //RGB 발광 위한 bool

    //플레이어가 발광 가로등과 상호작용 해서 능력얻으면 true 
    public bool getRed = false;
    public bool getGreen = false;
    public bool getBlue = false;

    
    private Material defaultMaterial; //원래 발광 머테리얼 
    private Material lastMaterial; 

    
    /*
    public AudioSource bgm;
    public AudioClip jump;
    AudioSource soundEffect;*/

  
    void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        reverseGravity = GetComponent<ReverseGravity>();
        wall = GameObject.Find("WaitWall").GetComponent<WaitingWall>();

        defaultMaterial = material[0];
        lastMaterial = defaultMaterial;
        

        if (PV.IsMine)
        {
            PV.RPC("StartLight", RpcTarget.AllBuffered);
            Camera.main.GetComponent<CameraMove>().player = gameObject;

            
        }
    }

    
    void Update()
    {
        
        if (PV.IsMine)
        {
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.z = Input.GetAxisRaw("Vertical");
            dir.Normalize(); //대각선 빨라지는거 방지위한 정규화

            //내 밑으로 광선을 쏴서 바닥 레이어랑 닿으면 점프시키기 
            Debug.DrawRay(transform.position, -transform.up * 0.51f, Color.blue);

            //1:쏘는 위치 2:쏘는 방향 3:해당 레이어 
            isGround = Physics.Raycast(transform.position, -transform.up, 0.51f, LayerMask.GetMask("Ground"));
            isBridge = Physics.Raycast(transform.position, -transform.up, 0.51f, LayerMask.GetMask("Bridge"));

            isMirrorJump = Physics.Raycast(transform.position, -transform.up, 0.51f, LayerMask.GetMask("MirrorJump"));
            isItem = Physics.Raycast(transform.position, transform.forward, out RGBitem,1.1f, LayerMask.GetMask("Item"));

            //내 앞으로 광선을 쏴서 물체를 검출해보자 
            Debug.DrawRay(transform.position, transform.forward * 1f, Color.red);

            if (Input.GetKeyDown("space")) //space로 아이템 상호작용할 때는 점프 불가능 
            {
                if (isItem && RGBitem.collider != null)
                {
                    GetItem(RGBitem);
                }

                else
                {
                    //땅이거나 다리를 밟으면
                    if (isGround || isBridge)
                    {
                        //1P일 경우 
                        if(networkManager.p1_id == PV.ViewID)
                        {
                            Debug.Log("jump");
                            rigid.AddForce(Vector2.down * JumpForce, ForceMode.Impulse);
                        }

                        else
                        {
                            rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
                        }
                            
                    }

                    else if (isMirrorJump)
                    {
                        // 1단 점프이지만 속도와 점프력이 상승 
                        if (lightOn)
                        {
                            JumpForce = 7;
                            rotSpeed = 11;
                            speed = 11;
                        }
                        rigid.AddForce(transform.up * JumpForce, ForceMode.Impulse);
                    }
                }
            }

            if (Input.GetKeyDown("l"))
            {
                PV.RPC("LightHandle",RpcTarget.AllBuffered);
            }

            //RPC 함수는 최대 인자 2개만 전송가능 Color는 못넘긴다 ~ 벡터로 변환 해줘야 한다 ~ 
            if(Input.GetKeyDown("r") && getRed)
            {
                PV.RPC("SetRGBColor", RpcTarget.AllBuffered, 1, new Vector3(1,0,0));
            }
            if (Input.GetKeyDown("g") && getGreen)
            {
                PV.RPC("SetRGBColor", RpcTarget.AllBuffered, 2, new Vector3(0, 1, 0));
            }
            if (Input.GetKeyDown("b") && getBlue)
            {
                PV.RPC("SetRGBColor", RpcTarget.AllBuffered, 3, new Vector3(0, 0, 1));
            }

        }
    }
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            //키 입력이 들어왔으면 ~
            if (dir != Vector3.zero)
            {
                //바라보는 방향 부호 != 가고자할 방향 부호
                if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
                {
                    //1P경우
                    if (PV.ViewID == networkManager.p1_id)
                    {
                        transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 180f);
                        Debug.Log("180");
                        
                    }

                    //2P경우 
                    else
                    {
                        transform.Rotate(0, 1, 0);
                        Debug.Log("no 180");
                    }
                }
                

                transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
            }
            rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);

            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name=="WaitWall")   wall.collisionCount++;

        if(collision.gameObject.CompareTag("Ground"))
        {
            JumpForce = 5;
            speed = 8;
            rotSpeed = 8;
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "WaitWall") wall.collisionCount--;
    }

    private void GetItem(RaycastHit item)
    {
        if(item.collider.name == "Red_item")
        {
            getRed = true;
            Destroy(item.collider.gameObject);
        }
        else if (item.collider.name == "Green_item")
        {
            getGreen = true;
            Destroy(item.collider.gameObject);
        }
        else if (item.collider.name == "Blue_item")
        {
            getBlue = true;
            Destroy(item.collider.gameObject);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Red_item" && Input.GetKeyDown("g"))
        {
            getRed = !getRed;
            if (PV.IsMine) Destroy(other.gameObject);
        }
        if (other.gameObject.name == "Green_item") getGreen = !getGreen;
        if (other.gameObject.name == "Blue_item") getBlue = !getBlue;
    }*/
    
    [PunRPC]
    void StartLight()
    {
        mesh.sharedMaterial = material[0];
        pLight1.color = Color.white;
        pLight2.color = Color.white;
        lightOn = false;
        colorLightOn = false;
        PV.RPC("LightPower", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void LightHandle()
    {
        lightOn = !lightOn;
        
        if (lightOn)
        {
            //set default material
            mesh.sharedMaterial = material[0];

            //set lights to white color
            colorLightOn = false;
            pLight1.color = Color.white;
            pLight2.color = Color.white;
            networkManager.playerLightCount++; //waitingWall에서 불킨 인원 수 보내주려고

            PV.RPC("LightPower", RpcTarget.AllBuffered);
        }
        else
        {
            networkManager.playerLightCount--;
            PV.RPC("StartLight", RpcTarget.AllBuffered);
        }
    }
    


    //Photon은 Color를 몰라 ,,즉 포톤은 칼라를 직렬화 하지 못해 Vector로 color를 변환하기  
    [PunRPC]
    void SetRGBColor(int matIdx, Vector3 color)
    {
        colorLightOn = !colorLightOn;
        
        if (colorLightOn)
        {
            lightOn = false;
            Color RGBcolor = new Color(color.x, color.y, color.z);
            
            mesh.sharedMaterial = material[matIdx];
            pLight1.color = RGBcolor;
            pLight2.color = RGBcolor;

            lastMaterial = material[matIdx];

            PV.RPC("LightPower", RpcTarget.AllBuffered);
        }

        else if(!colorLightOn)
        {
            //누른 색을 다시 누르면 꺼짐 
            if(lastMaterial == material[matIdx])
            {
                PV.RPC("StartLight", RpcTarget.AllBuffered);
            }
            //누른 색과 다른 색이면 안꺼	
            else
            {
                colorLightOn = !colorLightOn;
                lightOn = false;
                Color RGBcolor = new Color(color.x, color.y, color.z);

                mesh.sharedMaterial = material[matIdx];
                pLight1.color = RGBcolor;
                pLight2.color = RGBcolor;

                lastMaterial = material[matIdx];

                PV.RPC("LightPower", RpcTarget.AllBuffered);

            }
            
        }
   
    }

    [PunRPC]
    void LightPower()
    {
        //디폴트 빛을 켰거나, Rgb 발광이면 빛의 세기 높이기 
        if(lightOn || colorLightOn)
        {
            gameObject.layer = 9;
            pLight1.intensity = 2;
            pLight1.range = 2.5f;
            pLight2.intensity = 5;
        }

        else //디폴트 빛 세기 빛 껐을 때임 
        {
            gameObject.layer = 6;
            pLight1.intensity = 0;
            pLight1.range = 2.5f;
            pLight2.intensity = 0;
        }
    }

    
}

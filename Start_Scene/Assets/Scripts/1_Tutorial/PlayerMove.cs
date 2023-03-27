using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향
    public GameObject startPos;                // 캐릭터 죽었을시 다시 살아나는 장소(변경가능)
    public int JumpForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    private bool lightOn = false;
    public static bool badak = false;

    private bool isStone = false;

    // 플레이어 따라다니는 전등
    
    public Light spotLight;  //spot light
    public GameObject maskLight; // 발광 구 

    SkinnedMeshRenderer mesh;
    Material[] materials; //현재 eyes, body material 
    Rigidbody rigid;
    Animator animator;
    
    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        Camera.main.GetComponent<CameraMove>().player = gameObject;
        spotLight.intensity = 0;
       

        materials = mesh.sharedMaterials;
        // 재인이가 준 에셋 애니메이션 테스트를 위해 위 주석 처리함

        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        maskLight.SetActive(false);
        //materials[1].EnableKeyword("_EMISSION"); //emission 속성 활성화
        //materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f)); //디폴트 색상
    }

   
    void Update()
    {
        dir.x = Input.GetAxisRaw("Horizontal");
        //dir.z = Input.GetAxisRaw("Vertical");
        //dir.Normalize(); //대각선 빨라지는거 방지위한 정규화

        //내 밑으로 광선을 쏴서 바닥 레이어랑 닿으면 점프시키기 
        Debug.DrawRay(transform.position, Vector2.down * 1.3f, Color.blue);
        //1:쏘는 위치 2:쏘는 방향 3:해당 레이어 
        badak = Physics.Raycast(transform.position, Vector2.down, 1.3f, LayerMask.GetMask("Ground"));

        //내 앞으로 광선을 쏴서 물체를 검출해보자 
        Debug.DrawRay(transform.position, transform.forward * 1.5f, Color.red);

        if (Input.GetKeyDown("space"))
        {
            if (badak != false || isStone)
            {
                rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
                
                animator.Play("jump");   // 1. 어떤 애니매이션 2. layer 3. 얼마나 시간를 두고 해당 애니메이션을 플레이할 것인가
                
            }
        }

        if (Input.GetKey("l"))
        {
            lightOn = true;
            LightHandle();
        }


        if (Input.GetKeyUp("l"))
        {
            lightOn = false;
            LightHandle();
        }
    }

    private void FixedUpdate()
    {
        //키 입력이 들어왔으면 ~
        if (dir != Vector3.zero)
        {
            Move();

            //바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) )//|| Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
    }

    void LightHandle()  //L 누르면 빛 기본값으로 켜짐 다시 누르면 빛 꺼짐 
    {
        maskLight.SetActive(lightOn);
        ResetLight();
     
    }

    void ResetLight()   //발광 디폴트로 바꾸기 
    {
        if (lightOn)
        {
            //emission color의 밝기 1.5배 증가 시키기 
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f)*1.8f);
            spotLight.intensity = 50f;
        }

        else
        {
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f));
            spotLight.intensity = 0;
        }
        
    }

    void Move()
    {
        animator.SetBool("isWalk",true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isStone = true;
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isStone = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            transform.position = startPos.transform.position;
        }
    }
    
    
}

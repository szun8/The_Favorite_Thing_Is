using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향
    public GameObject startPos;             // 캐릭터 죽었을시 다시 살아나는 장소(변경가능)
    public int JumpForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    public bool lightOn = false;
    public static bool badak = false; //바닥 레이어 감지하는지 
    public static bool isStop = false; // 플레이어 이동제한 -> 타이틀 애니메이션할때
    public bool isGround = false; // 바닥과 닿아있는지 collision 
    public bool isJump = false; //점프 중인지 ~ 

    // 플레이어 따라다니는 전등
    public Light spotLight;  //spot light
    public GameObject maskLight; // 발광 구 

    SkinnedMeshRenderer mesh;
    Material[] materials; //현재 eyes, body material 
    Rigidbody rigid;
    Animator animator;

    public PhysicMaterial physicMaterial; //마찰력 0인 피지컬머티리얼 
    PhysicMaterial defaultMaterial;     //원래 기본 플레이어 머티리얼 = none

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        spotLight.intensity = 0;

        materials = mesh.sharedMaterials;
        // 재인이가 준 에셋 애니메이션 테스트를 위해 위 주석 처리함

        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        maskLight.SetActive(false);
        //materials[1].EnableKeyword("_EMISSION"); //emission 속성 활성화
        //materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f)); //디폴트 색상

        defaultMaterial = GetComponentInChildren<MeshCollider>().material;
    }

   
    void Update()
    {
        if (isStop)
        {
            animator.SetBool("isWalk", false);
            return;
        }
       dir.x = Input.GetAxisRaw("Horizontal"); //*--- oldest input system ---*

        rigid.AddForce(Vector3.down * 1.8f); //즁력 더 주기 
        //내 밑으로 광선을 쏴서 바닥 레이어랑 닿으면 점프시키기 
        //Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.blue);
        //1:쏘는 위치 2:쏘는 방향 3:해당 레이어 
        badak = Physics.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        //내 앞으로 광선을 쏴서 물체를 검출해보자 
        //Debug.DrawRay(transform.position, transform.forward * 1.5f, Color.red);
        if (isGround && badak) isJump = false; //바닥에 닿아있으면 점프중이 아님

        if (!badak) isJump = true;

        if (Input.GetKeyDown("space"))
        {
            if (isJump == false)
            {
                animator.SetBool("isWalk", false);
                animator.SetTrigger("isJump");
                rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            }
        }

        if (Input.GetKeyDown("l"))
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
        if (isStop) return;
        //키 입력이 들어왔으면 ~
        if (dir != Vector3.zero)
        {
            if (!isJump) animator.SetBool("isWalk", true);
            else if (isJump) animator.SetBool("isWalk", false);

            //바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) )//|| Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else animator.SetBool("isWalk", false);

        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
    }

    //public void OnMove(InputAction.CallbackContext state)
    //{
    //    dir = state.ReadValue<Vector2>();
    //}

    //public void OnJump(InputAction.CallbackContext state)
    //{
    //    if (state.performed && isJump == false)
    //    {
    //        animator.SetBool("isWalk", false);
    //        animator.SetTrigger("isJump");
    //        rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
    //    }
    //}

    //public void OnLight(InputAction.CallbackContext state)
    //{
    //    if (state.performed)
    //    {   // GetKeyDown
    //        lightOn = true;
    //        LightHandle();
    //    }
    //    else
    //    {   // GetKeyUp
    //        lightOn = false;
    //        LightHandle();
    //    }
    //}

    public void LightHandle()  //L 누르면 빛 기본값으로 켜짐 다시 누르면 빛 꺼짐 
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
            spotLight.intensity = 25f;
        }

        else
        {
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f));
            spotLight.intensity = 0;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGround = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !badak)//안 밟고 있는데 벽과 충돌해있으면 떨어지게 하자 
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = physicMaterial;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = defaultMaterial;
            isGround = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            transform.position = startPos.transform.position;
        }
        if (other.CompareTag("SavePoint")) startPos = other.gameObject;
    }
}

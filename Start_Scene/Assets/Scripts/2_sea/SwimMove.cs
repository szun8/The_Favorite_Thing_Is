using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimMove : MonoBehaviour
{
    Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향

    public GameObject[] startPos;             // 캐릭터 죽었을시 다시 살아나는 장소(변경가능)
    public int JumpForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    private bool lightOn = false;
    
    public static bool isBoss = false;      // 물고기 벽이 무너지면 true하고 보스 setActive시킬 예정
    public static bool isDied = false;      // 플레이어의 죽음여부

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

        //animator = GetComponent<Animator>();
        animator = GetComponent<Animator>();

        rigid = GetComponent<Rigidbody>();
        //rigid = GetComponent<Rigidbody>();

        maskLight.SetActive(false);
        
    }

    void Start()
    {
        transform.position = startPos[0].transform.position;
        dashSpeed = 5f;
    }


    void Update()
    {
        if (isDied && Dead()) return;
       
        if (Water.isWater && !isBoss)
        {   // 보스 만나기 전에는 z축이동가능
            dir.x = Input.GetAxisRaw("Vertical");
            dir.z = Input.GetAxisRaw("Horizontal") * -1;
            dir.Normalize(); //대각선 빨라지는거 방지위한 정규화
        }
        else
        {
            dir.x = Input.GetAxisRaw("Horizontal");
        }

        if (Input.GetKey("l"))
        {
            lightOn = true;
            LightHandle();

            if (isJelly)
            {
                isJelly = false;
                isBooster = true;
                StartCoroutine(Dash());
            }
        }

        if (Input.GetKeyUp("l"))
        {
            //SetDeadState();
            lightOn = false;
            LightHandle();
            StopCoroutine(Dash());
            speed = 5f;
            dashSpeed = 0f;
            isBooster = false;
        }
    }

    private void FixedUpdate()
    {
        if (isDied && Dead()) return;
        
        //키 입력이 들어왔으면 ~
        if (dir != Vector3.zero)
        {
            Move();

            //바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x))//|| Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else if (Water.isWater)
        {
            animator.SetBool("isSwim", false);
        }
        Jump();
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
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f) * 1.8f);
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
        if (Water.isWater)
        {
            animator.SetBool("isSwim", true);
        }
    }

    void Jump()
    {
        if (Water.isWater && Input.GetKey("space"))
        {
            rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            SoundManager.instnace.PlaySE("PlayerJump");
        }
    }

    public float dashSpeed;
    public bool isJelly, isBooster;

    public void SpeedUP()
    {
        dashSpeed = dashSpeed + 5f; // default : 3 -> total 6
                                // 아직 l버튼을 안눌렀으니 일단 증가변수만 저장
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        float duration = 3f;
        float startSpeed = dashSpeed;

        while (speed > 3f)
        {
            float t = (Time.time - startTime) / duration; // 보간 시간 계산
            speed = Mathf.Lerp(startSpeed, 5f, t);
            if (isJelly)
            {
                yield break; // 새해파리를 먹었으면 코루틴을 중단합니다.
            }
            yield return null;
        }
        isBooster = false;
    }

    public void SetDeadState()
    {   // 심해에서 보스에 닿아 죽으면 물고기 벽 부딪히게 전으로 돌아감 
        isDied = true;
        isBoss = false;
        UIManager.instnace.stopOut = false;
        Invoke("SetPos", 1.5f);
    }
    void SetPos()
    {   // 죽었을 경우 다시 시작되는 장소 설정
        transform.position = startPos[1].transform.position;   // 바꿔야함 리스폰 pos로
    }

    bool Dead()
    {
        if (UIManager.instnace.stopIn && UIManager.instnace.stopOut)
        {   // 보스에 닿아서 죽었을 때 다시 중간 스폰 자리로 돌아오면
            isDied = false;
            return false;
        }
        else return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwimMove : MonoBehaviour
{
    Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향

    public GameObject[] pos;                // [0] 시작 장소, [1] 재시작 장소, [2] 엔딩 장소
    public int JumpForce, DownForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    private bool lightOn = false, isMile1 = false; // mile1은 보스 생성전 플레이어 Z축 조정을 위한 변수
    
    public static bool isBoss = false;      // 물고기 벽이 무너지면 true하고 보스 setActive시킬 예정
    public static bool isDied = false;      // 플레이어의 죽음여부
    public static bool isEnd = false;       // 자동이동 여부
    public static bool isCave = false;      // 동굴 속으로 들어가는 이벤트 -> 어떠한 키조작 X

    // 플레이어 따라다니는 전등
    public Light spotLight;  //spot light
    public GameObject maskLight; // 발광 구 

    SkinnedMeshRenderer mesh;
    Material[] materials; //현재 eyes, body material 
    Rigidbody rigid;
    Animator animator;
    CinemachineDollyCart dolly;

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        spotLight.intensity = 0;
        materials = mesh.sharedMaterials;

        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        dolly = GetComponent<CinemachineDollyCart>();

        maskLight.SetActive(false);
    }

    void Start()
    {
        transform.position = pos[0].transform.position;
        dashSpeed = 10f;
    }

    void Update()
    {
        if(isCave)
        {   // 다음씬으로 넘어갑니...다
            rigid.useGravity = false;
            isCave = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(500f, 35f, 7f), Time.deltaTime * 0.5f);
            Invoke("NextScene", 1f);
            return;
        }
        if (isDied) return;

        if (Water.isWater && !isBoss)
        {   // 보스 만나기 전에는 z축이동가능
            dir.x = Input.GetAxisRaw("Vertical");
            dir.z = Input.GetAxisRaw("Horizontal") * -1;
            dir.Normalize(); //대각선 빨라지는거 방지위한 정규화
        }
        else if(isBoss && isMile1)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, 0f), Time.deltaTime*1.5f);
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
            StopCoroutine(Dash());
            lightOn = false;
            LightHandle();
            isBooster = false;
        }

        if (isEnd)
        {
            if (!dolly.enabled)
            {
                transform.position = Vector3.Lerp(transform.position, dolly.m_Path.transform.position, 0.05f);
                if(transform.position.x >= 399 && transform.position.y >= 39) dolly.enabled = true;
            }   
            else if(dolly.m_Position == dolly.m_Path.PathLength)
            {
                ControlVCam.instance.ControlDollyView();
                if(GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss.speed != 50)
                    GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss.speed = 50f;
                if (speed != 10 || dashSpeed != 10)
                {   // 이거 안해놓으면 추적한 다음에 남은 속도로 인해 플레이어 이동시 우주로 날라가는 ,,,버그
                    speed = 10;
                    dashSpeed = 10;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDied || isEnd || isCave) return;

        //키 입력이 들어왔으면 ~
        if (dir != Vector3.zero)
        {
            Move();

            //바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else if (Water.isWater)
        {
            animator.SetBool("isSwim", false);
        }
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);

        Jump();
        Down();
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
            SoundManager.instnace.PlaySE("PlayerSeaJump", 0.5f);
        }
    }

    void Down()
    {
        if(Water.isWater && Input.GetKey(KeyCode.LeftShift))
        {
            rigid.AddForce(Vector2.down * DownForce, ForceMode.Impulse);
        }
    }

    float dashSpeed;
    public float maxDash;
    public bool isJelly, isBooster;

    public void SpeedUP()
    {
        if (dashSpeed < maxDash)
        {   // 최대 속도 40:default으로 지정
            dashSpeed = dashSpeed + 10f; // 아직 l버튼을 안눌렀으니 일단 증가변수만 저장
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        float duration = 3f;
        float startSpeed = dashSpeed;

        while (speed > 9f)
        {
            float t = (Time.time - startTime) / duration; // 보간 시간 계산
            speed = Mathf.Lerp(startSpeed, 10f, t);
            if (isJelly || Input.GetKeyUp("l"))
            {
                speed = 10;
                dashSpeed = 10;
                yield break; // 새해파리를 먹었으면 코루틴을 중단합니다.
            }
            yield return null;
        }
        isBooster = false;
    }

    public void SetDeadState()
    {   // 심해에서 보스에 닿아 죽으면 물고기 벽 부딪히게 전으로 돌아감 
        isBoss = false;
        isMile1 = false;
        UIManager.instnace.stopOut = false;
        Invoke("SetPos", 1.5f);
    }
    void SetPos()
    {   // 죽었을 경우 다시 시작되는 장소 설정
        ControlVCam.instance.SwitchingSideToBack();
        transform.position = pos[1].transform.position;   // 바꿔야함 리스폰 pos로
        isDied = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isEnd)
        {   // 보스와 충돌시 && isEnd가 아니면(죽어도 되는데) isEnd상황이라면 (dollycart를 탑승해서 죽으면 안됨)
            SetDeadState();
            GameObject.Find("Disappear_Structure").transform.Find("MileStone 1").gameObject.SetActive(true);
            speed = 10f; // 이전에 먹었던 해파리 능력 초기화
            dashSpeed = 10f;
        }
    }

    IEnumerator SetBoss()
    {
        isBoss = true;
        isMile1 = true;
        CinematicBar.instance.ShowBars();
        ControlVCam.instance.SwitchingBackToSide();
        GameObject.Find("MileStone 1").SetActive(false);
        
        yield return new WaitForSeconds(1.3f);

        GameObject.Find("SpawnManager").transform.Find("boss_0").gameObject.SetActive(true);
        ControlVCam.instance.SwitchingWatchingBoss();
        Debug.Log("SetWatchingBoss");

        isMile1 = false;
        yield return new WaitForSeconds(1.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {   // 물고기 떼와 충돌(변경예정_0328.ver)
            // 물고기가 촤악 사라지는 무빙 이후 보스 활성화 시키는 것이 좋을듯함 -> 아래줄 invoke 함수화
            StartCoroutine(SetBoss());
        }
        if (other.gameObject.CompareTag("End"))
        {   // 보스 탈출 : dollycart 탑승
            isEnd = true;
            CinematicBar.instance.ShowBars();   // 블랙바 On
            ControlVCam.instance.SwitchingSideToBoss();
        }

        if(other.gameObject.name == "EndPos")
        {   // 동굴 입구로 들어가려고 하면 캠 전환
            ControlVCam.instance.SwitchingBossToCave();
        }

        if(other.gameObject.name == "CavePos")
        {   // 씬전환 트리거
            isCave = true;
            other.gameObject.SetActive(false);  // 한번만 닿으면 해당 오브젝트를 비활성화시켜 중복처리 되는 일이 없도록 합니다
        }
    }

    void NextScene()
    {
        ScenesManager.instance.Scene[ScenesManager.instance.SceneNum] = true;
        SoundManager.instnace.VolumeOutBGM();
    }
}

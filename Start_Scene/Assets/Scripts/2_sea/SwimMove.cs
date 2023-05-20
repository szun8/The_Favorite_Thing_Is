using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SwimMove : MonoBehaviour
{
    Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향

    public GameObject[] pos;                // [0] 시작 장소, [1] 재시작 장소, [2] 엔딩 장소
    public int JumpForce, DownForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    public bool lightOn = false;
    bool isESC = false;                     // 한번만 누르게 !!!
    bool isSeahorse = false;                // 해마가 위치한 곳에서 L키 눌렀는지 여부
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
        if (!videoHandler.instance.videoPlayer.isPlaying && !isESC && Input.GetKeyDown(KeyCode.Escape))
        {   // 비디오가 플레이되고 있지 않고 아직 한번도 esc키가 눌러지지 않았다면 실행
            NextScene();
            isESC = true;
        }

        if (isSeahorse && !isBoss)
        {   // 해마 발광 영역에 플레이어가 들어왔고 거기서 L키를 누르면 사이드뷰로 전환한다
            StartCoroutine(SetBoss());
            isSeahorse = false;
            lightOn = false;
            LightHandle();
        }

        if (isCave)
        {   // 다음씬으로 넘어가기 위해 플레이어 동굴 안으로 빨려 들어가는 자동 이동
            transform.position = Vector3.Lerp(transform.position, new Vector3(500f, 35f, 7f), Time.deltaTime * 0.5f);
            return;
        }
        if (isDied)
        {
            if (lightOn)
            {   // 죽을때 빛을 켜놓고 죽으면 다시 스폰되고 발광버튼을 안누르고 있어도 발광중인 버그존재로 인해 발광 자체 꺼주기~
                lightOn = false;
                LightHandle();
            }
            return;
        }

        if (isLight)
        {
            lightOn = true;
            LightHandle();

            if (isJelly)
            {   // 해파리를 먹어서 속도가 plus됐고 L키를 눌렀다면 대시 기능 ON
                isJelly = false;
                isBooster = true;
                StartCoroutine(Dash());
            }
        }

        if(isEnd && dolly.m_Position == dolly.m_Path.PathLength)
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

    private void FixedUpdate()
    {
        if (isDied || isEnd || isCave) return;

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
        else if (Water.isWater)
        {
            animator.SetBool("isSwim", false);
        }
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);

        if(isDown) rigid.AddForce(Vector2.down * DownForce, ForceMode.Impulse);
    }

    public void OnMove(InputAction.CallbackContext state)
    {
        if (Water.isWater)
        {
            if (!isBoss)
            {   // 보스 만나기 전에는 z축이동가능
                dir = state.ReadValue<Vector3>();
                float z = dir.z;
                dir.z = (dir.x * -1);
                dir.x = z;
            }
            else
            {
                dir = state.ReadValue<Vector3>();
                dir.z = 0f;
            }
            animator.SetBool("isSwim", true);
        }
    }

    public void OnJump(InputAction.CallbackContext state)
    {
        if (isDied) return;
        if (Water.isWater && state.performed)
        {
            rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            SoundManager.instnace.PlaySE("PlayerSeaJump", 0.5f);
        }
    }

    bool isDown = false;
    public void OnDown(InputAction.CallbackContext state)
    {
        if (isDied) return;

        if (Water.isWater && state.performed)
        {   // get key
            isDown = true;
        }
        else if(Water.isWater && state.canceled)
        {
            isDown = false;
        }
    }

    bool isLight = false;
    public void OnLight(InputAction.CallbackContext state)
    {
        if (isDied)
        {   // 죽었을때 발광 키를 누르고 있다가 떼면 발광 해제
            if (state.canceled)
            {
                isLight = false;
                lightOn = false;
                LightHandle();
            }
            return;
        }

        if (!isEnd && state.performed )
        {   // getKey
            isLight = true;
        }

        if (!isEnd && state.canceled)
        {   // get keyUp
            isLight = false;
            lightOn = false;
            LightHandle();

            isBooster = false;
        }
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

    public float dashSpeed;
    public float maxDash;
    public bool isJelly, isBooster;

    public void SpeedUP()
    {
        if (dashSpeed < maxDash)
        {   // 중복으로 먹어도 계속 20을 유지
            dashSpeed = 30f; // 아직 l버튼을 안눌렀으니 일단 증가변수만 저장
        }
    }

    IEnumerator Dash()
    {
        if (!lightOn) yield break;
        speed = dashSpeed;
        while (speed > 10.5f)
        {
            speed = Mathf.Lerp(speed, 10f, Time.deltaTime*0.75f);
            if (isJelly && lightOn)
            {
                speed = 30;
            }
            else if (!lightOn)
            {
                break;
            }
            yield return null;
        }
        dashSpeed = 10f;
        speed = 10f;
        isBooster = false;
    }

    public void SetDeadState()
    {   // 심해에서 보스에 닿아 죽으면 물고기 벽 부딪히게 전으로 돌아감 
        isBoss = false;
        UIManager.instnace.stopOut = false;
        Invoke("SetPos", 1.5f);
    }

    void SetPos()
    {   // 죽었을 경우 다시 시작되는 장소 설정
        ControlVCam.instance.SwitchingSideToBack();
        transform.position = pos[1].transform.position;   // 바꿔야함 리스폰 pos로
        isDied = false;
        GameObject.Find("Disappear_Structure").transform.Find("MileStone 1").gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isEnd)
        {   // 보스와 충돌시 && isEnd가 아니면(죽어도 되는데) isEnd상황이라면 (dollycart를 탑승해서 죽으면 안됨)
            SetDeadState();
            speed = 10f; // 이전에 먹었던 해파리 능력 초기화
            dashSpeed = 10f;
        }
    }

    IEnumerator SetBoss()
    {
        UIManager.instnace.RunAnimsBool("isSeaMoveInfoOn", false);    // 사이드뷰로 전환될때는 안내 창 끄기
        UIManager.instnace.RunAnims("isSeaMoveInfoOff");
        yield return new WaitForSeconds(0.3f);

        isBoss = true;
        CinematicBar.instance.ShowBars();
        ControlVCam.instance.SwitchingBackToSide();
        GameObject.Find("MileStone 1").SetActive(false);
        
        dir = Vector3.zero; // player가 계속 side뷰로 바뀌는 도중에도 w, s를 눌러 dir이 변경되는 경우를 대비해 스크립트가 비활성화된 뒤 이동dir을 zero로 초기화

        yield return new WaitForSeconds(1.3f);

        GameObject.Find("SpawnManager").transform.Find("boss_0").gameObject.SetActive(true);
        ControlVCam.instance.SwitchingWatchingBoss();
        Debug.Log("SetWatchingBoss");

        yield return new WaitForSeconds(1.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {   // 해마 가운데서 l버튼을 누르면
            isSeahorse = true;
        }
        if (other.gameObject.CompareTag("End") && !isDied)
        {   // 보스 탈출 : dollycart 탑승
            isEnd = true;
            dolly.enabled = true;
            if (lightOn)
            {   // dolly를 탈때는 발광 비활성화
                Debug.Log("light off");
                isLight = false;
                lightOn = false;
                LightHandle();
            }
            CinematicBar.instance.ShowBars();   // 블랙바 On
            ControlVCam.instance.SwitchingSideToBoss();
        }

        if(other.gameObject.name == "EndPos")
        {   // 동굴 입구로 들어가려고 하면 캠 전환
            ControlVCam.instance.SwitchingBossToCave();
        }

        if(other.gameObject.name == "CavePos")
        {   // 씬전환 한번!!!만 트리거
            Debug.Log("trigger cavePos");
            CinematicBar.instance.ShowBars();

            isCave = true;
            rigid.useGravity = false;
            other.gameObject.SetActive(false);  // 한번만 닿으면 해당 오브젝트를 비활성화시켜 중복처리 되는 일이 없도록 합니다
            Invoke("NextScene", 1f);
        }
    }

    void NextScene()
    {   // 다음씬으로 전환하기 위한 함수
        SoundManager.instnace.VolumeOutBGM();
        if (isCave) CinematicBar.instance.HideBars();
        ScenesManager.instance.Scene[ScenesManager.instance.SceneNum] = true;
    }
}

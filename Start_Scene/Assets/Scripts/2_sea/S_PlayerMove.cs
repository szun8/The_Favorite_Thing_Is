using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerMove : MonoBehaviour
{
    public static bool lightOn = true, isDied = false, isBroken = false, isBoss = false;
    // isBoss = true인 경우에는 사이드뷰로 전환 및 보스물고기 spawn on
    // 불이 꺼져있는지 아닌지에 따라 물고기(적)-isTrigger 여부 활용 -> 따라서 다른 스크립트에서도 사용하도록 static선언
    public int JumpForce;               //지상 캐릭터 점프력
    public float speed = 8f;            //지상 캐릭터 속도

    public float rotSpeed = 8f;         //방향키 반대이동시 몸의 회전 속도 
    public float swimSpeed;             //수중 캐릭터 속도

    private Vector3 dir = Vector3.zero; // 캐릭터가 나아갈, 바라볼 방향 
    private Vector3 camSpot, sideView, sideAngle, backView, backAngle;

    GameObject playerLight, playerPointLight;  // 플레이어 따라다니는 spotLight & pointLight
    Rigidbody rigid;

    bool isJelly = false, isBooster = false;
    float dashSpeed;

    [SerializeField] private string jumpSound;
    [SerializeField] private GameObject player;

    void Awake()
    {
        sideView = new Vector3(0f, 5f, 15f);
        sideAngle = new Vector3(4f, -180f, 0f);
        backView = new Vector3(12f, 5f, 0f);
        backAngle = new Vector3(20f, -90f, 0f);
        Camera.main.transform.eulerAngles = sideAngle;
        camSpot = sideView;

        rigid = GetComponent<Rigidbody>();

        playerLight = GameObject.Find("Light");
        playerPointLight = GameObject.Find("Point Light");
    }

    void Start()
    {
        //SoundManager.instnace.PlayBGM();
    }

    void Update()
    {
        if (isDied) return;
        Camera.main.transform.position = transform.position + camSpot;

        dir.x = Input.GetAxisRaw("Horizontal")*-1;
        dir.z =  Input.GetAxisRaw("Vertical")*-1;
        dir.Normalize(); // 대각선 빨라지는거 방지위한 정규화
        if (Input.GetKeyUp("l"))
        {
            Debug.Log("KeyUp");
            speed = 8f;
            dashSpeed = 8f;
            isBooster = false;
            isJelly = false;
        }
        if (!isDied && Input.GetKey("l"))
        {
            //LightHandle();
            //if ()
            if (isJelly)
            {
                isJelly = false;
                speed = dashSpeed;
                isBooster = true;
                StartCoroutine(Dash());
            }
            else if (isBooster)
            {   // 그냥 else로 뺄수있을까?
                StartCoroutine(Dash());
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (isDied) return;
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

        if (!Water.isWater)
        {   // 지상 이동 관련
            rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
            if (Input.GetKeyDown("space"))
            {
                SoundManager.instnace.PlaySE(jumpSound);    // 모든 오디오 관리를 해주는 SoundManager에 문자열만 넘겨줘서 재생하는 구조
                rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            rigid.MovePosition(transform.position + dir * Time.deltaTime * swimSpeed);  // 수중 이동 속도적용
            if (Input.GetKey("space")) rigid.velocity = transform.up * swimSpeed;       // 수중 점프 적용
        }
 
    }

    void LightHandle()
    {
        lightOn = !lightOn;
        if (lightOn)
        {
            playerLight.GetComponent<Light>().intensity = 20;
            playerPointLight.GetComponent<Light>().intensity = 1;
        }
        else
        {
            playerLight.GetComponent<Light>().intensity = 0;
            playerPointLight.GetComponent<Light>().intensity = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy")){
            // Destroy(this.gameObject);
            isDied = true;
            UIManager.instnace.PlayerDead();
            Invoke("Restart", 2f);
        }
    }

    public void Restart()
    {
        isDied = false;
        UIManager.instnace.PlayerRelive();
    }

    void SpeedUP()
    {
        dashSpeed = speed + 3f; // default : 3 -> total 6
                                // 아직 l버튼을 안눌렀으니 일단 증가변수만 저장
        Debug.Log("DashSpeed : " + dashSpeed);
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        float duration = 3f;
        float startSpeed = dashSpeed;

        while (speed > 3f)
        {
            float t = (Time.time - startTime) / duration; // 보간 시간 계산
            speed = Mathf.Lerp(startSpeed, 8f, t);
            if (isJelly)
            {
                yield break; // 새해파리를 먹었으면 코루틴을 중단합니다.
            }
            yield return null;
        }
        isBooster = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            isJelly = true; // 속도 증가 on
            Debug.Log("isJelly : " + isJelly);
            SpeedUP();
            // 해파리 destroy() 진행 -> 뭐 따로 정적변수로 둬서 처리하면 될듯
        }
    }
}
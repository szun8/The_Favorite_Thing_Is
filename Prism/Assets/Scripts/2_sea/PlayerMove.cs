using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
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

    [SerializeField] private string jumpSound;
    [SerializeField] private GameObject player;

    void Awake()
    {
        sideView = new Vector3(0f, 5f, 15f);
        sideAngle = new Vector3(5f, 180f, 0f);
        backView = new Vector3(12f, 5f, 0f);
        backAngle = new Vector3(20f, -90f, 0f);
        Camera.main.transform.eulerAngles = backAngle;
        camSpot = backView;

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

        dir.x = Input.GetAxisRaw("Vertical")*-1; 
        dir.z = Input.GetAxisRaw("Horizontal");
        dir.Normalize(); // 대각선 빨라지는거 방지위한 정규화

        if (!isDied && Input.GetKeyDown("l"))
            LightHandle();
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
            transform.forward = Vector3.Lerp(transform.forward, dir*-90, Time.deltaTime * rotSpeed);
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    private Vector3 dir = Vector3.zero;// 캐릭터가 나아갈, 바라볼 방향 
    public int JumpForce = 5; //점프력
    public float rotSpeed = 8; //방향키 반대이동시 몸의 회전 속도 
    public float speed = 8; //캐릭터 속도

    private bool lightOn = false;
    private bool badak = false;

    // 플레이어 따라다니는 전등
    public Light pLight1;  //point light
    public Light pLight2;  //spot light

    MeshRenderer mesh;
    Material mat;
    Rigidbody rigid;


    void Awake()
    {


        Camera.main.GetComponent<CameraMove>().player = gameObject;
        pLight1.intensity = 0;
        pLight2.intensity = 0;

        mesh = GetComponent<MeshRenderer>();
        mat = mesh.material;

        rigid = GetComponent<Rigidbody>();

    }


    void Update()
    {
        dir.x = Input.GetAxisRaw("Horizontal");
        dir.z = Input.GetAxisRaw("Vertical");
        dir.Normalize(); //대각선 빨라지는거 방지위한 정규화

        //내 밑으로 광선을 쏴서 바닥 레이어랑 닿으면 점프시키기 
        Debug.DrawRay(transform.position, Vector2.down * 1.3f, Color.blue);
        //1:쏘는 위치 2:쏘는 방향 3:해당 레이어 
        badak = Physics.Raycast(transform.position, Vector2.down, 1.3f, LayerMask.GetMask("Ground"));

        //내 앞으로 광선을 쏴서 물체를 검출해보자 
        Debug.DrawRay(transform.position, transform.forward * 1.5f, Color.red);

        if (Input.GetKeyDown("space"))
        {
            if (badak != false)
            {
                rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            }
        }

        if (Input.GetKeyDown("l"))
        {
            //LightHandle();
        }

    }

    private void FixedUpdate()
    {
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
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Vector3 dir = Vector3.zero;// 캐릭터가 나아갈, 바라볼 방향 
    public int JumpForce; //점프력
    public float rotSpeed; //방향키 반대이동시 몸의 회전 속도 
    public float speed; //캐릭터 속도

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
        Debug.DrawRay(transform.position, Vector2.down * 0.51f, Color.white);
        //1:쏘는 위치 2:쏘는 방향 3:해당 레이어 
        badak = Physics.Raycast(transform.position, Vector2.down, 0.51f, LayerMask.GetMask("Ground"));

        //내 앞으로 광선을 쏴서 물체를 검출해보자 
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.white);

        if (Input.GetKeyDown("space"))
        {
            if (badak != false)
            {
                rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            }
        }

        
        if (Input.GetKeyDown("l"))
        {
            LightHandle(); 
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

    void LightHandle()  //L 누르면 빛 기본값으로 켜짐 다시 누르면 빛 꺼짐 
    {
        lightOn = !lightOn;
        //playerLight.SetActive(lightOn);
        if (lightOn) ResetLight();

        else
        {
            pLight1.intensity = 0;
            pLight1.range = 2.5f;
            pLight2.intensity = 0;
        }
    }

    void ResetLight()   //발광 디폴트로 바꾸기 
    {
        if (lightOn)
        {
            pLight1.intensity = 2;
            pLight1.range = 2.5f;
            pLight2.intensity = 5;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //불켜져있고, 돌이나 벽에 닿았을 때 -> 첫 씬에서만 사용 
        if (lightOn)
        {
            if (collision.gameObject.CompareTag("Stone") || collision.gameObject.CompareTag("Wall"))
            {
                CancelInvoke(nameof(ResetLight));
                if (pLight1.intensity <= 13 && pLight2.intensity <= 16)
                {
                    pLight1.intensity++;
                    pLight1.range += 0.05f;
                    pLight2.intensity++;
                }
            }
        }

        

    }

    private void OnCollisionExit(Collision collision)
    {
        //돌에 박고 떨어지면 2초후 빛 세기 돌아옴 
        if (collision.gameObject.CompareTag("Stone"))
        {
            Invoke(nameof(ResetLight), 2f);
        }

        //벽에 박고 떨어지면 4초후 빛 세기 돌아옴 
        if (collision.gameObject.CompareTag("Wall"))
        {
            Invoke(nameof(ResetLight), 3.5f);
        }
    }
}

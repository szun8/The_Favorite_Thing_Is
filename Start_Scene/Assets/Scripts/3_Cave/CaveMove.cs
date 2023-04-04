using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMove : MonoBehaviour
{
    Vector3 dir = Vector3.zero;
    public Transform[] pos;
    public float speed, rotSpeed, jumpForce;
    public  bool lightOn = false;

    public static bool badak = false;

    // 플레이어 따라다니는 전등
    public Light spotLight;  //spot light
    public GameObject maskLight; // 발광 구

    SkinnedMeshRenderer mesh;
    Material[] materials; //현재 eyes, body material 
    Rigidbody rigid;
    public Animator animator;

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        spotLight.intensity = 0;
        maskLight.SetActive(false);
        materials = mesh.sharedMaterials;
        transform.position = pos[0].position;
    }

    // 1. 동굴 내부 2. 거울방
    void Update()
    {
        dir.x = Input.GetAxisRaw("Horizontal");
        Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.blue);
        badak = Physics.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

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

    void FixedUpdate()
    {
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
        else animator.SetBool("isWalk", false);

        Jump();
    }

    void Move()
    {
        animator.SetBool("isWalk", true);
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
    }

    void Jump()
    {
        if (badak && Input.GetKey("space"))
        {
            rigid.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            SoundManager.instnace.PlaySE("PlayerJump");
        }
    }

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
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f) * 1.8f);
            spotLight.intensity = 25f;
        }

        else
        {
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f));
            spotLight.intensity = 0;
        }
    }
}

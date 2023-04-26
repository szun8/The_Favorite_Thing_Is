using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMove : MonoBehaviour
{
    Vector3 dir = Vector3.zero;
    public Transform[] pos;
    int curPos = 0;
    public float speed, rotSpeed, jumpForce;

    public bool lightOn = false;
    public static bool badak = false;
    public static bool isStop = false;
    public static bool isDied = false;
    public static bool isMirror = false;

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
        transform.position = pos[curPos].position;
    }

    // 1. 동굴 내부 2. 거울방
    void Update()
    {
        if (isDied) return;
        if (isMirror)
        {
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.z = Input.GetAxisRaw("Vertical");
        }
        else dir.x = Input.GetAxisRaw("Horizontal");

        Jump();
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
        if (isDied) return;
        if (isStop)
        {
            animator.SetBool("isWalk", false);
            return;
        }
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

        
    }

    void Move()
    {
        animator.SetBool("isWalk", true);
        rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
    }

    void Jump()
    {
        if (badak && Input.GetKeyDown("space"))
        {
            badak = false;
            rigid.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isWalk", false);
            animator.SetTrigger("isJump");
            SoundManager.instnace.PlaySE("PlayerJump", 0.1f);
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
            spotLight.intensity = 900f;
        }

        else
        {
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f));
            spotLight.intensity = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!badak && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Stairs")))
        {
            badak = true;
        }
        //if (collision.gameObject.CompareTag("Dead"))
        //{
        //    SetCurStance(4);
        //}
        if (collision.gameObject.CompareTag("Stone"))
        {
            if (HiddenPlates.isSafe) return;
            isDied = true;
            SetCurStance(1);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            SetCurStance(3);
        }
    }

    void SetCurStance(int i)
    {
        UIManager.instnace.stopOut = false;
        curPos = i;
        Invoke("SetPos", 1.5f);
    }

    void SetPos()
    {   // 죽었을 경우 다시 시작되는 장소 설정
        transform.position = pos[curPos].position;
        isDied = false;
    }
}

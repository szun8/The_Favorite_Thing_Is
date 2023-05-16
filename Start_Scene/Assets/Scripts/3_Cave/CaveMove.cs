using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaveMove : MonoBehaviour
{
    Vector3 dir = Vector3.zero;                 // 움직일 방향 입력 변수
    public GameObject[] pos;                     // 조정당할 위치 배열
    GameObject savePoint;
    public float speed, rotSpeed, jumpForce;

    public bool lightOn = false;
    public static bool badak = false;
    public static bool isStop = false;
    public static bool isDied = false;

    // 플레이어 따라다니는 전등
    public Light spotLight;         //spot light
    public GameObject maskLight;    // 발광 구

    SkinnedMeshRenderer mesh;
    Material[] materials;           //현재 eyes, body material 
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
        
        transform.position = pos[0].transform.position;  // 플레이어 시작위치 초기화
        
        savePoint = pos[1];
        SoundManager.instnace.PlayBGM();
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
            rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);
            animator.SetBool("isWalk", true);
            //바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else animator.SetBool("isWalk", false);
    }

    public void OnMove(InputAction.CallbackContext state)
    {
        if (state.performed)
        {
            dir = state.ReadValue<Vector3>();
        }
    }

    public void OnJump(InputAction.CallbackContext state)
    {
        if (isStop || isDied) return;

        if (badak && state.performed)
        {
            badak = false;
            rigid.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isWalk", false);
            animator.SetTrigger("isJump");
            SoundManager.instnace.PlaySE("PlayerJump", 0.1f);
        }
    }

    public void OnLight(InputAction.CallbackContext state)
    {
        if (isDied) return;

        if (state.performed)
        {
            lightOn = true;
            LightHandle();
        }

        else if (state.canceled)
        {
            lightOn = false;
            LightHandle();
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
        {   // 점프 가능 상태
            badak = true;
        }
        if (collision.gameObject.CompareTag("Stone"))
        {
            isDied = true;

            lightOn = false;    // 죽었을때 L 킨채로 죽으면 다시살아날때 키 안눌러도 활성화된 채로 살아나는 버그로 인해 죽으면 발광 OFF
            LightHandle();

            StartCoroutine(SetPos());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item") && lightOn)
        {   // 끝부분 이스터에그 워프기능
            savePoint = pos[3];
            StartCoroutine(SetPos());
        }
        if (other.CompareTag("SavePoint"))
        {
            savePoint = other.gameObject;
        }

        if (other.CompareTag("Dead"))
        {
            StartCoroutine(SetPos());
        }
    }

    IEnumerator SetPos()
    {   // 죽으면 세이브포인트로 플레이어 이동
        UIManager.instnace.stopOut = false;
        yield return new WaitForSeconds(1.5f);

        transform.position = savePoint.transform.position;
        isDied = false;
    }
}

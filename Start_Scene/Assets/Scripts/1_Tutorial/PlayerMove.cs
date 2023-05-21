using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private Vector3 dir = Vector3.zero;     // 캐릭터가 나아갈, 바라볼 방향
    public GameObject startPos;             // 캐릭터 죽었을시 다시 살아나는 장소(변경가능)
    public int JumpForce;                   // 점프력
    public float rotSpeed;                  // 방향키 반대이동시 몸의 회전 속도 
    public float speed;                     // 캐릭터 속도

    public bool lightOn = false;
    public static bool isStart = false; // 튜토 완전 첫 시작을 위한 시작 변수
    public static bool badak = false;   // 바닥 레이어 감지하는지 
    public static bool isStop = false;  // 플레이어 이동제한 -> 타이틀 애니메이션할때
    public bool isGround = false;       // 바닥과 닿아있는지 collision 
    public bool isJump = false;         // 점프 중인지 ~ 

    // 플레이어 따라다니는 전등
    public Light spotLight;         // spot light
    public GameObject maskLight;    // 발광 구 

    SkinnedMeshRenderer mesh;
    Material[] materials; // 현재 eyes, body material 
    Rigidbody rigid;
    Animator animator;

    public PhysicMaterial physicMaterial;   // 마찰력 0인 피지컬머티리얼 
    PhysicMaterial defaultMaterial;         // 원래 기본 플레이어 머티리얼 = none
    bool isESC = false;                     // 씬 전환 기능 여러번 눌러도 한번만 인식되게 !!
    bool isPlay = false;    // 완전 시작을 위한 변수

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        spotLight.intensity = 0;
        materials = mesh.sharedMaterials;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        maskLight.SetActive(false);
        defaultMaterial = GetComponentInChildren<MeshCollider>().material;
    }

    void Update()
    {
        if (isStart)
        {   // 처음 시작 시 화면이 완전히 밝아지고 나서 중력 및 이동 가능
            rigid.useGravity = true;
            gameObject.GetComponent<PlayerMove>().enabled = true;
            UIManager.instnace.RunAnims("isMove");
            isStart = false;
            isPlay = true;
        }
        if (!isPlay) return;

        if (!isESC && Input.GetKeyDown(KeyCode.Escape))
        {
            isESC = true;
            ScenesManager.instance.Scene[0] = true;
            SoundManager.instnace.VolumeOutBGM();
        }

        if (isStop)
        {
            animator.SetBool("isWalk", false);
            return;
        }
        if(rigid.useGravity) rigid.AddForce(Vector3.down * 1.8f); // 즁력 더 주기 
        badak = Physics.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        if (isGround && badak) isJump = false; // 바닥에 닿아있으면 점프중이 아님

        if (!badak) isJump = true;
    }

    private void FixedUpdate()
    {
        if (isStop) return;
        
        if (dir != Vector3.zero)
        {   // 키 입력이 들어왔으면 ~
            rigid.MovePosition(transform.position + dir * Time.deltaTime * speed);

            if (!isJump) animator.SetBool("isWalk", true);
            else if (isJump) animator.SetBool("isWalk", false);

            // 바라보는 방향 부호 != 가고자할 방향 부호
            if (Mathf.Sign(transform.forward.x) != Mathf.Sign(dir.x) )//|| Mathf.Sign(transform.forward.z) != Mathf.Sign(dir.z))
            {
                transform.Rotate(0, 1, 0);
            }
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        else animator.SetBool("isWalk", false);
    }

    public void OnMove(InputAction.CallbackContext state)
    {
        if (!rigid.useGravity) return;
        dir = state.ReadValue<Vector3>();
    }

    public void OnJump(InputAction.CallbackContext state)
    {
        if (isStop || !rigid.useGravity) return;

        if (state.performed && isJump == false)
        {
            animator.SetBool("isWalk", false);
            animator.SetTrigger("isJump");
            rigid.AddForce(Vector2.up * JumpForce, ForceMode.Impulse);
            SoundManager.instnace.PlaySE("PlayerJump", 0.35f);
        }
    }

    public void OnLight(InputAction.CallbackContext state)
    {
        if (isStop || !rigid.useGravity) return;

        if (state.performed)
        {   // GetKeyDown
            SoundManager.instnace.PlaySE("Light", 0.55f);
            lightOn = true;
            LightHandle();
        }
        else if(state.canceled)
        {   // GetKeyUp
            lightOn = false;
            LightHandle();
        }
    }

    public void LightHandle()
    { // L 누르면 빛 기본값으로 켜짐 다시 누르면 빛 꺼짐 
        maskLight.SetActive(lightOn);
        ResetLight();
    }

    public void ResetLight()
    {   // 발광 디폴트로 바꾸기 
        if (lightOn)
        {   //emission color의 밝기 1.5배 증가 시키기 
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f)*1.8f);
            spotLight.intensity = 25f;
        }

        else
        {
            materials[1].SetColor("_EmissionColor", new Color(0.8f, 0.85f, 0.9f));
            spotLight.intensity = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGround = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !badak)
        {   // 안 밟고 있는데 벽과 충돌해있으면 떨어지게 하자 
            gameObject.GetComponentInChildren<MeshCollider>().material = physicMaterial;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = defaultMaterial;
            isGround = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            transform.position = startPos.transform.position;
        }
        if (other.CompareTag("SavePoint")) startPos = other.gameObject;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossMove : MonoBehaviour
{
    Rigidbody rigid;
    InfoFish boss;
    Transform respawnSpot, target;
    SkinnedMeshRenderer skin;

    bool isDisapear = false;
    //Material[] mat;

    bool isStop, isMileStone2;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        
    }

    private void Start()
    {
        boss = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss;
        respawnSpot = GameObject.Find("BossSpawn").transform;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //mat = skin.materials;
    }

    private void Update()
    {
        if (isDisapear) MatOut();

        if (!isStop && !SwimMove.isEnd) ChaseTarget();  // 플레이어가 해파리를 먹는 과정
        else if (!isMileStone2 && SwimMove.isEnd)  
        {   // 보스가 아직 milestone에 도달하지 못했는데 플레이어는 dollytrack탑승중이라면,
            //Debug.Log("else if");
            rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * 30f);
        }
        else if(isStop) rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * 0f);
        else if(isMileStone2)
        {
            //Debug.Log("else");
            rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * boss.speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            Debug.Log("BossDied");
            GameObject.Find("whole_cave").GetComponent<FractureObject>().Explode();
            isStop = true;
            boss.speed = 0f;
            rigid.useGravity = true;
            Invoke("BossDestroy", 3f);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke("ResetBoss", 1.5f);
        }
    }

    void ChaseTarget()
    {
        if(Vector3.Distance(transform.position, target.position) > 10f)
        {
            boss.speed = 15f;
        }
        else
        {
            boss.speed = 5f;
        }
        transform.position = Vector3.MoveTowards(transform.position, target.position, boss.speed * Time.deltaTime);
    }

    void ResetBoss()
    {
        gameObject.SetActive(false);
        transform.position = respawnSpot.position;
    }

    void BossDestroy()
    {
        SwimMove.isEnd = false;
        SwimMove.isBoss = false;
        CinematicBar.instance.HideBars();
        GameObject.Find("player").GetComponent<CinemachineDollyCart>().enabled = false;
        isDisapear = true;
        Destroy(this.gameObject, 1.25f);
    }

    public float animTime = 1f;         // Fade 애니메이션 재생 시간 (단위:초).    

    private float start = 1f;           // Mathf.Lerp 메소드의 첫번째 값.  
    private float end = 0f;             // Mathf.Lerp 메소드의 두번째 값.  
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.  

    void MatOut()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        // 컴포넌트의 색상 값 읽어오기.
        foreach (var item in skin.materials)
        {
            Color color = item.color;
            // 알파 값 계산.  
            color.a = Mathf.Lerp(start, end, time);
            // 계산한 알파 값 다시 설정.  
            item.color = color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            Debug.Log("End");
            isMileStone2 = true;
            boss.speed = 0f;
        }
    }
}

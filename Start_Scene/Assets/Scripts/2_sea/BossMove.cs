using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossMove : MonoBehaviour
{
    Rigidbody rigid;
    InfoFish boss;
    Transform respawnSpot, target;
    SkinnedMeshRenderer mesh;
    Material[] mat;
    Animator anim;
    bool isStop, isMileStone2;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        mat = mesh.materials;
        boss = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss;
        respawnSpot = GameObject.Find("BossSpawn").transform;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if (!isStop && !SwimMove.isEnd) ChaseTarget();  // 플레이어가 해파리를 먹는 과정
        else if (!isMileStone2 && SwimMove.isEnd)
        {   // 보스가 아직 milestone에 도달하지 못했는데 플레이어는 dollytrack탑승중이라면,
            Move(30f);
        }
        else if (isStop) Move(0);
        else if (isMileStone2) Move(boss.speed);
    }

    void Move(float speed)
    {
        // chase -> 30 -> 0 -> 50 -> 0 -> destroy
        rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * speed);
    }

    void ChaseTarget()
    {
        if (Vector3.Distance(transform.position, target.position) > 25f)
        {
            boss.speed = 25f;
        }
        else
        {
            boss.speed = 11.5f;
        }
        transform.position = Vector3.MoveTowards(transform.position, target.position, boss.speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            isMileStone2 = true;
            boss.speed = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            GameObject.Find("whole_cave").GetComponent<FractureObject>().Explode();
            GameObject.Find("MileStone 2").GetComponent<BoxCollider>().isTrigger = false;
            rigid.constraints = RigidbodyConstraints.None;
            Destroy(anim);  // 헤엄치는 애니메이션 죽으면 없애기
            isStop = true;
            rigid.useGravity = true;
            StartCoroutine(FadeInBoss());   // boss죽었을때 fade in(스르륵 사라지기)
        }
        if (!SwimMove.isEnd && collision.gameObject.CompareTag("Player"))
        {
            if (!SwimMove.isDied)
            {   // 죽었을 때 한번씩만 해파리 리스폰!
                // -> 안하면 보스랑 여러번 충돌나서 여러번 만들어지고 플레이어가 자유롭게 이동가능한 버그 생김
                SwimMove.isDied = true;
                GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().DestroyJellyFish();
            }
            Invoke("ResetBoss", 1.5f);
        }
    }

    void ResetBoss()
    {
        gameObject.SetActive(false);
        transform.position = respawnSpot.position;
        GetComponent<BossMove>().enabled = false;   // 움직이는 코드도 없애놔야함
    }

    void BossDestroy()
    {
        SwimMove.isEnd = false;
        SwimMove.isBoss = false;
        CinematicBar.instance.HideBars();
        GameObject.Find("player").GetComponent<CinemachineDollyCart>().enabled = false;
        UIManager.instnace.RunAnims("isWASD");  // 보스가 벽에 부딪히고 플레이어 조작키 안내 한번 더
        Destroy(this.gameObject);
    }
    IEnumerator FadeInBoss()
    {
        yield return new WaitForSeconds(2f);
        
        Color teeth = mat[0].color;
        Color body = mat[1].color;
        Color light = mat[2].color;
        float time = Time.deltaTime;
        Color lightEmission = mat[2].GetColor("_EmissionColor");
        mat[2].SetColor("_EmissionColor", lightEmission * 0f);
        while (true)
        {
            teeth.a = Mathf.Lerp(teeth.a, 0, time);
            mat[0].color = teeth;

            body.a = Mathf.Lerp(body.a, 0, time);
            mat[1].color = body;

            light.a = Mathf.Lerp(light.a, 0, time);
            mat[2].SetColor("_EmissionColor", lightEmission * 0f);
            mat[2].color = light;

            if (mat[1].color.a < 0.05f) break;
            yield return new WaitForSeconds(0.01f);
        }
        Invoke("BossDestroy", 1f);
        yield return null;
    }
    private void OnDestroy()
    {
        Color teeth = mat[0].color;
        Color body = mat[1].color;
        Color light = mat[2].color;

        teeth.a = 1f;
        body.a = 1f;
        light.a = 1f;

        mat[0].color = teeth;
        mat[1].color = body;
        mat[2].color = light;
    }
}

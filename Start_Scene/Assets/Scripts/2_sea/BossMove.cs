using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossMove : MonoBehaviour
{
    Rigidbody rigid;
    InfoFish boss;
    Transform respawnSpot, target;
    bool isStop, isMileStone2;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
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
        Debug.Log(speed);
        rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * speed);
    }

    void ChaseTarget()
    {
        if (Vector3.Distance(transform.position, target.position) > 20f)
        {
            boss.speed = 20f;
        }
        else
        {
            boss.speed = 10.5f;
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
            Debug.Log("BossDied");
            GameObject.Find("whole_cave").GetComponent<FractureObject>().Explode();
            isStop = true;
            rigid.useGravity = true;
            Invoke("BossDestroy", 3f);
        }
        if (!SwimMove.isEnd && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("destroy Jelly");
            if (!SwimMove.isDied)
            {   // 죽었을 때 한번씩만 해파리 리스폰!
                // -> 안하면 보스랑 여러번 충돌나서 여러번 만들어지고 플레이어가 자유롭게 이동가능한 버그 생김
                GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().DestroyJellyFish();
                SwimMove.isDied = true;
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
        Destroy(this.gameObject, 1f);
    }
}

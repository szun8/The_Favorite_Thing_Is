using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    Rigidbody rigid;
    InfoFish boss;
    Transform respawnSpot;
    SkinnedMeshRenderer skin;
    Material[] mat;
    //Animator anim;

    bool isStop;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        //anim = GetComponent<Animator>();
    }

    private void Start()
    {
        boss = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss;
        respawnSpot = GameObject.Find("BossSpawn").transform;
        mat = skin.materials;
    }

    private void Update()
    {
        //if(!isStop)
          // rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * boss.speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            Debug.Log(collision.gameObject.name);
            GameObject.Find("whole_cave").GetComponent<FractureObject>().Explode();
            //anim.speed = 0.5f;
            boss.speed = 15f;
            rigid.useGravity = true;
            Invoke("BossDestroy", 4f);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke("ResetBoss", 1.5f);
        }
    }

    void ResetBoss()
    {
        gameObject.SetActive(false);
        transform.position = respawnSpot.position;
    }

    void BossDestroy()
    {
        isStop = true;
        SwimMove.isBoss = false;
        Destroy(this.gameObject);
    }
}

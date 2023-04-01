using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossMove : MonoBehaviour
{
    Rigidbody rigid;
    InfoFish boss;
    Transform respawnSpot;
    SkinnedMeshRenderer skin;

    //Material[] mat;

    bool isStop;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        
    }

    private void Start()
    {
        boss = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss;
        respawnSpot = GameObject.Find("BossSpawn").transform;
        //mat = skin.materials;
    }

    private void Update()
    {
        if (!isStop)
            rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * boss.speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            GameObject.Find("whole_cave").GetComponent<FractureObject>().Explode();
            isStop = true;
            boss.speed = 15f;
            rigid.useGravity = true;
            Invoke("BossDestroy", 3f);
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
        SwimMove.isEnd = false;
        SwimMove.isBoss = false;
        CinematicBar.instance.HideBars();
        GameObject.Find("player").GetComponent<CinemachineDollyCart>().enabled = false;
        // 여기서 SwithcingBossToCave Cam turn ON
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            boss.speed = 0f;
        }
    }
}

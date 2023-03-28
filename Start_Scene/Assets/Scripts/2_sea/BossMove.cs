using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    Rigidbody rigid;
    InfoFish boss;
    Transform respawnSpot;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        boss = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().Boss;
        respawnSpot = GameObject.Find("BossSpawn").transform;
    }

    private void Update()
    {
        rigid.MovePosition(transform.position + Vector3.right * Time.deltaTime * boss.speed);
    }

    void ResetBoss()
    {
        gameObject.SetActive(false);
        transform.position = respawnSpot.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stone"))
        {
            GameObject.Find("fractured_noRigid").GetComponent<FractureObject>().Explode();
            Destroy(this.gameObject);
        }
        if (collision.gameObject.CompareTag("Player_mesh"))
        {
            transform.position = respawnSpot.position;
            gameObject.SetActive(false);
        }
    }
}

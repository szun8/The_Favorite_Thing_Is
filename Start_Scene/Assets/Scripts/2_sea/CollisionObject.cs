using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionObject : MonoBehaviour
{
    public GameObject wall;
    SwimMove player;

    private void Awake()
    {
        player = transform.parent.GetComponent<SwimMove>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {   // 보스와 충돌시
            player.SetDeadState();
            wall.SetActive(true);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {   // 물고기 떼와 충돌(변경예정_0328.ver)
            SwimMove.isBoss = true;
            wall.SetActive(false);
            GameObject.FindGameObjectWithTag("Enemy").SetActive(true);
        }
    }
}

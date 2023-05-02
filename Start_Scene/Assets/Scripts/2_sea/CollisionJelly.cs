using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionJelly : MonoBehaviour
{
    SwimMove player;
    private void Awake()
    {
        player = GameObject.Find("player").GetComponent<SwimMove>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player_mesh"))
        {
            GetComponentInParent<JellyMove>().isColl = true;
            player.isJelly = true;
            player.SpeedUP();
        }
    }
}

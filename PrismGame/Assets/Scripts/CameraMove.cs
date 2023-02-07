using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject player;
    public float dist = 7f;
    public float height = 5f;

    private void Awake()
    {
        
    }

    private void Update()
    {
       if(player != null)
        {
            transform.position = player.transform.position - (Vector3.forward * dist) + (Vector3.up * height);
            transform.LookAt(player.transform);
        } 
    }
}

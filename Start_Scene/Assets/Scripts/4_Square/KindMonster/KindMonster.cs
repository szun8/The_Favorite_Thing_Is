using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KindMonster : MonoBehaviourPun
{
    PhotonView PV;

    public Flowers flower;          //꽃의 isBloom 가져오기 위함 
    public KindManager kindManager; //isWalk bool 값 가져와서 이동하게 하려고 

    public bool isArrive = false;

    private Vector3 pos;
    private float speed = 2f;


    void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
        pos = transform.parent.position;

    }

    void Update()
    {
        // 꽃이 피면 + walk Anim이면  
        if (flower.isBloom && kindManager.isWalk) Move();

    }
    void Move()
    {
        if (!isArrive)
        {
            pos += Vector3.right * speed * Time.deltaTime;

            transform.parent.position = pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TurtleStop"))
        {
            isArrive = true;
        }
    }


}

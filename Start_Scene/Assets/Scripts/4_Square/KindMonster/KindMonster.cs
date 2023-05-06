using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KindMonster : MonoBehaviourPun
{
    PhotonView PV;

    public KindPlate kindPlate; //해당 G 단상 
    public Transform[] stop;    //거북이가 멈추는 곳
    
    private bool isMove = false;
    private bool front = false;
    private bool back = false;

    private Vector3 pos;
    private float speed = 3f;

    void Awake()
    {
        pos = transform.position;
    }

    void Update()
    {
        MoveTurtle();   
    }

    

    void Direction()
    {
        if (!kindPlate.isgreen)
        {
            back = isMove = true;
            front = false;
        }

        else if (kindPlate.isgreen)
        {
            front = isMove = true;
            back = false;
        }
        else
        {
            back = front = isMove = false;
        }
    }

    void MoveTurtle()
    {
        Direction();
        if (isMove)
        { 
            if (back && pos.x < stop[0].position.x)
                pos += Vector3.right * speed * Time.deltaTime;
            else if (front && transform.position.x > stop[1].position.x) pos -= Vector3.right * speed * Time.deltaTime;

            transform.position = pos;
        }
    }




}

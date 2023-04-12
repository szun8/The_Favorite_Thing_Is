using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KindMonster : MonoBehaviour
{
    public KindPlate kindPlate;

    public Transform[] stop;
    
    private bool isMove = false;
    private bool front = false;
    private bool back = false;

    private Vector3 pos;
    private float speed = 3f;

    void Awake()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTurtle();   
    }

    

    void Direction()
    {
        if (kindPlate.redObj)
        {
            back = isMove = true;
            front = false;
        }

        else if (kindPlate.greenObj)
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

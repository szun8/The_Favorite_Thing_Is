using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingStone : MonoBehaviour
{

    public float speed = 2.5f;
    bool far = true;

    void Update()
    {
        if (far)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 20, transform.position.z), speed * Time.deltaTime);
            Moving();
        }
        else if (!far)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 6, transform.position.z), speed * Time.deltaTime);
            Moving();
        }


    }

    bool Moving()
    {
        if (transform.position.y == 20)
        {
            far = false;
        }
        else if (transform.position.y == 6)
        {
            far = true;
        }
        return far;
    }
}

//마찰력이 없어서 발판 따라서 내가 움직여죠야해
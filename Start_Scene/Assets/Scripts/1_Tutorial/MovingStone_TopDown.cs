using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingStone_TopDown : MonoBehaviour
{
    Vector3 pos;
    public float speed = 2.5f;
    bool far = true;

    private void Awake()
    {
        pos = transform.position;
    }

    void Update()
    {
        Moving();
        if (far) 
        {
            pos = Vector3.MoveTowards(pos, new Vector3(pos.x, 15, pos.z), speed * Time.deltaTime);

        }
        else 
        {
            pos = Vector3.MoveTowards(pos, new Vector3(pos.x, 45, pos.z), speed * Time.deltaTime);
        }
        transform.position = pos;
    }

    void Moving()
    {
        if (transform.position.y <= 15 && far)
        {
            far = false;
        }
        else if (transform.position.y >= 45 && !far)
        {
            far = true;
        }
    }
}

//마찰력이 없어서 발판 따라서 내가 움직여죠야해
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seahorse : MonoBehaviour
{
    Quaternion originRotation;

    void Start()
    {
        originRotation = transform.rotation;
    }

    void Update()
    {
        if (SwimMove.isBoss)
        {
            if(gameObject.name == "0508_seahorse_left")
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0,-180,0), Time.deltaTime * 1.5f);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * 1.5f);
        }
        if (SwimMove.isDied)
        {
            transform.rotation = originRotation;
        }
    }

    
}

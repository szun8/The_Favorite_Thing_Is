using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingStone_Beside : MonoBehaviour
{
    public float speed = 2.5f;
    Vector3 pos;
    bool far = true;

    private void Awake()
    {
        pos = transform.localPosition;
    }

    void Update()
    {
        if (far)
        {
            pos = Vector3.MoveTowards(pos, new Vector3(pos.x, pos.y, 38), speed * Time.deltaTime);
        }
        else
        {
            pos = Vector3.MoveTowards(pos, new Vector3(pos.x, pos.y, 57), speed * Time.deltaTime);
        }
        // Update position of the object
        transform.localPosition = pos;

        // Check if the object has reached its target position
        if (transform.localPosition.z == 38 && far)
        {
            far = false;
        }
        else if (transform.localPosition.z == 57 && !far)
        {
            far = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    Rigidbody rigid;
    MeshCollider coll;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<MeshCollider>();
    }

    void Start()
    {
        rigid.velocity = Vector3.down * Random.Range(50f, 75f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            if (transform.parent.name == "DropBackArea")
                Destroy(this.gameObject);
            else
            {
                Destroy(rigid);
                coll.isTrigger = false;
                DropObject.isDroped = true;
            }
        }
    }
}

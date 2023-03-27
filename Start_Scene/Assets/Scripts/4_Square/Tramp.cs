using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tramp : MonoBehaviour
{
    public int jumpforce = 12;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.y <= 0f)
        {
            Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                Vector3 velocity = rigidbody.velocity;
                velocity.y = jumpforce;
                rigidbody.velocity = velocity;
            }
        }
    }

    
}

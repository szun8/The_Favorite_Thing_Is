using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PushingWall : MonoBehaviourPunCallbacks
{
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
        if (collision.gameObject.GetComponent<PhotonView>().ViewID == 1001)
        {
            transform.position += new Vector3(0.2f, 0, 0);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<PhotonView>().ViewID == 1001)
        {
            transform.position += new Vector3(0.1f, 0, 0);
        }
    }
}

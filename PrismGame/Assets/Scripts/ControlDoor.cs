using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControlDoor : MonoBehaviourPunCallbacks
{
    Animator Anim;
    BoxCollider Coll;
    PhotonView PV;
    Vector3 upY = new Vector3(0f, 0.5f, 0f);

    public bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        Coll = GetComponent<BoxCollider>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;
        if (isOpen)
        {
            StartCoroutine(OpenDoor());
            isOpen = false;
        }
    }

    IEnumerator OpenDoor()
    {
        Anim.SetBool("isOpen", true);
        while (Coll.center.y < 6)
        {
            Coll.center += upY;
            yield return new WaitForSeconds(3f);
        }
        Coll.isTrigger = true;
        yield break;
    }
}

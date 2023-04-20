using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class KungKung : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    private Material material;
    public Material sleep, wake;
    //private Animator animator;

    public GameObject kung;
    private bool isDrop = false;
    private bool isPlayerIn = false;

    public float speed = 4f;

    Vector3 startPos;
    Vector3 pos;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        //kung = transform.GetChild(0).gameObject;

        if (kung != null)
        {
            startPos = kung.transform.position;
            pos = kung.transform.position;

            material = kung.GetComponent<MeshRenderer>().sharedMaterial;       
            //animator = kung.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if(PhotonNetwork.InRoom) StartCoroutine("KungKungManager");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_mesh"))
        {
            PV.RPC("SyncIsDrop", RpcTarget.AllBuffered, true);
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player_mesh"))
        {
            PV.RPC("SyncIsDrop", RpcTarget.AllBuffered, false);
            isPlayerIn = false;
        }
    }

    IEnumerator KungKungManager()
    {
        if (isDrop)
        {
            kung.transform.position = pos;
            PV.RPC("SyncKung", RpcTarget.AllBuffered,true);
            if (kung.CompareTag("UpKung"))
            {
                pos = Vector3.MoveTowards(pos, new Vector3(pos.x, 0, pos.z), speed * 4f * Time.deltaTime);
                if (!isPlayerIn) PV.RPC("SyncIsDrop", RpcTarget.AllBuffered, false);
            }
            else if (kung.CompareTag("DownKung"))
            {
                pos = Vector3.MoveTowards(pos, new Vector3(pos.x, 5.4f, pos.z), speed * 4f * Time.deltaTime);
                if (!isPlayerIn) PV.RPC("SyncIsDrop", RpcTarget.AllBuffered, false);
            }
            
        }

        else
        {
            kung.transform.position = pos;
            PV.RPC("SyncKung", RpcTarget.AllBuffered, false);
            pos = Vector3.MoveTowards(pos, startPos, speed * 1.5f * Time.deltaTime);
            yield return null;
        }
        
    }
    [PunRPC]
    void SyncKung(bool value)
    {
        if (value)
        {
            //animator.SetBool("isWake", value);
            material = wake;
            kung.GetComponent<MeshRenderer>().sharedMaterial = material;
        }
        else
        {
            //animator.SetBool("isWake", value);
            material = sleep;
            kung.GetComponent<MeshRenderer>().sharedMaterial = material;
        }
        
    }

    [PunRPC]
    void SyncIsDrop(bool value)
    {
        isDrop = value;
    }

}

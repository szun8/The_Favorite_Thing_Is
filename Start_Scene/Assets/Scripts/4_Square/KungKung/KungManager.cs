using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class KungManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    private Material material;
    public Material sleep, wake;

    public GameObject kung;
    public GameObject plate;

    private bool isDrop = false;
    private bool isPlayerIn = false;

    public float speed = 4f;

    Vector3 startPos;
    Vector3 pos;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        if (kung != null)
        {
            startPos = kung.transform.position;
            pos = kung.transform.position;

            material = kung.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }

    private void Update()
    {
        if(PhotonNetwork.InRoom && kung != null) StartCoroutine("KungKungManager");

        if (kung == null)
        {
            PV.RPC("SyncMat", RpcTarget.AllBuffered);

            if (material.GetFloat("_SplitValue") == 1)
                PV.RPC("Die", RpcTarget.AllBuffered);
        }

    }

    private void OnTriggerEnter(Collider other) //플레이어가 영역 들어오면 떨어진다 
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

    IEnumerator KungKungManager()   //쿵쿵이 윗세계 아랫세계에 따라서 위아래 와따가따 하게 하기 
    {
        if (isDrop)
        {
            kung.transform.position = pos;
            PV.RPC("SyncKung", RpcTarget.AllBuffered, true);
            if (kung.CompareTag("UpKung"))
            {
                pos = Vector3.MoveTowards(pos, new Vector3(pos.x, 0, pos.z), speed * 4f * Time.deltaTime);
                if (!isPlayerIn) PV.RPC("SyncIsDrop", RpcTarget.AllBuffered, false);
            }
            else if (kung.CompareTag("DownKung"))
            {
                pos = Vector3.MoveTowards(pos, new Vector3(pos.x, -0.6f, pos.z), speed * 4f * Time.deltaTime);
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
    //쿵쿵이의 눈뜨고 잠자게하는거 동기화 
    [PunRPC]
    void SyncKung(bool value)
    {
        if(kung != null)
        {
            if (value)
            {
                material = wake;
                //if (material.GetFloat("_SplitValue") <= 1) material.SetFloat("_SplitValue", 1);
                kung.GetComponent<MeshRenderer>().sharedMaterial = material;
            }
            else
            {
                material = sleep;
                kung.GetComponent<MeshRenderer>().sharedMaterial = material;
            }
        }
        
        
    }

    [PunRPC]
    void SyncIsDrop(bool value)
    {
        if(value) SoundManager.instnace.PlaySE("KungKung", 0.75f);   // 쿵쿵이가 내려올때만 사운드 On
        isDrop = value;
    }

    [PunRPC]
    void Die() => Destroy(gameObject);  //쿵쿵이의 감지 범위 트리거 삭제 

    [PunRPC]
    void SyncMat() => material.SetFloat("_SplitValue", 1);  


}

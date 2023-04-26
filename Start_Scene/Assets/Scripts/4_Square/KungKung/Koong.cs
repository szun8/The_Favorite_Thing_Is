using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Koong : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    public bool dieReady = false;

    public KungDanSang kungPlate;

    Material material;


    void Awake()
    {
        PV = GetComponent<PhotonView>();
        
    }

    private void Update()
    {
        if (kungPlate.isLight && dieReady)
        {
            PV.RPC("Dissolve", RpcTarget.AllBuffered);

            if(material.GetFloat("_SplitValue") <= 0.01) PV.RPC("DieKung", RpcTarget.AllBuffered);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MonsterDead")) PV.RPC("SyncDie", RpcTarget.AllBuffered, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MonsterDead")) PV.RPC("SyncDie", RpcTarget.AllBuffered, false);
    }


    [PunRPC]
    void SyncDie(bool value) => dieReady = value;

    [PunRPC]
    void Dissolve()
    {
        material = GetComponent<MeshRenderer>().sharedMaterial;

        material.SetFloat("_SplitValue", Mathf.Lerp(material.GetFloat("_SplitValue"), 0, Time.deltaTime * 2f));
    }

    [PunRPC]
    void DieKung() => Destroy(gameObject);    

}

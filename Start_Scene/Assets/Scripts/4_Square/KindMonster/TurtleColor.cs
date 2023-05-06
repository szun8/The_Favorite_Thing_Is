using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurtleColor : MonoBehaviourPun
{
    public Material dark, green;
    public GameObject G_Lamp;

    private SkinnedMeshRenderer mesh;
    private PhotonView PV;

    private bool isDone = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        mesh.material = dark;
    }

    void Update()
    {
        if (!isDone && G_Lamp.GetComponent<RGBLamp>().isColor)
        {
            PV.RPC("SyncMat", RpcTarget.AllBuffered);
            isDone = true;
        }
    }

    [PunRPC]
    void SyncMat() => mesh.material = green;
}

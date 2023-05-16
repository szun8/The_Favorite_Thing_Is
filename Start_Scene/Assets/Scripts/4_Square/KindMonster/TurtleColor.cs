using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
public class TurtleColor : MonoBehaviour //플레이어가 G 백색광 먹으면 스킨이 검정 - 초록 바뀜 
{
    public Material dark, green;
    public GameObject G_Lamp;

    private SkinnedMeshRenderer mesh;


    private bool isDone = false;

    void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        mesh.material = dark;
    }

    void Update()
    {
        if (!isDone && G_Lamp.GetComponent<RGBLamp>().isColor) //불이켜져야 바뀜 
        {
            mesh.material = green;
            isDone = true;
        }
    }

    //[PunRPC]
    //void SyncMat() => mesh.material = green;//green;
}

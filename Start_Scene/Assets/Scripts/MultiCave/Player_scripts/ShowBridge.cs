using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShowBridge : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    MeshRenderer MeshRenderer;
    MeshRenderer[] bridgeMesh;  //다리 하나하나의 mesh가져오기 
    CavePlayerMove cavePlayerMove;

    GameObject Bridge;
    
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        MeshRenderer = GetComponent<MeshRenderer>();
        Bridge = GameObject.FindGameObjectWithTag("Bridge");
        bridgeMesh = Bridge.GetComponentsInChildren<MeshRenderer>();
        cavePlayerMove = GetComponent<CavePlayerMove>();

    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {       //플레이어의 색에 따른 다리의 색 보이고 안보이고
            if (MeshRenderer.sharedMaterial == cavePlayerMove.material[1])
            {
                SetBridgeVisible(true);
            }
            else SetBridgeVisible(false);
        }
    }

    //다리의 색이 보일지 안보일지 Bridge 하위 다리들의 meshRenderer 접근 
    void SetBridgeVisible(bool visible)
    {
        for (int i = 0; i < bridgeMesh.Length; i++)
        {
            bridgeMesh[i].enabled = visible;
        }
    }
}

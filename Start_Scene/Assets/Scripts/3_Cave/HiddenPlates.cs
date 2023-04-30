using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPlates : MonoBehaviour
{
    [SerializeField] GameObject player;
    MeshCollider coll;
    CaveMove cm;

    private void Start()
    {
        coll = GetComponent<MeshCollider>();
        cm = player.GetComponent<CaveMove>();
    }

    void Update()
    {
        if (cm.lightOn )
        {
            coll.isTrigger = false;
        }
            
        else
        {
            coll.isTrigger = true;
        }
    }
}

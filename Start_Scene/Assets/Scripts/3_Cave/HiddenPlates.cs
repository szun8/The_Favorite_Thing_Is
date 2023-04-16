using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPlates : MonoBehaviour
{
    [SerializeField] GameObject player;
    MeshCollider coll;
    CaveMove cm;
    public static bool isSafe = false;

    private void Start()
    {
        coll = GetComponent<MeshCollider>();
        cm = player.GetComponent<CaveMove>();
    }

    void Update()
    {
        if (cm.lightOn)
        {
            coll.isTrigger = false;
            isSafe = true;
        }
            
        else
        {
            coll.isTrigger = true;
            isSafe = false;
        }
    }
}

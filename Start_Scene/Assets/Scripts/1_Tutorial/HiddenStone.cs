using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenStone : MonoBehaviour
{
    [SerializeField] PlayerMove player;
    MeshCollider coll;
    int lightCnt = 0;
    private void Awake()
    {
        coll = GetComponent<MeshCollider>();
    }

    private void Update()
    {
        if (player.lightOn)
        {
            coll.isTrigger = false;
            lightCnt = 1;
        }
        else if (!player.lightOn && lightCnt == 1)
        {
            coll.isTrigger = true;
            lightCnt = 0;
        }
    }
}

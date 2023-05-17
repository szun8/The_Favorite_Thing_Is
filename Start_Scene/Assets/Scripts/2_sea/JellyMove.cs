using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMove : MonoBehaviour
{
    public SkinnedMeshRenderer[] skinMat;
    public bool isColl = false;

    void Awake()
    {
        skinMat = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (isColl)
        {
            Destroy(this.gameObject);
        }
    }
}
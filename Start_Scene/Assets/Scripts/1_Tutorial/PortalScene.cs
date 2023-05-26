using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScene : MonoBehaviour
{
    BoxCollider coll;
    private void Start()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_mesh"))
        {
            ScenesManager.instance.Scene[0] = true;
            SoundManager.instnace.PlaySE("Portal", 0.15f);
            SoundManager.instnace.VolumeOutBGM();
            Destroy(coll);
        }
    }
}

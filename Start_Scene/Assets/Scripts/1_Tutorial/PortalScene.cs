using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScene : MonoBehaviour
{
    [SerializeField] Renderer portalMat;
    bool isConvert = false;

    void Update()
    {
        if (isConvert)
        {
            portalMat.material.SetFloat("_SplitValue", Mathf.Lerp(portalMat.material.GetFloat("_SplitValue"), 0, Time.deltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_mesh"))
        {
            isConvert = true;
            ScenesManager.instance.Scene[0] = true;
            SoundManager.instnace.VolumeOutBGM();
        }
    }
}

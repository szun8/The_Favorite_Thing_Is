using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeadStone : MonoBehaviour
{
    [SerializeField] CinemachineCollisionImpulseSource cinemachineCollision;

    public void InvokeNextScene()
    {
        Invoke("NextScene", 2f);
    }

    void NextScene()
    {
        ScenesManager.instance.Scene[ScenesManager.instance.SceneNum] = true;
    }

    public void SettingCinemachine()
    {
        SoundManager.instnace.PlaySE("CaveCollapse", 0.85f);
        CinematicBar.instance.ShowBars();
        Invoke("DestroyCine", 2f);
    }

    void DestroyCine()
    {
        CinematicBar.instance.HideBars();
        Destroy(cinemachineCollision);
    }
}

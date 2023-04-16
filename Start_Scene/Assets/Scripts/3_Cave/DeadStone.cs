using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeadStone : MonoBehaviour
{
    [SerializeField] CinemachineCollisionImpulseSource cinemachineCollision;
    public void SettingCinemachine()
    {
        SoundManager.instnace.PlaySE("CaveCollapse");
        CinematicBar.instance.ShowBars();
        Invoke("DestroyCine", 2f);
    }

    void DestroyCine()
    {
        CinematicBar.instance.HideBars();
        Destroy(cinemachineCollision);
    }
}

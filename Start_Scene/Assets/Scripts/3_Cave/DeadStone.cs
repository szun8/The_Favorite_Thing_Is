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
        Invoke("DestroyCine", 2.5f);
    }

    void DestroyCine()
    {
        Destroy(cinemachineCollision);
    }
}

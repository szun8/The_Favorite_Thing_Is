using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DeadStone : MonoBehaviour
{
    [SerializeField] CinemachineCollisionImpulseSource cinemachineCollision;
    CaveMove player;

    public void SettingDeadCoroutine()
    {
        StartCoroutine(SettingDead());
    }

    IEnumerator SettingDead()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CaveMove>();
        if (player.lightOn)
        {   // 죽는 시점에 플레이어의 발광구가 켜져있다면 끄는 기능
            player.lightOn = false;
            player.LightHandle();
        }
        player.enabled = false;

        SoundManager.instnace.PlaySE("CaveCollapse", 0.85f);
        CinematicBar.instance.ShowBars();
        yield return new WaitForSeconds(2f);

        CinematicBar.instance.HideBars();
        Destroy(cinemachineCollision);
        ScenesManager.instance.Scene[ScenesManager.instance.SceneNum] = true;
        yield return null;
    }
}

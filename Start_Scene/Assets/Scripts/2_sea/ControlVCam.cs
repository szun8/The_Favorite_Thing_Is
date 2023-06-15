using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ControlVCam : MonoBehaviour
{
    GameObject backCam;
    CinemachineVirtualCameraBase vBack;
    GameObject sideCam;
    CinemachineVirtualCameraBase vSide;
    GameObject watchingBossCam;
    CinemachineVirtualCameraBase vWatchingBoss;
    GameObject bossCam;
    CinemachineVirtualCameraBase vBoss;
    GameObject caveCam;
    CinemachineVirtualCameraBase vCave;

    [SerializeField] GameObject player;

    public Vector3 followOffset, trackedOffset;
    public bool isLerp = false;

    Vector3 originPos; // bossWatchingCam의 원위치

    #region Singleton
    static public ControlVCam instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }
    #endregion Singleton

    private void Start()
    {
        backCam = GameObject.Find("BackCam");
        vBack = backCam.GetComponent<CinemachineVirtualCameraBase>();

        sideCam = GameObject.Find("SideCam");
        vSide = sideCam.GetComponent<CinemachineVirtualCameraBase>();

        watchingBossCam = GameObject.Find("WatchingBossCam");
        vWatchingBoss = watchingBossCam.GetComponent<CinemachineVirtualCameraBase>();
        originPos = watchingBossCam.transform.position;

        bossCam = GameObject.Find("BossCam");
        vBoss = bossCam.GetComponent<CinemachineVirtualCameraBase>();

        caveCam = GameObject.Find("CaveCam");
        vCave = caveCam.GetComponent<CinemachineVirtualCameraBase>();
    }

    private void Update()
    {
        if (isLerp)
        {
            caveCam.transform.position = Vector3.Lerp(caveCam.transform.position, new Vector3(425f, caveCam.transform.position.y, 0f), Time.deltaTime * 0.3f) ;
        }
    }

    // 현재 진행 중인 캠의 우선순위를 원상복귀시키고 바뀔 캠의 우선순위화
    public void SwitchingSideToBack()
    {   // Side -> Back
        vBack.Priority = 11;
        vSide.Priority = 10;
    }

    public void SwitchingBackToSide()
    {   // Back -> Side
        player.GetComponent<SwimMove>().enabled = false;        // 플레이어 이동스크립트 비활성화
        player.GetComponent<PlayerInput>().enabled = false;     // 아예 키 인풋 비활성화
        player.GetComponent<Rigidbody>().useGravity = false;    // 플레이어 중력 비활성화로 가라앉는 것 막음

        vSide.Priority = 11;
        vBack.Priority = 10;
    }

    public void SwitchingWatchingBoss()
    {   // boss가 spawn된 시점에 비춰주는 코드
        vWatchingBoss.Priority = 11;
        vSide.Priority = 10;

        // 플레이어가 z축기준으로 가운데 위치하고 있지 않은 (이상) 경우를 대비해 z축 조정 코드
        StartCoroutine(LerpCam());  // 이후 보스 시점으로 전환 코루틴 호출
    }

    public void SwitchingWatchingBossToSide()
    {   // 추격전 시작
        vSide.Priority = 11;
        vWatchingBoss.Priority = 10;
    }

    public void SwitchingSideToBoss()
    {   // Side -> BossDolly
        vBoss.Priority = 11;
        vSide.Priority = 10;
    }

    [SerializeField] Transform bossSpawn, seahorse;

    IEnumerator LerpCam()
    {   // 보스 비추는 캠
        while (true)
        {
            watchingBossCam.transform.position = new Vector3(Mathf.Lerp(watchingBossCam.transform.position.x, bossSpawn.position.x, Time.deltaTime*2f), originPos.y, originPos.z); ;
            if(watchingBossCam.transform.position.x < (bossSpawn.position.x + 5f))
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }
        
        while (true)
        {
            watchingBossCam.transform.position = new Vector3(Mathf.Lerp(watchingBossCam.transform.position.x, seahorse.position.x, Time.deltaTime), originPos.y, originPos.z);
            if (watchingBossCam.transform.position.x < 160)
            {
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }
        player.transform.position = new Vector3(player.transform.position.x, 40f, 0f);
        SwitchingWatchingBossToSide();
        yield return new WaitForSeconds(1.5f);

        CinematicBar.instance.HideBars();
        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<PlayerInput>().enabled = true;
        player.GetComponent<SwimMove>().enabled = true;

        GameObject.Find("boss_0").GetComponent<BossMove>().enabled = true;

        watchingBossCam.transform.position = originPos;
        yield return null;
    }

    public void SwitchingBossToCave()
    {   // Boss -> Cave(Ending - LookAt : Player)
        vCave.Priority = 11;
        vBoss.Priority = 10;
        isLerp = true;
    }

    public void ControlDollyView()
    {
        //bossCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = followOffset;
        //bossCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = trackedOffset;
        StartCoroutine(ResetLookAtBoss());
    }

    IEnumerator ResetLookAtBoss()
    {
        yield return new WaitForSeconds(1f);
        vBoss.LookAt = null;
        //bossCam.transform.rotation = Quaternion.Euler(4, 95, 0);
    }
}

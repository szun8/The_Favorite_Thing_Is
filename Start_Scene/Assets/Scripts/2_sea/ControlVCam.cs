using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlVCam : MonoBehaviour
{
    GameObject backCam;
    CinemachineVirtualCameraBase vBack;
    GameObject sideCam;
    CinemachineVirtualCameraBase vSide;
    GameObject bossCam;
    CinemachineVirtualCameraBase vBoss;
    GameObject caveCam;
    CinemachineVirtualCameraBase vCave;

    public Vector3 followOffset, trackedOffset;
    public bool isLerp = false;

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

        vSide.Priority = 11;
        vBack.Priority = 10;
    }
    
    public void SwitchingSideToBoss()
    {   // Side -> BossDolly
        vBoss.Priority = 11;
        vSide.Priority = 10;
    }

    public void SwitchingBossToCave()
    {   // Boss -> Cave(Ending - LookAt : Player)
        vCave.Priority = 11;
        vBoss.Priority = 10;
        isLerp = true;
    }

    public void ControlDollyView()
    {
        bossCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = followOffset;
        bossCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = trackedOffset;
    }
}

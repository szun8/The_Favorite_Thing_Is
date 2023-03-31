using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlVCam : MonoBehaviour
{
    CinemachineBrain liveCam;

    GameObject backCam;
    CinemachineVirtualCameraBase vBack;
    GameObject sideCam;
    CinemachineVirtualCameraBase vSide;
    GameObject bossCam;
    CinemachineVirtualCameraBase vBoss;
    //GameObject caveCam;
    //CinemachineVirtualCameraBase vCave;

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
        liveCam = GetComponent<CinemachineBrain>();

        backCam = GameObject.Find("BackCam");
        vBack = backCam.GetComponent<CinemachineVirtualCameraBase>();

        sideCam = GameObject.Find("SideCam");
        vSide = sideCam.GetComponent<CinemachineVirtualCameraBase>();

        bossCam = GameObject.Find("BossCam");
        vBoss = bossCam.GetComponent<CinemachineVirtualCameraBase>();

        //caveCam = GameObject.Find("CaveCam");
        //vCave = cAveCam.GetComponent<CinemachineVirtualCameraBase>();

    }

    public void SwitchingSideToBack()
    {   // Side -> Back
        vBack.Priority = 11;
        vSide.Priority = 10;
        //backCam.SetActive(true);
        //sideCam.SetActive(false);
    }

    public void SwitchingBackToSide()
    {   // Back -> Side

        vSide.Priority = 11;
        vBack.Priority = 10;
        //sideCam.SetActive(true);
        //backCam.SetActive(false);
    }
    
    public void SwitchingSideToBoss()
    {   // Side -> BossDolly
        vBoss.Priority = 11;
        vSide.Priority = 10;
        //bossCam.SetActive(true);
        //sideCam.SetActive(false);
    }

    //public void SwitchingBossToCave()
    //{   // Boss -> Cave(Ending - LookAt : Player)
    //    caveCam.SetActive(true);
    //    bossCam.SetActive(false);
    //}
}

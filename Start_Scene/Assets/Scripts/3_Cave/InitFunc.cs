using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InitFunc : MonoBehaviour
{
    GameObject thornCam;
    CinemachineVirtualCameraBase vThorn;
    GameObject sideCam;
    CinemachineVirtualCameraBase vSide;
    GameObject backCam;
    CinemachineVirtualCameraBase vBack;

    [SerializeField] Transform startPos, endPos;
    bool isShow = false, isStart=false, isEnd=false;
    int i = 0;
    private void Start()
    {
        thornCam = GameObject.Find("ThornCam");
        vThorn = thornCam.GetComponent<CinemachineVirtualCameraBase>();

        sideCam = GameObject.Find("SideCam");
        vSide = sideCam.GetComponent<CinemachineVirtualCameraBase>();

        backCam = GameObject.Find("BackCam");
        vBack = backCam.GetComponent<CinemachineVirtualCameraBase>();
    }

    IEnumerator LerpCam()
    {
        while (true)
        {
            thornCam.transform.position = Vector3.Slerp(thornCam.transform.position, endPos.position, Time.deltaTime);
            if (thornCam.transform.position.x >= 360)
            {
                isEnd = true;
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }

        while (true)
        {
            thornCam.transform.position = Vector3.Slerp(thornCam.transform.position, startPos.position, Time.deltaTime);
            if (thornCam.transform.position.x < 300)
            {
                isStart = true;
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }
        SwithchSideCam();
        CinematicBar.instance.HideBars();
        GameObject.Find("player").GetComponent<CaveMove>().enabled = true;
        yield return null;
    }

    public void StartFunc()
    {
        UIManager.instnace.LightSubtitle();
    }

    public void SwitchThornCam()
    {
        if (i == 0)
        {
            ++i;
            vThorn.Priority = 11;
            vSide.Priority = 10;
            GameObject.Find("player").GetComponent<CaveMove>().enabled = false;
            CinematicBar.instance.ShowBars();
            StartCoroutine(LerpCam());
        }
    }

    void SwithchSideCam()
    {
        vSide.Priority = 11;
        vThorn.Priority = 10;
    }

    public void InvokeSwitchBackCam()
    {
        Invoke("SwitchBackCam", 3f);
    }

    void SwitchBackCam()
    {
        vBack.Priority = 11;
        vSide.Priority = 10;
        CaveMove.isMirror = true;   //거울룸에서는 백캠으로 좌우상하 이동가능
    }
}

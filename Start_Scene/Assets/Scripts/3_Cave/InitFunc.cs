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

    [SerializeField] Transform startPos, endPos;
    int i = 0;
    private void Start()
    {
        thornCam = GameObject.Find("ThornCam");
        vThorn = thornCam.GetComponent<CinemachineVirtualCameraBase>();

        sideCam = GameObject.Find("SideCam");
        vSide = sideCam.GetComponent<CinemachineVirtualCameraBase>();
    }

    IEnumerator LerpCam()
    {
        while (true)
        {   // 두번째 발판 지역을 비춰주는 시작 캠 (오른쪽 이동)
            thornCam.transform.position = Vector3.Slerp(thornCam.transform.position, endPos.position, Time.deltaTime*1.5f);
            if (thornCam.transform.position.x >= 360)
            {
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }

        while (true)
        {   // 두번째 발판 지역을 비춰준 뒤 플레이어에게 다시 돌아오는 (왼쪽 이동)
            thornCam.transform.position = Vector3.Slerp(thornCam.transform.position, startPos.position, Time.deltaTime*2f);
            if (thornCam.transform.position.x < 300)
            {
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }
        SwithchSideCam();   // 다시 플레이어 사이드뷰로 변	
        CinematicBar.instance.HideBars();
        GameObject.Find("player").GetComponent<CaveMove>().enabled = true;  // 캠 활성화동안 못움직이는거 다시 활성화
        yield return null;
    }

    public void SwitchThornCam()
    {
        if (i == 0)
        {
            ++i;    // 한번만 해당 캠 보여주기 위한 변수 (1번만 닿았을때 캠 진행, 이상닿으면 해당 조건문 진입 x)
            vThorn.Priority = 11;
            vSide.Priority = 10;
            GameObject.Find("player").GetComponent<CaveMove>().enabled = false; // 캠 활성화동안 플레이어 조작 비활성화
            CinematicBar.instance.ShowBars();
            StartCoroutine(LerpCam());
        }
    }

    void SwithchSideCam()
    {   // 기본 캠인 사이드뷰로 변경해주는 함수
        vSide.Priority = 11;
        vThorn.Priority = 10;
    }

    public void Anim_isL()
    {
        UIManager.instnace.RunAnims("isL");
    }

    public void FocusDeadPlayerCoroutine()
    {
        StartCoroutine(FocusDeadPlayer());
    }

    IEnumerator FocusDeadPlayer()
    {
        while (true)
        {
            sideCam.transform.rotation = Quaternion.Lerp(sideCam.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime*4f);
            vSide.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(vSide.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, new Vector3(4.5f, 2f, 4f), Time.deltaTime*5f);

            if (vSide.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x < 4.75f) break;
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlTitleCam : MonoBehaviour
{
    CinemachineVirtualCamera titleCam;
    public Vector3 followOffset, trackedOffset;
    Vector3 originFollowOffset, originTrackedOffset;

    bool isCam = false, isOrigin;
    void Start()
    {
        titleCam = GetComponent<CinemachineVirtualCamera>();
        originFollowOffset = titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        originTrackedOffset = titleCam.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset;
    }

    private void Update()
    {
        if (isCam)
        {
            titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
                = Vector3.Lerp(titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * 2f);
            titleCam.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset
                = Vector3.Lerp(titleCam.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset, trackedOffset, Time.deltaTime * 2f);
        }
        else if (isOrigin)
        {
            titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
                = Vector3.Lerp(titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, originFollowOffset, Time.deltaTime * 2f);
            titleCam.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset
                = Vector3.Lerp(titleCam.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset, originTrackedOffset, Time.deltaTime * 2f);
        }
    }

    public void StartTitle()
    {
        StartCoroutine(ControlTitle());
    }

    IEnumerator ControlTitle()
    {
        isCam = true;
        yield return new WaitForSeconds(1f);

        UIManager.instnace.RunAnims("isTitle");
        yield return new WaitForSeconds(2.5f);

        isCam = false;
        isOrigin = true;
        yield return new WaitForSeconds(2f);

        isOrigin = false;
    }
}

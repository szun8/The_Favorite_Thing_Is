using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlTitleCam : MonoBehaviour
{
    [SerializeField] Transform player;
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
            titleCam.transform.rotation = Quaternion.Lerp(titleCam.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 2f);
        }
    }

    public void StartTitle()
    {
        StartCoroutine(ControlTitle());
    }

    IEnumerator ControlTitle()
    {
        PlayerMove.isStop = true;
        player.rotation = Quaternion.Euler(0, 0, 0);
        titleCam.LookAt = player.transform;
        isCam = true;
        yield return new WaitForSeconds(1f);

        UIManager.instnace.RunAnims("isTitle");
        yield return new WaitForSeconds(4f);

        isCam = false;
        titleCam.LookAt = null;
        isOrigin = true;
        yield return new WaitForSeconds(2f);

        isOrigin = false;
        PlayerMove.isStop = false;
    }
}

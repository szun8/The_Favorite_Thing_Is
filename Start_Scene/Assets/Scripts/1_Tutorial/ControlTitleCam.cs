using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlTitleCam : MonoBehaviour
{
    [SerializeField] GameObject player;
    CinemachineVirtualCamera titleCam;
    public Vector3 followOffset;
    Vector3 originFollowOffset;

    bool isCam = false, isOrigin;
    void Start()
    {
        titleCam = GetComponent<CinemachineVirtualCamera>();
        originFollowOffset = titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    private void Update()
    {
        if (isCam)
        {
            titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
                = Vector3.Lerp(titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * 2f);
            titleCam.transform.rotation = Quaternion.Lerp(titleCam.transform.rotation, Quaternion.Euler(-11, 0, 0), Time.deltaTime * 2f);
        }
        else if (isOrigin)
        {
            titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
                = Vector3.Lerp(titleCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, originFollowOffset, Time.deltaTime * 2f);
            titleCam.transform.rotation = Quaternion.Lerp(titleCam.transform.rotation, Quaternion.Euler(0, 10, 0), Time.deltaTime * 2f);
        }
    }

    public void StartTitle()
    {
        StartCoroutine(ControlTitle());
    }

    IEnumerator ControlTitle()
    {
        PlayerMove.isStop = true;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (player.GetComponent<PlayerMove>().lightOn)
        {   // 타이틀이 뜰때는 발광 비활성화
            Debug.Log("light off");
            player.GetComponent<PlayerMove>().lightOn = false;
            player.GetComponent<PlayerMove>().LightHandle();
        }
        isCam = true;
        yield return new WaitForSeconds(1f);

        UIManager.instnace.RunAnims("isTitle");
        yield return new WaitForSeconds(4f);

        isCam = false;
        isOrigin = true;
        yield return new WaitForSeconds(2f);

        isOrigin = false;
        PlayerMove.isStop = false;
    }
}

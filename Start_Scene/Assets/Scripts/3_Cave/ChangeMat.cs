using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeMat : MonoBehaviour
{
    MeshRenderer wall;
    GameObject sideCam;
    CinemachineTransposer ct;

    [SerializeField] GameObject player;
    CaveMove cm;

    public Vector3 followOffset, originOffset;
    Color wallAlpha;

    bool isChange = false, isL = false;
    private void Awake()
    {
        wall = GetComponent<MeshRenderer>();
        sideCam = GameObject.Find("SideCam");
    }
    private void Start()
    {
        ct = sideCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        cm = player.GetComponent<CaveMove>();
        wallAlpha = wall.materials[1].color;
    }

    private void Update()
    {
        if (Input.GetKeyDown("l")) isL = true;
        if (isL && isChange)
        {   // 특정 지점에 닿았고 L 버튼도 켰다면,
            MatIn();
            if (wall.materials[1].color.a >= 0.99)
            {   // 벽화의 색이 일정 수준 다 보이게 되면 원상복귀
                CinematicBar.instance.HideBars();
                ct.m_FollowOffset.z = Mathf.Lerp(ct.m_FollowOffset.z, 15f, Time.deltaTime * 1.5f);
                sideCam.transform.rotation = Quaternion.Lerp(sideCam.transform.rotation, Quaternion.Euler(0f, 10f, 0), Time.deltaTime * 1.5f);

                if (ct.m_FollowOffset.z >= 14.5)
                {   // 카메라위치가 제자리로 돌아오면 플레이어 재시동
                    cm.enabled = true;
                    cm.animator.enabled = true;
                    isChange = false;
                    isL = false;
                    CaveMove.isStop = false;
                }
            }
            else
            {
                CinematicBar.instance.ShowBars();
                CamOnLerp();
            }
        }
    }
    public float animTime = 5f;         // Fade 애니메이션 재생 시간 (단위:초).   
    private float start = 1f;           // Mathf.Lerp 메소드의 첫번째 값.  
    private float end = 0f;             // Mathf.Lerp 메소드의 두번째 값.  
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.

    void CamOnLerp()
    {
        ct.m_FollowOffset.z = Mathf.Lerp(ct.m_FollowOffset.z, 5.5f, Time.deltaTime);

        sideCam.transform.rotation = Quaternion.Lerp(sideCam.transform.rotation, Quaternion.Euler(-10f, 20f, 0), Time.deltaTime);
        if (cm.lightOn)
        {
            cm.lightOn = false;
            cm.LightHandle();
        }
        cm.animator.enabled = false;
        cm.enabled = false;
    }

    void MatIn()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        Color color = wall.materials[1].color;
        // 알파 값 계산.  
        color.a = Mathf.Lerp(end, start, time);
        // 계산한 알파 값 다시 설정.  
        wall.materials[1].color = color;
    }

    public void ChangeMaterialWall()
    {   // 플레이어가 특정 벽화지점에 도착한 후 이동제어 비활성화 -> L 버튼 켜야 화면 조정 시작
        isChange = true;
        CaveMove.isStop = true;
    }
}

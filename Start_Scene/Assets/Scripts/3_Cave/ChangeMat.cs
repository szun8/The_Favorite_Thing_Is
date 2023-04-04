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
    }

    private void Update()
    {
        if (Input.GetKeyDown("l")) isL = true;
        if (isL && isChange)
        {   
            wall.material.color = Color.Lerp(wall.material.color, Color.white, Time.deltaTime);            
            if(wall.material.color.r >= 0.99 && wall.material.color.g >= 0.99 && wall.material.color.b >= 0.99)
            {   // 벽화의 색이 일정 수준 다 보이게 되면 원상복귀
                CinematicBar.instance.HideBars();
                ct.m_FollowOffset = Vector3.Lerp(ct.m_FollowOffset, originOffset, Time.deltaTime);
                if(ct.m_FollowOffset.x <= 0.5 && ct.m_FollowOffset.y <= 3.25 && ct.m_FollowOffset.z >= 8.5)
                {   // 카메라위치가 제자리로 돌아오면 플레이어 재시동
                    cm.enabled = true;
                    cm.animator.enabled = true;
                    isChange = false;
                    isL = false;
                }
            }
            else if (wall.material.color.r >= 0.1 && wall.material.color.g >= 0.1 && wall.material.color.b >= 0.1)
            {   // 벽화 앞에서 L버튼을 누르면 벽화의 색이 보이고 카메라 줌인, 플레이어 모든 행동 금지
                CinematicBar.instance.ShowBars();
                ct.m_FollowOffset = Vector3.Lerp(ct.m_FollowOffset, followOffset, Time.deltaTime);
                if (cm.lightOn)
                {
                    cm.lightOn = false;
                    cm.LightHandle();
                }
                cm.animator.enabled = false;
                cm.enabled = false;
            }
        }
    }

    public void ChangeMaterialWall()
    {
        isChange = true;
    }
}

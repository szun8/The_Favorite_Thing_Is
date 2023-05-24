using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StaineGlass : MonoBehaviourPun
{
    PhotonView PV;

    private Material[] Materials;
    private Material[] changeMat;

    public LastPlane p1;
    public LastPlane p2;

    public int light_state = 0; // L-> 0 L눌렀으면 1  Rgbcmyk 총 8단계

    public bool stopBG = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Materials = GetComponent<MeshRenderer>().materials;
        changeMat = Materials;
    }

    
    void Update()
    {
        if(light_state ==0 && p1.isLight[0] && p2.isLight[0]) //L
        {   
            ChangeColor(4); //default_M
            ChangeColor(9); //Light_Y
            
            Materials = changeMat;
            stopBG = true;

            if(SyncState(4) && SyncState(9)) 
            {   // 첫번째 L을 밝히는 작업이 끝나고 나서 실행되는 조건문
                UIManager.instnace.RunAnimsBool("isStainGlassY", false);   // 재인이의 요청사항 : 처음 L누르고 나서 UI 변경
                light_state = 1;
                stopBG = false;
            }
            
        }

        else if (light_state ==1 && p1.isLight[1] && p2.isLight[1]) //R
        {
            ChangeColor(6); //r

            Materials = changeMat;
            stopBG = true;

            if (SyncState(6))
            {
                light_state = 2;
                stopBG = false;
            }
        }
        else if (light_state == 2 && p1.isLight[2] && p2.isLight[2]) //G
        {
            ChangeColor(1); //g
            ChangeColor(0); //dark_g

            Materials = changeMat;
            stopBG = true;

            if (SyncState(1) && SyncState(0))
            {
                light_state = 3;
                stopBG = false;
            }
        }
        else if (light_state == 3 && p1.isLight[3] && p2.isLight[3]) //B
        {
            ChangeColor(3); //b

            Materials = changeMat;
            stopBG = true;

            if (SyncState(3))
            {
                light_state = 4;
                stopBG = false;
            }
        }
        else if (light_state == 4 && ((p1.isLight[2] && p2.isLight[3]) || (p1.isLight[3] && p2.isLight[2]) ) ) //C
        {
            ChangeColor(2); //c

            Materials = changeMat;
            stopBG = true;

            if (SyncState(2))
            {
                light_state = 5;
                stopBG = false;
            }
        }
        else if (light_state == 5 && ((p1.isLight[1] && p2.isLight[3]) || (p1.isLight[3] && p2.isLight[1]))) //M
        {
            ChangeColor(7); //m

            Materials = changeMat;
            stopBG = true;

            if (SyncState(7))
            {
                light_state = 6;
                stopBG = false;
            }
        }
        else if (light_state == 6 && ((p1.isLight[1] && p2.isLight[2]) || (p1.isLight[2] && p2.isLight[1]))) //Y
        {
            ChangeColor(5); //y
            ChangeColor(8); //mid_y

            Materials = changeMat;
            stopBG = true;

            if (SyncState(5) && SyncState(8))
            {
                light_state = 7;
                stopBG = false;
            }
        }
    }

    void ChangeColor(int value)
    {
        changeMat[value].color = Color.Lerp(changeMat[value].color,
                new Color(changeMat[value].color.r, changeMat[value].color.g, changeMat[value].color.b, 1), Time.deltaTime* 0.3f);  
    }

    bool SyncState(int m)
    {
        if(Materials[m].color.a  >= 0.65f) return true;
        else return false;
    }
}

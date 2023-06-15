using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StaineGlass : MonoBehaviourPun
{
    PhotonView PV;

    private Material[] Materials; //staineGlass 색깔들 
    private Material[] changeMat;

    public LastPlane p1;
    public LastPlane p2;

    public int light_state = 0; // L-> 0 L눌렀으면 1  Rgbcmyk 총 8단계

    public bool stopBG = false;

    public bool isClear = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        Materials = GetComponent<MeshRenderer>().materials;
        changeMat = Materials;
    }

    
    void Update()
    {
        ManageColor(); // 0.65 까진 플레이어가 알아서 색채우고 , light_state넘어가는데 넘어가면 알아서 alpha값 1 되게 했음 

        if(light_state == 0 && p1.isLight[0] && p2.isLight[0]) //L
        {
            isClear = true;

            if (isClear)
            {
                ChangeColor(4); //default_M
                ChangeColor(9); //Light_Y

                Materials = changeMat;
                stopBG = true;

                if (SyncState(4) && SyncState(9))
                {   // 첫번째 L을 밝히는 작업이 끝나고 나서 실행되는 조건문
                    UIManager.instnace.RunAnimsBool("isStainGlassY", false);   // 재인이의 요청사항 : 처음 L누르고 나서 UI 변경
                    light_state = 1;
                    stopBG = false;
                    isClear = false;
                }
            } 
        }

        else if (light_state ==1 && p1.isLight[1] && p2.isLight[1]) //R
        {
            isClear = true;

            if (isClear)
            {
                stopBG = true;
                ChangeColor(6); //r

                Materials = changeMat;
                

                if (SyncState(6))
                {
                    light_state = 2;
                    stopBG = false;
                    isClear = false;
                }
            }
        }
        else if (light_state == 2 && p1.isLight[2] && p2.isLight[2]) //G
        {
            isClear = true;

            if (isClear)
            {
                stopBG = true;
                ChangeColor(1); //g
                ChangeColor(0); //dark_g

                Materials = changeMat;
                
                if (SyncState(1) && SyncState(0))
                {
                    light_state = 3;
                    stopBG = false;
                    isClear = false;
                }
            }
        }
        else if (light_state == 3 && p1.isLight[3] && p2.isLight[3]) //B
        {
            isClear = true;

            if (isClear)
            {
                stopBG = true;
                ChangeColor(3); //b

                Materials = changeMat;
                

                if (SyncState(3))
                {
                    light_state = 4;
                    stopBG = false;
                    isClear = false;
                }
            }
        }
        else if (light_state == 4 && ((p1.isLight[2] && p2.isLight[3]) || (p1.isLight[3] && p2.isLight[2]) ) ) //C
        {
            isClear = true;
            

            if (isClear)
            {
                stopBG = true;
                ChangeColor(2); //c

                Materials = changeMat;
                

                if (SyncState(2))
                {
                    light_state = 5;
                    stopBG = false;
                    isClear = false;
                }
            }
        }
        else if (light_state == 5 && ((p1.isLight[1] && p2.isLight[3]) || (p1.isLight[3] && p2.isLight[1]))) //M
        {
            isClear = true;

            if (isClear)
            {
                stopBG = true;
                ChangeColor(7); //m

                Materials = changeMat;
                

                if (SyncState(7))
                {
                    light_state = 6;
                    stopBG = false;
                    isClear = false;
                }
            }
        }
        else if (light_state == 6 && ((p1.isLight[1] && p2.isLight[2]) || (p1.isLight[2] && p2.isLight[1]))) //Y
        {
            isClear = true;

            if (isClear)
            {
                ChangeColor(5); //y
                ChangeColor(8); //mid_y

                Materials = changeMat;
                stopBG = true;

                if (SyncState(5) && SyncState(8))
                {
                    light_state = 7;
                    stopBG = false;
                    isClear = false;
                }
            }
        }
    }

    void ChangeColor(int value)
    {
        changeMat[value].color = Color.Lerp(changeMat[value].color,
                new Color(changeMat[value].color.r, changeMat[value].color.g, changeMat[value].color.b, 1), Time.deltaTime * 0.3f);  
    }

    bool SyncState(int m) //플레이어가 빛을 어느정도 까지 채워야 하는지 
    {
        if (Materials[m].color.a >= 0.70f) return true;

        else return false;
    }

    bool ColorFull(int value) // 다 채우고 다음꺼 할 때 알아서 1로 채워짐 
    {
        if (changeMat[value].color.a >= 0.99f) return true;
        else return false;
    }

    void ManageColor()
    {
        if(light_state == 1 && !ColorFull(4) && !ColorFull(9))
        {
            ChangeColor(4);
            ChangeColor(9);
            Materials = changeMat;
        }

        else if (light_state == 2 && !ColorFull(6))
        {
            ChangeColor(6);
            Materials = changeMat;
        }

        else if (light_state == 3 && !ColorFull(1) && !ColorFull(0))
        {
            ChangeColor(1);
            ChangeColor(0);
            Materials = changeMat;
        }

        else if (light_state == 4 && !ColorFull(3))
        {
            ChangeColor(3);
            Materials = changeMat;
        }

        else if (light_state == 5 && !ColorFull(2))
        {
            ChangeColor(2);
            Materials = changeMat;
        }

        else if (light_state == 6 && !ColorFull(7))
        {
            ChangeColor(7);
            Materials = changeMat;
        }


    }
}

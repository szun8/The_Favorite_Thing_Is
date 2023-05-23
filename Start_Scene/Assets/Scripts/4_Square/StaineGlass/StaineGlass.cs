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
        if(light_state ==0 && (p1.isLight[0] || p2.isLight[0])) //L
        {   
            ChangeColor(4); //default_M
            ChangeColor(9); //Light_Y
            
            Materials = changeMat;
            stopBG = true;
            //if (Materials[4].color.a == 1 && Materials[9].color.a == 1) light_state=1;

            if(SyncState(4) && SyncState(9)) 
            {
                light_state = 1;
                stopBG = false;
            }
            
        }

        else if (light_state ==1 && (p1.isLight[1] || p2.isLight[1])) //R
        {

        }
        else if (light_state == 2 && p1.isLight[2] && p2.isLight[2]) //G
        {

        }
        else if (light_state == 3 && p1.isLight[3] && p2.isLight[3]) //B
        {

        }
        else if (light_state == 4 && ((p1.isLight[2] && p2.isLight[3]) || (p1.isLight[3] && p2.isLight[2]) ) ) //C
        {

        }
        else if (light_state == 5 && ((p1.isLight[1] && p2.isLight[3]) || (p1.isLight[3] && p2.isLight[1]))) //M
        {

        }
        else if (light_state == 6 && ((p1.isLight[1] && p2.isLight[2]) || (p1.isLight[2] && p2.isLight[1]))) //Y
        {

        }
    }

    void ChangeColor(int value)
    {
        changeMat[value].color = Color.Lerp(changeMat[value].color,
                new Color(changeMat[value].color.r, changeMat[value].color.g, changeMat[value].color.b, 1), Time.deltaTime* 0.3f);  
    }

    bool SyncState(int m)
    {
        if(Materials[m].color.a  >= 0.9f) return true;
        else return false;
    }
}

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
            changeMat[4].color = Color.Lerp(changeMat[4].color,
                new Color(changeMat[4].color.r, changeMat[4].color.g, changeMat[4].color.b, 1), Time.deltaTime);

            changeMat[9].color = Color.Lerp(changeMat[9].color,
                new Color(changeMat[9].color.r, changeMat[9].color.g, changeMat[9].color.b, 1), Time.deltaTime);

            Materials = changeMat;

            if (Materials[4].color.a == 1 && Materials[9].color.a == 1) light_state++;

        }

        else if (light_state ==1 && p1.isLight[1] && p2.isLight[1]) //R
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
}

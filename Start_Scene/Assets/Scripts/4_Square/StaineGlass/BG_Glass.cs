using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Glass : MonoBehaviour
{
    public StaineGlass staineGlass;

    private Material[] Materials;
    private Material[] changeMat;

    private float speed = 0.5f;

    void Awake()
    {
        Materials = GetComponent<MeshRenderer>().materials;
        changeMat = Materials;
    }

    void Update()
    {
        if (staineGlass.light_state == 1)
        {
            ChangeColor(6);
        }
        else if (staineGlass.light_state == 2)
        {
            // 다음 상태 처리
        }
    }

    void ChangeColor(int value)
    {
        if (!staineGlass.stopBG)
        {
            if(changeMat[value].color.r >= 0.9f)
            {
                changeMat[value].color = Color.Lerp(changeMat[value].color, new Color(168, 168, 168, 1), Time.deltaTime* 0.6f); 
            }

            else if(changeMat[value].color.r <= 168/255f)
            {
                changeMat[value].color = Color.Lerp(changeMat[value].color, new Color(255, 255, 255, 1), Time.deltaTime* 0.6f); 
            }

            Materials = changeMat;
        }
    }


    bool CheckLight(int m)
    {
        if(Materials[m].color.r  >= 0.99f) return true;
        else return false;
    }
}

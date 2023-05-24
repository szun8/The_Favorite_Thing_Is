using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Glass : MonoBehaviour
{
    public StaineGlass staineGlass;

    private Material[] Materials;


    private float speed = 0.5f;

    void Awake() =>  Materials = GetComponent<MeshRenderer>().materials;


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
            Color fadeColor = Materials[value].color;

            fadeColor.r = Mathf.PingPong(Time.time*0.5f, 0.8f) + 0.35f;
            fadeColor.g = fadeColor.r;
            fadeColor.b = fadeColor.r;

            Materials[value].color = fadeColor;

        }
    }

    bool CheckLight(int m)
    {
        if(Materials[m].color.r  >= 0.99f) return true;
        else return false;
    }
}

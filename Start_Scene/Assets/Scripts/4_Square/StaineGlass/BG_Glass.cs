using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Glass : MonoBehaviour
{
    public StaineGlass staineGlass;

    private Material[] Materials;


    private float speed = 0.6f;

    void Awake() =>  Materials = GetComponent<MeshRenderer>().materials;


    void Update()
    {
        if (staineGlass.light_state == 1)
        {
            ChangeColor(6); // r
        }
        else if (staineGlass.light_state == 2)
        {
            ChangeColor(1);//g
            ChangeColor(0);//dark_g
        }
        else if (staineGlass.light_state == 3)
        {
            ChangeColor(3);//b
        }
        else if (staineGlass.light_state == 4)
        {
            ChangeColor(2);//c
        }
        else if (staineGlass.light_state == 5)
        {
            ChangeColor(7);//m
        }
        else if (staineGlass.light_state == 6)
        {
            ChangeColor(5);//y
            ChangeColor(8);//mid_y
        }
    }

    void ChangeColor(int value)
    {
        if (!staineGlass.stopBG)
        {
            Color fadeColor = Materials[value].color;

            fadeColor.r = Mathf.PingPong(Time.time* speed, 0.8f) + 0.35f;
            fadeColor.g = fadeColor.r;
            fadeColor.b = fadeColor.r;

            Materials[value].color = fadeColor;

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleColor : MonoBehaviour //플레이어가 G 백색광 먹으면 스킨이 검정 - 초록 바뀜 
{
    public RGBLamp G_Lamp;
    public Animator animator;


    private bool isDone = false;

    void Update()
    {
        if (!isDone && G_Lamp.isColor) //불이켜져야 바뀜 
        {
            animator.SetBool("isLight", true);
            isDone = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //textmeshpro 쓸 때 


public class HelpTalk : MonoBehaviour
{
    
    public Transform Player;    //플레이어의 위치 
    public Transform startPos;  // 플레이어의 시작 위치

    private int count;

    Animator animator;
    TMP_Text tmp;
    
     
    void Awake()
    {
        animator = GetComponent<Animator>();
        tmp = GetComponent<TMP_Text>();
        
        count = 0;

        animator.SetBool("isMove", true);//처음 시작 시 나오는 이동 문구 

    }

    void Update()
    {
        if (startPos.position != Player.transform.position) animator.SetBool("isMove", false);

        Debug.DrawRay(Player.transform.position, Player.transform.forward * 2f, Color.yellow);
        
        bool talkJump = Physics.Raycast(Player.transform.position, Player.transform.forward, 2f, LayerMask.GetMask("Ground"));

        if (talkJump)
        {
            count = 1;
            ChangeTalk();
            animator.SetBool("isJump", true);
        }
    }

    
    // 점프 false 만들고 발광하세요랑 빛이 부족합니다만 만들면 댐 ~ 
    void ChangeTalk()
    {
        if (count == 0) tmp.text="화살표를 눌러 이동해보자";
        else if (count == 1) tmp.text= "Space로 점프해보자";
        else if (count == 2) tmp.text= "L 버튼으로 빛을 내보자";
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //textmeshpro 쓸 때 


public class HelpTalk : MonoBehaviour
{
    
    public Transform Player;    //플레이어의 위치
    public GameObject startPos;

    public int count = 0;
    private bool Showjump = false;

    Animator animator;
    TMP_Text tmp;
    
     
    void Awake()
    {
        animator = GetComponent<Animator>();
        tmp = GetComponent<TMP_Text>();

        ChangeTalk();
        animator.SetTrigger("isMove");
        

    }

    void Update()
    {
        

        Debug.DrawRay(Player.transform.position + new Vector3(0,-1f,0), Player.transform.forward * 2f, Color.yellow);
        
        bool talkJump = Physics.Raycast(Player.transform.position + new Vector3(0, -1f, 0), Player.transform.forward, 2f, LayerMask.GetMask("Ground"));

        if (talkJump && !Showjump)
        {
            count = 1;
            ChangeTalk();
            animator.SetTrigger("isJump");
            Showjump = true;    //한번만 보여줄거지롱 ~ 
        }
    }
    
    

    // 점프 false 만들고 발광하세요랑 빛이 부족합니다만 만들면 댐 ~ 
    void ChangeTalk()
    {
        if (count == 0) tmp.text= "←  →를 눌러 이동해보자";
        else if (count == 1) tmp.text= "Space로 점프해보자";
        else if (count == 2) tmp.text= "L 버튼으로 빛을 내보자";
    }

    
}

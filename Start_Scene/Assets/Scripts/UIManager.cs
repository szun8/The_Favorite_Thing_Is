using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPun
{
    #region Singleton
    static public UIManager instnace;
    void Awake()
    {
        if (instnace == null)
        {
            instnace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    Animator animator;
    public Image Image;            // Image컴포넌트 참조 변수.
    bool isLoad = false;

    void Start()
    {
        Image = GetComponentInChildren<Image>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // FadeIn 재생.  
        if (stopIn == false && time <= 1.5f)
        {
            PlayFadeIn();
        }
        if (stopOut == false && time <= 1.5f)
        {
            PlayFadeOut();
        }
        if (time >= 1.5f && stopIn == false)
        {
            stopIn = true;
            time = 0;
            Debug.Log("StopIn");
        }
        if (time >= 1.5f && stopOut == false)
        {
            stopOut = true;
            if(CaveMove.isDied) isLoad = true;  // 동굴에서 죽으면 거울넘어갈때의 fadeOut(화면 어두워짐)까지 재생완료한 후
            else stopIn = false;
            time = 0;
            Debug.Log("StopOut");
        }
        if (isLoad && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {   // 포톤 씬 Fade In 동기화 : 방에 참여하였고 입장한 플레이어가 한명이라도 있다면 fadeIn 재생(화면 밝아짐)
            stopIn = false;
            isLoad = false;
            if (CaveMove.isDied) isLoad = false;
        }
    }

    public float animTime = 2f;         // Fade 애니메이션 재생 시간 (단위:초).    

    private float start = 1f;           // Mathf.Lerp 메소드의 첫번째 값.  
    private float end = 0f;             // Mathf.Lerp 메소드의 두번째 값.  
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.  

    public bool stopIn = true; //false일때 실행되는건데, 초기값을 false로 한 이유는 게임 시작할때 페이드인으로 들어가려고...그게 싫으면 true로 하면됨.
    public bool stopOut = true;

    // 흰색->투명
    public void PlayFadeIn()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        // Image 컴포넌트의 색상 값 읽어오기.  
        Color color = Image.color;
        // 알파 값 계산.  
        color.a = Mathf.Lerp(start, end, time);
        // 계산한 알파 값 다시 설정.  
        Image.color = color;
        // Debug.Log(time);
    }

    // 투명->흰색
    public void PlayFadeOut()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        // Image 컴포넌트의 색상 값 읽어오기.  
        Color color = Image.color;
        // 알파 값 계산.  
        color.a = Mathf.Lerp(end, start, time);  //FadeIn과는 달리 start, end가 반대다.
                                                 // 계산한 알파 값 다시 설정.  
        Image.color = color;
    }

    public void RunAnims(string animTriggerName)
    {   // 원하는 애니메이션 실행
        animator.SetTrigger(animTriggerName);
    }

    bool CheckAnims(string animsName)
    {   // 현재 실행 중인 애니메이션이 끝났는지 확인해주는 함수
        AnimatorStateInfo curAnims = animator.GetCurrentAnimatorStateInfo(0);
        if (curAnims.IsName(animsName) && curAnims.normalizedTime >= 0.99f)
        {
            return true;
        }
        else return false;
    }
}

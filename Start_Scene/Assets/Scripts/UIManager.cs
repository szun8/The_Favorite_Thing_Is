using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; //textmeshpro 쓸 때

public class UIManager : MonoBehaviour
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
            Destroy(gameObject);

    }
    #endregion Singleton

    [SerializeField] Image UsePanel;
    [SerializeField] GameObject Player;    //플레이어의 위치

    Animator animator;
    TMP_Text tmp;
    public Image Image;            // Image컴포넌트 참조 변수.

    void Start()
    {
        Image = GetComponentInChildren<Image>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            animator = GetComponentInChildren<Animator>();
            tmp = GetComponentInChildren<TMP_Text>();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            
        }
        
    }

    bool talkGround = false, talkJump = false, talkLight = false;
    int cnt = 0;

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (cnt == 0) talkGround = PlayerMove.badak;
            Control_Subtitle();
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {

        }

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
            stopIn = false; //하얗게 전환되고 나서 씬 전환 후 다시 풀거라 넣었다. 그냥 게임 끝낼거면 넣을 필요 없음.
            stopOut = true;
            time = 0;
            Debug.Log("StopOut");
        }
    }

    public float animTime = 2f;         // Fade 애니메이션 재생 시간 (단위:초).    

    private float start = 1f;           // Mathf.Lerp 메소드의 첫번째 값.  
    private float end = 0f;             // Mathf.Lerp 메소드의 두번째 값.  
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.  

    public bool stopIn = true; //false일때 실행되는건데, 초기값을 false로 한 이유는 게임 시작할때 페이드인으로 들어가려고...그게 싫으면 true로 하면됨.
    public bool stopOut = true;

    // 흰색->투명
    void PlayFadeIn()
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
    void PlayFadeOut()
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


    void ChangeTalk(string _str)
    {   // 현재 출력해줄 자막의 텍스트를 바꿔주는 함수
        tmp.text = _str;
    }

    void RunAnims(string runTriggerName, string runTxt, ref bool setTrue, ref bool setFalse)
    {   // 원하는 애니메이션 실행
        Debug.Log(runTriggerName);
        ChangeTalk(runTxt);
        animator.SetTrigger(runTriggerName);
        setTrue = true;
        setFalse = false;

        cnt++;
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

    void Control_Subtitle()
    {
        if (cnt == 0 && talkGround)
        {   // 이동 자막 실행
            RunAnims("isMove", "←  →를 눌러 이동해보자", ref talkJump, ref talkGround);
        }
        
        else if (CheckAnims("wasdMove") && cnt == 1 && talkJump)
        {   // 점프 자막 실행
            RunAnims("isJump", "press Space", ref talkLight, ref talkJump);
        }

        else if (CheckAnims("spaceJump") && cnt == 2 && talkLight)
        {   // 발광 자막 실행->조건 추가해야함
            RunAnims("isLight", "press L", ref talkLight, ref talkLight);
        }
        
    }
}

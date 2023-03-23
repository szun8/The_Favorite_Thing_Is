using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Image DeadPanel;
    GameObject DeadUI;

    void Start()
    {
        DeadUI = GameObject.Find("Dead");
        DeadUI.SetActive(false);
    }
   
    public void PlayerDead()
    {
        DeadUI.SetActive(true);
        StartCoroutine(FadeFlowIn());
    }
    public void PlayerRelive()
    {
        StartCoroutine(FadeFlowOut());
        DeadUI.SetActive(false);
    }

    float time = 0f;    // 0부터 1까지 지속시간
    float F_time = 1f;  // 얼마동안 페이드가 진행될지
    Color alpha;

    // 지금은 DeadPanel만 있기에 판넬 하나에 대해서만 fade 적용하지만
    // 판넬이 많아지면 변수화해서 여러 판넬에 대해서 fade기능 가능하게 수정 가능
    IEnumerator FadeFlowIn()  
    {   
        time = 0f;
        alpha = DeadPanel.color;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);   // 0~1까지 부드럽게 만들어주는 기능, smoothStep써도됨
            DeadPanel.color = alpha;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FadeFlowOut()
    {
        time = 0f;
        alpha = DeadPanel.color;
        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);   // 0~1까지 부드럽게 만들어주는 기능, smoothStep써도됨
            DeadPanel.color = alpha;
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;

    }
}

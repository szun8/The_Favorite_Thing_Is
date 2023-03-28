using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMove : MonoBehaviour
{
    public SkinnedMeshRenderer[] skinMat;
    public bool isColl = false;

    void Awake()
    {
        skinMat = GetComponentsInChildren<SkinnedMeshRenderer>();
    }
    void Start()
    {

    }

    void Update()
    {
        if (isColl)
        {
            MatOut();
            Invoke("JellyDestroy", 1.3f);
        }
    }

    public float animTime = 1f;         // Fade 애니메이션 재생 시간 (단위:초).    

    private float start = 1f;           // Mathf.Lerp 메소드의 첫번째 값.  
    private float end = 0f;             // Mathf.Lerp 메소드의 두번째 값.  
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.  

    public bool stopIn = true; //false일때 실행되는건데, 초기값을 false로 한 이유는 게임 시작할때 페이드인으로 들어가려고...그게 싫으면 true로 하면됨.
    public bool stopOut = true;

    void JellyDestroy()
    {
        Destroy(this.gameObject);
    }


    void MatOut()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        // 컴포넌트의 색상 값 읽어오기.
        foreach (var item in skinMat)
        {
            Color color = item.material.color;
            // 알파 값 계산.  
            color.a = Mathf.Lerp(start, end, time);
            // 계산한 알파 값 다시 설정.  
            item.material.color = color;
            // Debug.Log(time);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    // 스테인글라스 다 밝히고 흰색 fade out후 엔딩 영상 송출
    // 비디오 끝날때 fade in으로 밝아진 스카이 박스 보여주고 끝나느 UI 업
    [SerializeField] StaineGlass staineGlass;
    [SerializeField] Material endingSkybox;
    Animator endAnim;
    Material originSkybox;

    bool isStart = false;

    void Start()
    {
        originSkybox = RenderSettings.skybox;
        endAnim = GameObject.Find("FadeScreen").GetComponent<Animator>();
    }

    void Update()
    {
        if(!isStart && staineGlass.light_state == 7)
        {
            StartCoroutine(StartEnding());
            isStart = true;
        }
        if (isShining)
        {
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(RenderSettings.skybox.GetFloat("_Exposure"), 2.6f, Time.deltaTime*0.5f));
            if (RenderSettings.skybox.GetFloat("_Exposure") > 2.55f) isShining = false;
        }
    }

    bool isShining = false;
    IEnumerator StartEnding()
    {
        UIManager.instnace.RunAnimsBool("isStainGlass", true);  // 현재 나와있는 UI 끄기
        yield return new WaitForSeconds(0.5f);

        endAnim.enabled = true; // fade white
        SoundManager.instnace.VolumeOutBGM();
        yield return new WaitForSeconds(1.5f);

        RenderSettings.skybox = endingSkybox;
        yield return new WaitForSeconds(1f);
        SoundManager.instnace.PlayBGM(6);           // 엔딩 브금 시작
        yield return new WaitForSeconds(2f);
        isShining = true;
        
        yield return new WaitForSeconds(5f);
        // 나비 촤르르륵 예정
        endAnim.enabled = false;
        UIManager.instnace.stopOut = false;
        yield return new WaitForSeconds(2f);

        //videoHandler.instance.SetVideo(3);  // 엔딩 영상 시작
    }

    private void OnDestroy()
    {
        RenderSettings.skybox = originSkybox;
    }
}

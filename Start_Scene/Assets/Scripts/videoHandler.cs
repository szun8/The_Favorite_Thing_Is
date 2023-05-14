using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering;

public class videoHandler : MonoBehaviour
{
    [SerializeField] VideoClip[] clip;
    public VideoPlayer videoPlayer;

    public bool isChanged, isStop;

    #region Singleton
    static public videoHandler instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    #endregion Singleton

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += CheckOver;
    }
    private void Update()
    {
        if(videoPlayer.targetCamera == null)
        {
            videoPlayer.targetCamera = FindObjectOfType<Camera>();
        }
    }

    public void SetVideo(int SceneNum)
    {
        Debug.Log("clipSet : "+SceneNum);
        
        videoPlayer.clip = SceneNum switch
        {
            // 오프닝
            0 => clip[SceneNum],
            // 튜토 -> 심해 전환
            1 => clip[SceneNum],
            // 미러 -> 광장 전환
            4 => clip[2],
            // 엔딩
            _ => clip[3],
        };
        videoPlayer.Play();
        isStop = false;
    }
    void CheckOver(VideoPlayer vp)
    {   // 영상 종료 후 실행기능 정의
        //videoPlayer.Stop();
        Debug.Log("isStop : " + isStop);
        isStop = true;
        StartCoroutine(Fade());
    }

    public IEnumerator Fade()
    {
        StartCoroutine(FadeOut());
        yield return new WaitUntil(() => isChanged);
        isChanged = false;
        
        yield return null;
    }

    public IEnumerator FadeOut()
    {
        bool isSea = false;
        while (videoPlayer.targetCameraAlpha > 0.01f)
        {
            videoPlayer.targetCameraAlpha -= 0.05f;
            if(!isSea && videoPlayer.targetCameraAlpha > 0.1f && ScenesManager.instance.SceneNum == 1)
            {   // 심해에서 비디오가 끝나갈때쯔,,,음
                isSea = true;
                SoundManager.instnace.PlayBGM();
                GameObject.Find("GlobalWaterVolume").GetComponent<Volume>().enabled = true;
                GameObject.FindGameObjectWithTag("Sea").GetComponent<Water>().GetWater(GameObject.FindGameObjectWithTag("Player_mesh").GetComponent<Collider>());
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().useGravity = true;
                UIManager.instnace.RunAnims("isWASD");
                UIManager.instnace.RunAnims("isJelly");
            }
            else if(ScenesManager.instance.SceneNum == 4)
            {   // 광장에서 비디오가 끝나갈때...쯔음
                //SoundManager.instnace.PlayBGM();
                Debug.Log("video Out and BGM On");
            }
            yield return new WaitForSeconds(0.1f);
        }
        isChanged = true;

        // 다음 비디오 재생을 위해 설정 초기화
        videoPlayer.clip = null;
        videoPlayer.targetCameraAlpha = 1;
        yield break;
    }
}

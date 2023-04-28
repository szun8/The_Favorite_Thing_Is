using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoHandler : MonoBehaviour
{
    [SerializeField] VideoClip[] clip;
    VideoPlayer videoPlayer;

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
        Debug.Log("clipSet");
        videoPlayer.clip = clip[SceneNum];
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
            //Debug.Log("333");
            videoPlayer.targetCameraAlpha -= 0.05f;
            if(!isSea && videoPlayer.targetCameraAlpha > 0.1f && ScenesManager.instance.SceneNum == 1)
            {
                isSea = true;
                GameObject.FindGameObjectWithTag("Sea").GetComponent<Water>().GetWater(GameObject.FindGameObjectWithTag("Player_mesh").GetComponent<Collider>());
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().useGravity = true;
                UIManager.instnace.RunAnims("isWASD");
            }
            yield return new WaitForSeconds(0.1f);
        }
        isChanged = true;
    }
}

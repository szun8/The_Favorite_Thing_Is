using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public int SceneNum = 0;
    public bool[] Scene;
    // 0. ture -> 심해씬으로 전환
    // 1. true-> 광장씬으로 전환
    // 2. true -> 프리즘탑씬으로 전환

    #region Singleton
    static public ScenesManager instance;
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
    void Update()
    {
        if (Scene[SceneNum] && Scene.Length >= SceneNum + 1)   // playerMove 스크립트에서 player.pos가 특정 위치 내부일 경우 true 작동
        {
            Scene[SceneNum] = false;
            UIManager.instnace.stopOut = false;
            Invoke("SceneLoad", 2f);
        }
        //else if (sea) SceneManager.LoadSceneAsync("4_Square");
    }

    void SceneLoad()
    {
        SceneManager.LoadSceneAsync(SceneNum + 1);
        SceneNum++;
        Invoke("PlayVideo", 1f);
    }

    void PlayVideo()
    {
        videoHandler.instance.SetVideo(SceneNum);
    }
    
}

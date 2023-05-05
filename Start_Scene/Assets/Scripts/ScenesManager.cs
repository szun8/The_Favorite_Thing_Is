using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public int SceneNum = 0;
    public bool[] Scene;
    // 0. true -> 심해씬으로 전환
    // 1. true-> 동굴씬으로 전환
    // 2. true -> 거울씬으로 전환
    // 3. true -> 광잔씬으로 전환

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
    }

    void SceneLoad()
    {
        SceneManager.LoadSceneAsync(SceneNum + 1);
        SceneNum++;
        if (SceneNum == 1)
        {   // 심해씬으로 가게되면 
            PlayVideo();  // 비디오 바로 송출
            // SoundManager.instnace.PlayBGM(); //-> 사운드 ON 기능
        }
        else if (SceneNum == 2)
        {   // 동굴씬으로 가게되면
            //SoundManager.instnace.PlayBGM(); -> 몇번 씹히길래 CaveMove - start에서 해주기,,,
        }
        else if(SceneNum == 3)
        {   // 거울씬으로 가게되면
            // SoundManager.instnace.PlayBGM(); //-> 사운드 ON 기능
        }
        else if(SceneNum == 4)
        {   // 광장씬으로 가게되면
            // SoundManager.instnace.PlayBGM(); //-> 사운드 ON 기능
            // PlayVideo();
        }
    }

    void PlayVideo()
    {   // 각 씬 넘버에 맞는 비디오 지정되어있음 -> 필요없는 부분은 null할건데 만약 중간에 null안되면 black.mp4채워넣을것
        videoHandler.instance.SetVideo(SceneNum);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public GameObject videoPlayer;

    public bool tutorial = false;   // true -> 심해씬으로 전환
    public bool sea = false;        // true -> 광장씬으로 전환
    public bool square = false;     // true -> 프리즘탑씬으로 전환

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

    // Update is called once per frame
    void Update()
    {
        if (tutorial)   // playerMove 스크립트에서 player.pos가 특정 위치 내부일 경우 true 작동
        {
            videoPlayer.SetActive(true);
            Invoke("SceneOn", 2f);  // fade in out 줄거..
            
        }
        else if (sea) SceneManager.LoadSceneAsync("4_Square");
    }

    void SceneOn()
    {
        SceneManager.LoadScene("2_sea");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;     // 곡의 이름
    public AudioClip clip;  // 곡 파일
}

public class SoundManager : MonoBehaviour
{
    // Singleton 싱글톤
    // 씬1에서 씬2로 load될 때, 기존 씬1 하이라키에 존재했던 오브젝트들이 파괴되고 같은 오브젝트라고 하더라도 새로 load되는 씬2에서 새로 생성한다
    // 당연 그럴 필요가 없이 하나로 여러 씬에서 관리되게 하면 불필요한 일을 줄일 수 있게 되므로 싱글톤을 사용한다고 한다

    #region Singleton
    static public SoundManager instnace;
    void Awake()
    {
        if (instnace == null) {
            instnace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        playSoundName = new string[audioSourceEffects.Length + 1];  // 내가등록한 오디오파일의 수와 맞게 초기화
    }
    #endregion Singleton
    // 실제 재생되고있는 음악 목록
    public AudioSource[] audioSourceEffects;
    public AudioSource[] audioSourceBGM;

    public string[] playSoundName;

    // 재생되고자 하는 음악 목록
    public Sound[] effectSounds;
    public Sound[] bgmSound;

    public void PlaySE(string _name, float volume)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (_name.Equals(effectSounds[i].name))
            {   // 일단 틀고자하는 파일이 존재하는가
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {   // 재생중이지 않은 사운드에 대해서
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].volume = volume;
                        audioSourceEffects[j].Play();
                        return;
                    }   // 재생을 시켜주고 함수 종료
                }
                return;
            }
        }
        Debug.Log(_name + " 사운드가 SoundManager에 등록되지 않았습니다");
    }

    public void StopAllSound()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i].Equals(_name))
            {
                audioSourceEffects[i].Stop();
                break;
            }
            
        }
    }

    // 0 : Opening
    // 1 : tutorial
    // 2 : Sea
    // 3 : Cave
    // 4 : Mirror
    // 5 : Square
    int i, realSceneNum;
    public void PlayBGM()
    {   // 1. 튜토리얼부터 브금 시작
        i = ScenesManager.instance.SceneNum;
        
        //i = realSceneNum - 1;
        Debug.Log(i);
        if (!audioSourceBGM[i].isPlaying)
        {   // 재생중이지 않은 사운드에 대해서
            playSoundName[audioSourceEffects.Length] = bgmSound[i].name;
            audioSourceBGM[i].clip = bgmSound[i].clip;
            audioSourceBGM[i].PlayOneShot(bgmSound[i].clip, 0.1f);
            audioSourceBGM[i].loop = true;
            return;
        }   // 재생을 시켜주고 함수 종료
        
    }
    public float animTime = 5.5f;         // Fade 애니메이션 재생 시간 (단위:초). 
    private float time = 0f;            // Mathf.Lerp 메소드의 시간 값.
    private bool isStop = false;

    private void Update()
    {
        if (isStop)
        {
            time += Time.deltaTime / animTime;
            audioSourceBGM[i].volume = Mathf.Lerp(audioSourceBGM[i].volume, 0, time);
        }
        else if(time != 0)
        {
            time = 0;
        }
    }
    public void VolumeOutBGM()
    {
        isStop = true;
        StartCoroutine(StopBGM());
    }

    IEnumerator StopBGM()
    {
        yield return new WaitForSeconds(animTime);
        audioSourceBGM[i].Stop();
        isStop = false;
    }
}

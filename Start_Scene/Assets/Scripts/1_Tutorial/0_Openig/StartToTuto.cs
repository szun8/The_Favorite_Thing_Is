using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartToTuto : MonoBehaviour
{
    Animator anim;
    [SerializeField] Image[] subtitle;

    bool isL = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        SoundManager.instnace.PlayBGM(0);
    }

    public void OnStart(InputAction.CallbackContext state)
    {
        if (state.performed && !isL)
        {
            StartCoroutine(SceneLoad());
            isL = true;
        }
    }

    // 0 : logo
    // 1 : press to start
    // 2 : control System
    // 3 : Exit
    IEnumerator SceneLoad()
    {
        anim.enabled = false;
        while (true)
        {
            Color color_0 = subtitle[0].color; 
            Color color_1 = subtitle[1].color;
            Color color_2 = subtitle[2].color;
            Color color_3 = subtitle[3].color;

            color_0.a = Mathf.Lerp(color_0.a, 0, Time.deltaTime*5.25f);
            color_1.a = Mathf.Lerp(color_1.a, 0, Time.deltaTime*5.25f);
            color_2.a = Mathf.Lerp(color_2.a, 0, Time.deltaTime*5.25f);
            color_3.a = Mathf.Lerp(color_3.a, 0, Time.deltaTime*5.25f);

            subtitle[0].color = color_0;
            subtitle[1].color = color_1;
            subtitle[2].color = color_2;
            subtitle[3].color = color_3;

            if (subtitle[3].color.a < 0.005f) break;
            yield return new WaitForSeconds(0.01f);
        }

        
        videoHandler.instance.SetVideo(0);

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);

        yield return null;
    }
}

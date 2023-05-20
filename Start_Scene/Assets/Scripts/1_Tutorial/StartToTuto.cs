using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartToTuto : MonoBehaviour
{
    Animator anim;
    [SerializeField] Image startSubtitle;
    bool isL = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnStart(InputAction.CallbackContext state)
    {
        if (state.performed && !isL)
        {
            StartCoroutine(SceneLoad());
            isL = true;
        }
    }

    IEnumerator SceneLoad()
    {
        anim.enabled = false;
        while (true)
        {
            Color color = startSubtitle.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime*3f);
            startSubtitle.color = color;
            if (startSubtitle.color.a < 0.05f) break;
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadSceneAsync(1);
        videoHandler.instance.SetVideo(0);

        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);

        yield return null;
    }
}

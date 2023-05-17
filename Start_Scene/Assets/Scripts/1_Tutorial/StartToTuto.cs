using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartToTuto : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnStart(InputAction.CallbackContext state)
    {
        if (state.performed)
        {
            StartCoroutine(SceneLoad());
        }
    }

    IEnumerator SceneLoad()
    {
        anim.enabled = false;
        GameObject.Find("StartSubtitle").SetActive(false);
        SceneManager.LoadSceneAsync(1);
        videoHandler.instance.SetVideo(0);

        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);

        yield return null;
    }
}

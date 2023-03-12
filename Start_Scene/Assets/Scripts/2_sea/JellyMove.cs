using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMove : MonoBehaviour
{
    Material skin;

    float time = 0f;    // 0부터 1까지 지속시간
    float F_time = 1f;  // 얼마동안 페이드가 진행될지
    Color alpha;

    void Awake()
    {
        skin = GetComponent<Material>();
    }
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            StartCoroutine("FadeSkin");
            Destroy(this.gameObject);
        }
    }

    IEnumerator FadeSkin()
    {
        time = 0f;
        alpha = skin.color;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);   // 0~1까지 부드럽게 만들어주는 기능, smoothStep써도됨
            skin.color = alpha;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
    }
}
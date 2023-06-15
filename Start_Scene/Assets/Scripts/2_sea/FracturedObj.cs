using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedObj : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SetOutBlocksCor());
    }

    [SerializeField] Material blockMat;

    IEnumerator SetOutBlocksCor()
    {   // 폭파된 후 부서진 조각 점점 사라지다가 destroy
        yield return new WaitForSeconds(3f);
        Color color = blockMat.color;

        while (true)
        {
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime);
            blockMat.color = color;
            if (blockMat.color.a < 0.005f) break;
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(gameObject);
        yield return null;
    }
    private void OnDestroy()
    {   // 바뀐 투명도 다시 원상복귀
        Color color = blockMat.color;
        color.a = 1f;
        blockMat.color = color;
    }
}

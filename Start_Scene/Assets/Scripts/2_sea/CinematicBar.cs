using System.Collections;
using UnityEngine;

public class CinematicBar : MonoBehaviour
{
    public static CinematicBar instance { get; private set; }
    [SerializeField] GameObject blackBars;
    [SerializeField] Animator blackBarsAnim;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void ShowBars()
    {
        blackBars.SetActive(true);
    }

    public void HideBars()
    {
        if (blackBars.activeSelf)
            StartCoroutine(HideBarsAndDisable());
    }

    IEnumerator HideBarsAndDisable()
    {
        blackBarsAnim.SetTrigger("HideBars");
        yield return new WaitForSeconds(0.5f);
        blackBars.SetActive(false);
    }
}

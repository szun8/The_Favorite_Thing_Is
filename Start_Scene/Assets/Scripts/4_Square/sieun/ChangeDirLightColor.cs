using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirLightColor : MonoBehaviour
{
    Color originColor;

    [SerializeField] Fire _fire;
    [SerializeField] Color fireColor;
    bool isRed = false;

    [SerializeField] Ivy _ivy;
    [SerializeField] Color ivyColor;
    bool isGreen = false;

    [SerializeField] Cloud _cloud;
    [SerializeField] Color cloudColor;
    bool isBlue = false;

    [SerializeField] YellowBridge _bridge;
    [SerializeField] Color darkColor;
    bool isYellow = false;

    [SerializeField] Light L1;
    [SerializeField] Light L2;

    private void Start()
    {
        originColor = RenderSettings.skybox.GetColor("_Tint");
    }
    void Update()
    {
        if (!isRed && _fire.isDone)
        {
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), fireColor, Time.deltaTime));
            if (RenderSettings.skybox.GetColor("_Tint") == fireColor) isRed = true;
        }

        else if (!isGreen && _ivy.isDone)
        {
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), ivyColor, Time.deltaTime));
            if (RenderSettings.skybox.GetColor("_Tint") == ivyColor) isGreen = true;
        }

        else if (!isBlue && _cloud.isDone)
        {
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), cloudColor, Time.deltaTime));
            if (RenderSettings.skybox.GetColor("_Tint") == cloudColor) isBlue = true;
        }

        else if (!isYellow && _bridge.isDone)
        {   // 스테인 글라스 가면 더 어둡게(빛이랑 스카이박스)
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), darkColor, Time.deltaTime));
            L1.intensity = Mathf.Lerp(L1.intensity, 0, Time.deltaTime * 0.9f);
            L2.intensity = Mathf.Lerp(L2.intensity, 0, Time.deltaTime * 0.9f);
            if (RenderSettings.skybox.GetColor("_Tint") == originColor && L1.intensity <= 0.01) isYellow = true;
        }
    }

    private void OnDestroy()
    {   // 스카이박스 색상 원상복귀
        RenderSettings.skybox.SetColor("_Tint", originColor);
    }
}

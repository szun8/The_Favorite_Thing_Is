using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirLightColor : MonoBehaviour
{
    [SerializeField] Color originColor;

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
    bool isYellow = false;

    // Update is called once per frame
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
        {
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), originColor, Time.deltaTime));
            if (RenderSettings.skybox.GetColor("_Tint") == originColor) isYellow = true;
        }
    }

    private void OnDestroy()
    {   // 스카이박스 색상 원상복귀
        RenderSettings.skybox.SetColor("_Tint", originColor);
    }
}
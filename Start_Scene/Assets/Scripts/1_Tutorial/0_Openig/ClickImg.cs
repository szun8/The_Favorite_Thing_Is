using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClickImg : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject controlImg;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            controlImg.SetActive(true);
        }
    }

    public void OnCancle(InputAction.CallbackContext state)
    {
        if (state.performed)
        {
            controlImg.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RunUI : MonoBehaviourPun
{
    GameObject realPlayer;
    void GetPhotonViewID()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in player)
        {
            if (item.GetPhotonView().IsMine) realPlayer = item;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player_mesh")) return;

        GetPhotonViewID();
        
        if(other.gameObject.GetComponentInParent<PhotonView>().ViewID == realPlayer.GetPhotonView().ViewID)
        {
            if(gameObject.name.Contains("L_press"))
                UIManager.instnace.RunAnims("isL_Square");

            if(gameObject.name.Contains("Get") && !other.gameObject.GetComponentInParent<MultiPlayerMove>().getRed)
                UIManager.instnace.RunAnims("isL_Square");

            if(gameObject.name == "press R" && other.gameObject.GetComponentInParent<MultiPlayerMove>().getRed)
                UIManager.instnace.RunAnims("isLight_R");

            if (gameObject.name.Contains("G") && other.gameObject.GetComponentInParent<MultiPlayerMove>().getGreen)
                UIManager.instnace.RunAnims("isLight_G"); 

            if (gameObject.name.Contains("press B") && other.gameObject.GetComponentInParent<MultiPlayerMove>().getBlue)
            {
                Debug.Log("blue");
                UIManager.instnace.RunAnims("isLight_B");
            }
                
        }
    }
}

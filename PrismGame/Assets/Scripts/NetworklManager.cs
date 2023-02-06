using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworklManager : MonoBehaviourPunCallbacks
{

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        
    }
}

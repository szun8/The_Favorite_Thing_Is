using UnityEngine;
using Photon.Pun;

public class ReverseGravity : MonoBehaviourPun
{
    NetworkManager networkManager;


    public Vector3 gravityDirection = Vector3.down;
    private const float GravityForce = 9.81f;
    private const float GravityScale = 1.0f;
    Rigidbody rigidbody;
    PhotonView PV;
    public int player_ID; //1p의 뷰아이디 

   

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        //1p의 뷰 아이디를 가져오자 RPC함수로 다른 놈들한테도 전달함 player_ID 
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PV.RPC("SetID", RpcTarget.AllBuffered);
            
        }
        
    }

    

    private void FixedUpdate()
    {
        PV.RPC("SetGravity", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void SetGravity() //1p는 중력위로 2p는 중력 아래로 
    {
        if (PV.ViewID == player_ID) {
            rigidbody.AddForce(gravityDirection * GravityForce * 2);
            
        }
    }

    [PunRPC]
    void SetID() // 1p의 뷰 아이디를 동기화 
    {
        player_ID = PV.ViewID;
        networkManager.p1_id = player_ID;
        gravityDirection = Vector3.up;
        transform.eulerAngles = new Vector3(0, 0, -180f);

    }

}

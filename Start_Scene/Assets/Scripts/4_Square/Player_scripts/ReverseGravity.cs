using UnityEngine;
using Photon.Pun;

public class ReverseGravity : MonoBehaviourPunCallbacks
{
    NetworkManager networkManager;
    private const float GravityForce = 9.81f;
    Rigidbody rigidbody;
    PhotonView PV;
    public int P1_ID; //1p의 뷰아이디는 네트워크 매니저에서 받아오자

    public bool isReversed = false;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        //일단 2P 놈이 뒤집힌 채로 시작. 
        if (PV.IsMine && PV.ViewID == 2001) //귀찮으면 2001로 바꿔서 테스트 해보자 + 대신 스폰 포인트 1, 2 바꿔야함 
        {
            PV.RPC("SyncisReversed", RpcTarget.AllBuffered);
            Debug.Log("i am Reverse");
            PV.RPC("Sync1pViewID", RpcTarget.AllBuffered, PV.ViewID);
            transform.rotation= Quaternion.Euler(0, 0, -179f);
            
        }
        
    }
    private void Update()
    {
        PV.RPC("GravityControl", RpcTarget.AllBuffered);
    }

    //1p의 뷰 아이디를 저장하고 networkManager에게 주어서 2P도 사용 할 수 있게
    [PunRPC]
    void Sync1pViewID(int viewID)
    {
        networkManager.p1_id = viewID;
        P1_ID = networkManager.p1_id;
    }

    [PunRPC]
    void SyncisReversed() => isReversed = true;


    [PunRPC]
    void GravityControl()
    {
        if (isReversed) //1p는 중력위로 2p는 중력 아래로
            rigidbody.AddForce(Vector3.up * GravityForce * 2f);

        else rigidbody.AddForce(Vector3.down * 1.3f);
    }
}

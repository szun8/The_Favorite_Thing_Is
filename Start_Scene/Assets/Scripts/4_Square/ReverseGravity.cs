using UnityEngine;
using Photon.Pun;

public class ReverseGravity : MonoBehaviourPunCallbacks
{
    NetworkManager networkManager;
    private const float GravityForce = 9.81f;
    //private const float GravityScale = 1.0f;
    Rigidbody rigidbody;
    PhotonView PV;
    public int P1_ID; //1p의 뷰아이디는 네트워크 매니저에서 받아오자

    public bool isReversed = false;

   

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        if (PV.IsMine && PV.ViewID == 1001)
        {
            Debug.Log("i am 1P");
            PV.RPC("Sync1pViewID", RpcTarget.AllBuffered, PV.ViewID);
            transform.eulerAngles = new Vector3(0, 0, -180f);
            isReversed = true;
        }
        

       

    }
    private void Update()
    {
        if (PV.ViewID == P1_ID) //1p는 중력위로 2p는 중력 아래로
            rigidbody.AddForce(Vector3.up * GravityForce * 2f);
        
    }

    //1p의 뷰 아이디를 저장하고 networkManager에게 주어서 2P도 사용 할 수 있게
    [PunRPC]
    void Sync1pViewID(int viewID)
    {
        networkManager.p1_id = viewID;
        P1_ID = networkManager.p1_id;
    }

}

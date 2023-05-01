using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//백색광 코드!!!!!! 
public class RGBLamp : MonoBehaviourPunCallbacks
{
    Animator animator;
    GameObject stopPlayer;
    PhotonView PV;
    Rigidbody rigid; //전구가 검출할 플레이어의 rigidbody 접근

    MultiPlayerMove multiPlayerMove;

    private RaycastHit hit;  //전구에 검출된 플레이어

    public bool isColor = false;  //전구 불 켜져 있는지 여부 - 이 전구 역할 끝 

    public bool isPlayer;  // 전구가 플레이어를 검출 했는지 여부

    public bool isShort; //전구 높낮이에 따른 레이 길이 조정 용도

    public Light pointLight;

    void Awake() 
    {
        animator = GetComponentInChildren<Animator>();
        PV = GetComponent<PhotonView>();

    }

    void Update()
    {
        Ray();

        if (!isColor)
        {
            if (hit.collider != null && isPlayer && rigid == null)
            {
                PV.RPC("SetStopPlayer", RpcTarget.AllBuffered, hit.collider.gameObject.GetComponentInParent<PhotonView>().ViewID);

                if (CheckGetColor()) PV.RPC("FreezePlayer", RpcTarget.AllBuffered);
            }
        }
        

    }
    
    bool CheckGetColor()    // 그 색을 먹은 플레이어라면 못먹게해 
    {
        if ((gameObject.CompareTag("R_item") && !multiPlayerMove.getRed) ||
            (gameObject.CompareTag("G_item") && !multiPlayerMove.getGreen) ||
            (gameObject.CompareTag("B_item") && !multiPlayerMove.getBlue))
            return true;
        else return false;
    }

    void Ray()
    {
        if (isShort)
        {
            //위로 광선 쏴서 가로등 빛나도록 하기
            Debug.DrawRay(transform.position, Vector3.down * 4f, Color.blue);
            isPlayer = Physics.Raycast(transform.position, Vector3.down, out hit, 4f, LayerMask.GetMask("LightPlayer"));
        }
        else
        {
            //위로 광선 쏴서 가로등 빛나도록 하기
            Debug.DrawRay(transform.position, Vector3.down * 4.2f, Color.blue);
            isPlayer = Physics.Raycast(transform.position, Vector3.down, out hit, 4.2f, LayerMask.GetMask("LightPlayer"));
            
        }
        
    }

    //딸깍 당하는 플레이어 rpc로 다 전달하게 하기
    [PunRPC]
    void SetStopPlayer(int viewID)
    {
        stopPlayer = PhotonView.Find(viewID).gameObject;
        multiPlayerMove = stopPlayer.GetComponent<MultiPlayerMove>();
    }

    //플레이어 공중 딸깍 동안 멈추게 하기 
    [PunRPC]
    void FreezePlayer()
    {
        if (stopPlayer != null)
        {
            //키 입력을 아예 못받게 해버리자
            stopPlayer.GetComponent<MultiPlayerMove>().enabled = false;

            //이거 하면 공중부양
            rigid = stopPlayer.GetComponent<Rigidbody>();
            rigid.constraints = RigidbodyConstraints.FreezePosition;

            StartCoroutine(LampManager());

        }
    }

    IEnumerator LampManager()
    {
        yield return new WaitForSeconds(1f);
        PV.RPC("SyncLight", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(0.5f);
        PV.RPC("UnFreezePlayer", RpcTarget.AllBuffered);
        PV.RPC("PlayerRGB", RpcTarget.AllBuffered);
    }

    

    //플레이어 딸깍 멈췄다가 다시 움직이게 하기 
    [PunRPC]
    void UnFreezePlayer()
    {
        stopPlayer.GetComponent<MultiPlayerMove>().enabled = true;
        stopPlayer.GetComponent<MultiPlayerMove>().l_pressed = false;
        rigid.constraints = RigidbodyConstraints.FreezeRotation;
        isColor = true;
    }

    //플레이어의 색 능력 얻게 하기 
    [PunRPC]
    void PlayerRGB()
    {
        if (stopPlayer != null)
        {
            //이 가로등이 무슨 색을 갖는 태그 인지
            if (gameObject.CompareTag("R_item")) stopPlayer.GetComponent<MultiPlayerMove>().getRed = true;

            if (gameObject.CompareTag("G_item")) stopPlayer.GetComponent<MultiPlayerMove>().getGreen = true;

            if (gameObject.CompareTag("B_item")) stopPlayer.GetComponent<MultiPlayerMove>().getBlue = true;

        }
    }

   

    [PunRPC]
    void SyncLight() => animator.SetBool("isOn", true);
   

    
    

}
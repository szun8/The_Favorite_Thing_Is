using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionMonster : MonoBehaviour
{
    public PhysicMaterial physicMaterial; //마찰력 0인 피지컬머티리얼 
    PhysicMaterial defaultMaterial;     //원래 기본 플레이어 머티리얼 = none
    MultiPlayerMove PlayerMove;


    void Awake()
    {
        PlayerMove = GetComponent<MultiPlayerMove>();
        defaultMaterial = GetComponentInChildren<MeshCollider>().material;
    }


    //쿵쿵이와 충돌해있을 경우에는 떨어지도록 플레이어 마찰력 0으로 
    private void OnCollisionStay(Collision collision)
    {
        //착몬을 그라운드 태그로 두자 !!!
        //땅 (돌)에 붙으면 안되니 ground 도 해주지만 밟고 있을때는 아닌걸로 위해서 
        if (collision.gameObject.CompareTag("UpKung") || collision.gameObject.CompareTag("DownKung") || collision.gameObject.CompareTag("Ice")  || (collision.gameObject.CompareTag("Ground") && !PlayerMove.isGround) )
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = physicMaterial;

        }

      
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("UpKung") || collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("DownKung") || collision.gameObject.CompareTag("Ice"))
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = defaultMaterial;
        }
    }




}

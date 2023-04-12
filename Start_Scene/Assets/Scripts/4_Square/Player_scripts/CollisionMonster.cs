using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionMonster : MonoBehaviour
{
    public PhysicMaterial physicMaterial; //마찰력 0인 피지컬머티리얼 
    PhysicMaterial defaultMaterial;     //원래 기본 플레이어 머티리얼 = none

    
    void Awake() => defaultMaterial = GetComponentInChildren<MeshCollider>().material;


    //쿵쿵이와 충돌해있을 경우에는 떨어지도록 플레이어 마찰력 0으로 
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("KungKung"))
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = physicMaterial;

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("KungKung"))
        {
            gameObject.GetComponentInChildren<MeshCollider>().material = defaultMaterial;
        }
    }




}

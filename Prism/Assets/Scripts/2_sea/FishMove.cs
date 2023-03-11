using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMove : MonoBehaviour
{
    Rigidbody rigid;
    CapsuleCollider coll;
    //InfoFish infoFish;
    Animator animator;

    Vector3 moveDir;
    string fishName;
    int numOfFish;
    bool isWall;

    void Start()
    {
        fishName = gameObject.name;
        // 물고기 생성 시 각 번호가 이름에 부여되어 enemyManager속
        // [spawnEnemy]-[infoFish]에 각 물고기들의 정보가 저장되어있음
        numOfFish = int.Parse(fishName.Substring(fishName.Length - 1)); // 물고기 번호 추출
        //infoFish = GameObject.Find("EnemyManager").GetComponent<SpawnEnemy>().infoFish[numOfFish];// 물고기 정보 추출
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        IsRay("Wall");
        IsRay("Enemy");
        //rigid.MovePosition(this.transform.position + moveDir * Time.deltaTime * infoFish.speed);
                
        if (isWall)
        {
            if (moveDir == Vector3.right)
            {   // 오른쪽으로 가고 있으면 왼쪽으로 도는 애니메이션과 직진방향 변경
                animator.SetTrigger("leftRotate");
                moveDir = Vector3.left;
            }
            else
            {   // 왼쪽으로 가고 있으면 오른쪽으로 도는 애니메이션과 직진방향 변경
                animator.SetTrigger("rightRotate");
                moveDir = Vector3.right;
            }
        }
    }

    // 물고기(적)이 감지해야할 대상 : 벽(회전)
    bool IsRay(string mask)
    {
        return Physics.Raycast(transform.position, moveDir, 3f, LayerMask.GetMask(mask));
    }

}

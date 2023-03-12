using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Dir
{
    right,
    left
}

public class FishMove : MonoBehaviour
{
    Rigidbody rigid;
    CapsuleCollider coll;
    InfoFish infoFish; // type = A, B, Boss 
    Animator animator;

    Vector3 moveDir, rayDistance;
    string fishName, fishType;
    int fishNum;
    bool isWall;

    void Awake()
    {
        rayDistance = new Vector3(0f, 3f, 0f);
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        // 물고기 정보 추출

        fishName = gameObject.name;
        fishNum = int.Parse(fishName.Substring(fishName.Length - 1)); // 물고기 번호 추출
        fishType = fishName.Substring(0, 6);
        Debug.Log(fishType);

        if (fishType.Equals("enemyA"))
            infoFish = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().FishA[fishNum];

        else
            infoFish = GameObject.Find("SpawnManager").GetComponent<SpawnEnemy>().FishB[fishNum];
        Debug.Log(fishName + " : " + fishType.Equals("enemyA"));   // 추출한 정보가 맞는지 확인

        if (transform.rotation == Quaternion.Euler(0, 90, 0))
        {
            SetDirection(Dir.right);
        }
        else
        {
            SetDirection(Dir.left);
        }
    }

    void Update()
    {
        //IsRay("Enemy");
        rigid.MovePosition(this.transform.position + moveDir * Time.deltaTime * infoFish.speed);
        Debug.DrawRay(transform.position + rayDistance, moveDir, Color.magenta);
        isWall = IsRay("Wall");

        if (isWall)
        {
            if (moveDir == Vector3.right)
            {   // 오른쪽으로 가고 있으면 왼쪽으로 도는 애니메이션과 직진방향 변경
                animator.SetTrigger("leftRotate");
                SetDirection(Dir.left);
            }
            else
            {   // 왼쪽으로 가고 있으면 오른쪽으로 도는 애니메이션과 직진방향 변경
                animator.SetTrigger("rightRotate");
                SetDirection(Dir.right);
            }
        }
    }

    // 물고기(적)이 감지해야할 대상 : 벽(회전)
    bool IsRay(string mask)
    {
        return Physics.Raycast(transform.position + rayDistance, moveDir, 3f, LayerMask.GetMask(mask));
    }

    // 회전에 따른 물고기의 바라보는 방향 및 ray 수정적,
    void SetDirection(Dir _dir)
    {
        switch (_dir)
        {
            case Dir.left:
                moveDir = Vector3.left;
                rayDistance.x = -3f;
                break;

            case Dir.right:
                moveDir = Vector3.right;
                rayDistance.x = 3f;
                break;
        }
    }

}

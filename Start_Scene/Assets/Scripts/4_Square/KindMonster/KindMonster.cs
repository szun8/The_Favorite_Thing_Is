using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KindMonster : MonoBehaviour
{
    public Flowers flower;          //꽃의 isBloom 가져오기 위함 
    public KindManager kindManager; //isWalk bool 값 가져와서 이동하게 하려고 

    public bool isArrive = false; //꽃 앞 콜라이더에 다다르면 true 

    private Vector3 pos;
    private float speed = 2f;

    //거북이 몸뚱아리 움직이게 하려고 부모꺼 
    void Awake() => pos = transform.parent.position;

    void Update()
    {
        // 꽃이 피면 + walk Anim이면  
        if (flower.isBloom && kindManager.isWalk) Move();

    }
    void Move()
    {
        if (!isArrive)
        {
            pos += Vector3.right * speed * Time.deltaTime;

            transform.parent.position = pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TurtleStop")) isArrive = true;
    }


}

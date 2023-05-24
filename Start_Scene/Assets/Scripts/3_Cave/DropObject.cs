using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField] GameObject[] Stone;
    [SerializeField] GameObject player;

    public GameObject _stone;
    Vector3 curPlayer_pos;
    public static bool isDroped = true; // false일경우 새로운 스톤 생성 불가

    public bool isCreate = true;

    void Update()
    {
        curPlayer_pos = player.transform.position;
    }

    public void StartDrop()
    {
        isCreate = true;
        StartCoroutine(CreateStoneRoutine());
    }

    public void EndDrop()
    {
        isCreate = false;
        StopAllCoroutines();
    }

    IEnumerator CreateStoneRoutine()
    {
        while (isCreate)
        {
            if ((_stone == null) || (curPlayer_pos.x > _stone.transform.position.x+2 && isDroped))
            {
                CreateStone(curPlayer_pos);
            } 
            yield return new WaitForSeconds(1f);
        }
    }

    void CreateStone(Vector3 playerPos)
    {
        Vector3 pos = new Vector3(Random.Range(3f, 5f) + playerPos.x, 35f, playerPos.z);
        isDroped = false;
        _stone = Stone[Random.Range(0, 3)];
        _stone = Instantiate(_stone, pos, _stone.transform.rotation);
        _stone.transform.parent = this.transform;
    }
}

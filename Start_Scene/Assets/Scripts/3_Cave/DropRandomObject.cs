using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRandomObject : MonoBehaviour
{
    [SerializeField] GameObject[] Stone;

    BoxCollider rangeCollider;
    private void Awake()
    {
        rangeCollider = GetComponent<BoxCollider>();
    }
    void Start()
    {
        StartCoroutine(CreateRandomStoneRoutine());
    }

    IEnumerator CreateRandomStoneRoutine()
    {
        while (true)
        {
            CreateRandomStone();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void CreateRandomStone()
    {
        Vector3 pos = Return_RandomPos_Stone();
        GameObject _stone = Stone[Random.Range(0, 3)];
        _stone = Instantiate(_stone, pos, _stone.transform.rotation);
        _stone.transform.parent = gameObject.transform;
    }

    Vector3 Return_RandomPos_Stone()
    {
        Vector3 originPos = transform.position;
        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range(0f, range_Z / 2);
        Vector3 randomPos = new Vector3((int)range_X, 20f, (int)range_Z);
        Vector3 respawnPos = originPos + randomPos;
        return respawnPos;
    }
}

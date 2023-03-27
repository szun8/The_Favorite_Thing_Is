using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InfoFish
{
    public string name;
    public float speed;
    public Vector3 spawnSpot;
}

enum TypeFish
{
    enemy,
    boss,
    jelly
}

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemy;

    Quaternion[] rotation;

    public InfoFish[] FishA;    
    public InfoFish[] Jelly;
    public InfoFish Boss;

    public GameObject rangeJelly;  // 해파리가 랜덤 소환될 구역 오브젝트
    BoxCollider rangeCollider;

    private void Awake()
    {
        rotation = new Quaternion[3];

        rangeCollider = rangeJelly.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        SetQuaternion();
        InstantiateFish();
    }

    void SetQuaternion()
    {
        for (int i = 0; i < rotation.Length; i++)
            rotation[i] = enemy[i].transform.rotation;
    }

    void InstantiateFish()
    {
        for (int i = 0; i < FishA.Length; i++)
            InstantiateFish(FishA[i], i, TypeFish.enemy);

        for (int i = 0; i < Jelly.Length; i++)
            InstantiateFish(Jelly[i], i, TypeFish.jelly);

        InstantiateFish(Boss, 0, TypeFish.boss);
    }

    void InstantiateFish(InfoFish _gameObject, int i, TypeFish _type)
    {
        GameObject clone;
        _gameObject.name = _type + "_" + i;
        switch (_type)
        {
            //case TypeFish.enemy:
            //    
            //    break;
            
            case TypeFish.boss:
                _gameObject.speed = 20f;
                _gameObject.spawnSpot = new Vector3(160f, 40f, 0f);
                break;

            case TypeFish.jelly:
                _gameObject.speed = 0f;
                _gameObject.spawnSpot = Return_RandomPos_jelly();
                break;
        }

        clone = Instantiate(enemy[(int)_type], _gameObject.spawnSpot, rotation[(int)_type]);
        clone.name = _gameObject.name;
        if (clone.tag == "Enemy") clone.transform.parent = this.transform;
        else clone.transform.parent = GameObject.Find("Jelly_RespawnRange").transform;

        if (_type == TypeFish.boss) clone.gameObject.SetActive(false);
    }

    Vector3 Return_RandomPos_jelly()
    {   // 지정한 특정구역 내에 해파리를 랜덤 소환해주는 함수
        Vector3 originPos = rangeJelly.transform.position;
        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = rangeCollider.bounds.size.x;    // 세로
        float range_Y = rangeCollider.bounds.size.y;    // 높이
        float range_Z = rangeCollider.bounds.size.z;    // 가로

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range(5f , range_Y-5f);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPos = new Vector3(range_X, range_Y, range_Z);

        Vector3 respawnPos = originPos + RandomPos;
        return respawnPos;
    }
}

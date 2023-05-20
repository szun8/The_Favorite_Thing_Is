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
    jelly,
    boss,
}

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject[] seaObj;
    bool isDestroy = false;
    Quaternion[] rotation;
 
    public InfoFish[] Jelly;
    public InfoFish Boss;
    public Transform bossSpawn;

    public GameObject rangeJelly;       // 해파리가 랜덤 소환될 구역 오브젝트
    BoxCollider rangeJellyCollider;
    
    private void Awake()
    {
        rotation = new Quaternion[2];

        rangeJellyCollider = rangeJelly.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        SetQuaternion();
        InstantiateFish();
    }

    void SetQuaternion()
    {   // 각 바다 객체들의 회전 값 가져오는 함수
        for (int i = 0; i < rotation.Length; i++)   
            rotation[i] = seaObj[i].transform.rotation;
    }

    void InstantiateFish()
    {
        for (int i = 0; i < Jelly.Length; i++)      // 해파리 스폰
            InstantiateFish(Jelly[i], i, TypeFish.jelly);

        InstantiateFish(Boss, 0, TypeFish.boss);    // 보스 스폰
    }

    void InstantiateFish(InfoFish _gameObject, int i, TypeFish _type)
    {   // 입력된 개체를 생성하는 함수
        GameObject clone;

        _gameObject.name = _type + "_" + i;
        switch (_type)
        {   
            case TypeFish.boss:
                _gameObject.speed = 25f;
                _gameObject.spawnSpot = bossSpawn.position;
                break;

            case TypeFish.jelly:
                _gameObject.speed = 0f;
                _gameObject.spawnSpot = Return_RandomPos_jelly();
                break;
        }

        clone = Instantiate(seaObj[(int)_type], _gameObject.spawnSpot, rotation[(int)_type]);
        clone.name = _gameObject.name;

        // 바다 객체들 하이라키에서 위치할 부모 계층 조정
        if (clone.tag == "Enemy") clone.transform.parent = this.transform;
        else if(clone.tag == "Item") clone.transform.parent = GameObject.Find("Jelly_RespawnRange").transform;

        // 보스는 스폰되자마자 비활성화
        if (_type == TypeFish.boss) clone.gameObject.SetActive(false);
    }

    Vector3 Return_RandomPos_jelly()
    {   // 지정한 특정구역 내에 해파리를 랜덤 소환해주는 함수
        Vector3 originPos = rangeJelly.transform.position;
        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = rangeJellyCollider.bounds.size.x;    // 세로
        float range_Y = rangeJellyCollider.bounds.size.y;    // 높이

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPos = new Vector3((int)range_X, range_Y + rangeJellyCollider.center.y, 0f);

        Vector3 respawnPos = originPos + RandomPos;
        return respawnPos;
    }

    public void DestroyJellyFish()
    {   // 플레이어 사망 시 기존 해파리 삭제하고 재생성
        
        if (!isDestroy)
        {
            isDestroy = true;
            StartCoroutine(DestroyJellyFishCoroutine());
        }
    }

    IEnumerator DestroyJellyFishCoroutine()
    {
        Transform[] jellyParent = rangeJelly.GetComponentsInChildren<Transform>();
        yield return new WaitForSeconds(1.5f);  // 플레이어가 죽자마자 해파리가 바로 사라져서 1.5초 뒤에 destroy
        foreach (var item in jellyParent)
        {
            if (item == rangeJelly.transform) continue;
            if (item != null) Destroy(item.gameObject);
        }

        for (int i = 0; i < Jelly.Length; i++)
        {
            Debug.Log("jelly_" + i);
            InstantiateFish(Jelly[i], i, TypeFish.jelly);
        }
        isDestroy = false;
        yield return null;
    }
}

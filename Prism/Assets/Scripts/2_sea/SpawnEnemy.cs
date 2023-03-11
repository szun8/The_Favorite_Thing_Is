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
    enemyA,
    enemyB,
    boss
}

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemy;

    Quaternion[] rotation;

    public InfoFish[] FishA;    // 5마리
    public InfoFish[] FishB;
    public InfoFish Boss;

    private void Awake()
    {
        rotation = new Quaternion[3];
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
            InstantiateFish(FishA[i], i, TypeFish.enemyA);

        for (int i = 0; i < FishB.Length; i++)
            InstantiateFish(FishB[i], i, TypeFish.enemyB);

        InstantiateFish(Boss, 0, TypeFish.boss);
    }

    void InstantiateFish(InfoFish _gameObject, int i, TypeFish _type)
    {
        GameObject clone;
        _gameObject.name = _type + "_" + i;
        switch (_type)
        {
            case TypeFish.enemyA:
                _gameObject.speed = Random.Range(3f, 10f);
                _gameObject.spawnSpot = new Vector3(Random.Range(-30f, 15f), Random.Range(-13f, -3f), Random.Range(-5, 25));
                break;
            case TypeFish.enemyB:   // 일정한 것이 좋을까?
                _gameObject.speed = Random.Range(7f, 14f);
                if(i%2==0)
                    _gameObject.spawnSpot = new Vector3(Random.Range(-85f, -35f), Random.Range(-5f, 10f), Random.Range(20f, 28f));
                else
                    _gameObject.spawnSpot = new Vector3(Random.Range(-85f, -35f), Random.Range(-5f, 10f), Random.Range(0f, 8f));
                break;
            case TypeFish.boss:
                _gameObject.speed = 20f;
                _gameObject.spawnSpot = new Vector3(-75f, -5f, 10f);
                break;
        }

        clone = Instantiate(enemy[(int)_type], _gameObject.spawnSpot, rotation[(int)_type]);
        clone.name = _gameObject.name;
        clone.transform.parent = this.transform;

        if (_type == TypeFish.boss) clone.gameObject.SetActive(false);
    }
}

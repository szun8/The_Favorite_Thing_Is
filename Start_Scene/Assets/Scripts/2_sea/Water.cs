using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static bool isWater = false; // 물속인지 아닌지
    [SerializeField]
    private float waterDrag; // 물속 중력 : 물 속에 들어가면 천천히 가라앉게하는 변수
    private float originDrag; // 물밖 중력 : 물 밖으로 나가면 원래의 중력으로 돌아오게하는 변수

    [SerializeField]
    private float waterFogDensity; // 물 탁함 정도 : 높으면 가까운것만 보이게되고 낮으면 멀리있는것도 보이게 하는 변수 like fog
    private float originFogDensity;

    [SerializeField] private string sound_WaterIn;
    [SerializeField] private string sound_WaterOut;
    [SerializeField] private string sound_WaterBreathe;

    [SerializeField] private float breatheTime;
    private float currentBreatheTime;

    // Start is called before the first frame update
    void Start()
    {
        originFogDensity = RenderSettings.fogDensity;
        originDrag = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWater)
        {
            currentBreatheTime += Time.deltaTime;
            if(currentBreatheTime >= breatheTime)
            {   // 물 속에서 숨쉬는 거처럼 일정 시간마다 숨쉬는 소리 재생
                SoundManager.instnace.PlaySE(sound_WaterBreathe, 0.5f);
                currentBreatheTime = 0;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player_mesh"))
        {
            GetOutWater(other);
        }
    }

    public void GetWater(Collider _player)
    {
        SoundManager.instnace.PlaySE(sound_WaterIn, 0.9f);
        SoundManager.instnace.PlayBGM();
        isWater = true;
        _player.transform.GetComponentInParent<Rigidbody>().drag = waterDrag;

        RenderSettings.fogDensity = waterFogDensity;
    }

    private void GetOutWater(Collider _player)
    {
        if (isWater)
        {   // 물에 들어가 있을때만 빠져나올수 있기때문에
            SoundManager.instnace.PlaySE(sound_WaterOut, 1.0f);
            isWater = false;
            _player.transform.GetComponentInParent<Rigidbody>().drag = originDrag;

            RenderSettings.fogDensity = originFogDensity;
        }
    }
}

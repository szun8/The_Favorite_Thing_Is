using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButterflies : MonoBehaviour
{
    [SerializeField] Ending ending;
    [SerializeField] Transform destPos;
    ParticleSystem ps;
    
    bool isMove = false;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (ending.isShining) isMove = true;
        if (isMove)
        {
            if(!ps.isPlaying) ps.Play();
            transform.position = Vector3.MoveTowards(transform.position, destPos.position, Time.deltaTime*15f);
        }
    }
}

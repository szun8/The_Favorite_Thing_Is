using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Light pLight1;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pLight1.intensity == 12) gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }
}

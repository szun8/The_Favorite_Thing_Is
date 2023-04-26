using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureObject : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject fracturedObject;
    //public GameObject explosionVFX;
    public float explosionMinForce = 5;
    public float explosionMaxForce = 100;
    public float explosionForceRadius = 10;
    public float fragScaleFactor = 1;

    private GameObject fractObj;

    public void Explode()
    {
        if(originalObject != null)
        {
            originalObject.SetActive(false);
            if(fracturedObject != null)
            {
                fractObj = Instantiate(fracturedObject) as GameObject;
                SoundManager.instnace.PlaySE("CaveBroken", 0.85f);
                foreach (Transform t in fractObj.transform)
                {
                    var rb = t.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(Random.Range(explosionMinForce, explosionMaxForce), originalObject.transform.position, explosionForceRadius);
                }
            }
        }
        
    }
}

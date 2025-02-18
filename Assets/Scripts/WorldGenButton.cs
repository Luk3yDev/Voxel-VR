using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenButton : MonoBehaviour
{
    [SerializeField] MapBuilder world;
    bool dontpressthisshitagainyouidiot = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand") && !dontpressthisshitagainyouidiot)
        {
            dontpressthisshitagainyouidiot = true;          
            world.CreateWorld();
            Destroy(transform.parent.gameObject);
        }
    }
}

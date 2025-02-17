using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenButton : MonoBehaviour
{
    [SerializeField] MapBuilder world;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand"))
        {
            world.CreateWorld();
            Destroy(transform.parent.gameObject);
        }
    }
}

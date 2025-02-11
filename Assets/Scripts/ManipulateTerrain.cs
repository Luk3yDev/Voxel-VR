using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManipulateTerrain : MonoBehaviour
{
    public InputActionProperty pinchAction;

    [SerializeField] MapBuilder map;

    [Header("Raycast Parameters")]
    [SerializeField] LayerMask terrainLayer;
    [SerializeField] float range;

    [Header("Voxels")]
    [SerializeField] Voxel air;

    [Header("Visuals")]
    [SerializeField] GameObject indicator;

    float cooldown = 0.1f;

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, terrainLayer))
        {
            Vector3 targetPos = hitInfo.point - (hitInfo.normal * 0.5f);
            Vector3Int voxelPos = new Vector3Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), Mathf.RoundToInt(targetPos.z));
            indicator.SetActive(true);
            indicator.transform.position = voxelPos;
        }
        else
            indicator.SetActive(false);

        float triggerValue = pinchAction.action.ReadValue<float>();
        if (triggerValue > 0.5f && cooldown <= 0)
        {
            DestroyVoxel();
            cooldown = 0.1f;
        }

        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    void DestroyVoxel()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, terrainLayer))
        {
            Vector3 targetPos = hitInfo.point - (hitInfo.normal * 0.5f);
            Vector3Int voxelPos = new Vector3Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), Mathf.RoundToInt(targetPos.z));

            map.SetVoxel(voxelPos, air);
        }
    }
}

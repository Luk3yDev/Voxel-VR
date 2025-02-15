using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManipulateTerrain : MonoBehaviour
{
    public InputActionProperty pinchAction;
    public InputActionProperty gripAction;
    public InputActionProperty buttonAction;

    [SerializeField] MapBuilder map;

    [Header("Raycast Parameters")]
    [SerializeField] LayerMask terrainLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float range;

    [Header("Voxels")]
    [SerializeField] Voxel[] voxels;
    [SerializeField] Material[] voxelHandMats;
    [SerializeField] int currentVoxel;

    [Header("Visuals and Effects")]
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject currentVoxelIndicator;
    [SerializeField] GameObject blockAudioObject;

    float destroyCooldown = 0.3f;
    float switchCooldown = 0.3f;

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
        if (triggerValue > 0.5f && destroyCooldown <= 0)
        {
            DestroyVoxel();
            destroyCooldown = 0.3f;
        }

        if (destroyCooldown > 0)
        {
            destroyCooldown -= Time.deltaTime;
        }
        if (switchCooldown > 0)
        {
            switchCooldown -= Time.deltaTime;
        }

        float gripValue = gripAction.action.ReadValue<float>();
        if (gripValue > 0.5f && switchCooldown <= 0)
        {
            if (currentVoxel < voxels.Length - 1)
            {
                currentVoxel++;
            }
            else
            {
                currentVoxel = 0;
            }
            currentVoxelIndicator.GetComponent<MeshRenderer>().material = voxelHandMats[currentVoxel];
            switchCooldown = 0.3f;
        }

        if (buttonAction.action.triggered)
        {
            PlaceVoxel();
        }
    }

    void DestroyVoxel()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, terrainLayer))
        {
            Vector3 targetPos = hitInfo.point - (hitInfo.normal * 0.5f);
            Vector3Int voxelPos = new Vector3Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), Mathf.RoundToInt(targetPos.z));

            Voxel voxelTB = map.GetVoxel(voxelPos);
            if (voxelTB.breakSound != null)
            {
                GameObject soundObj = Instantiate(blockAudioObject, voxelPos, Quaternion.identity);
                soundObj.GetComponent<AudioSource>().clip = voxelTB.breakSound;
            }
            map.SetVoxel(voxelPos, voxels[0]);
        }
    }

    void PlaceVoxel()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, terrainLayer))
        {
            Vector3 targetPos = hitInfo.point + (hitInfo.normal * 0.5f);

            if (!Physics.CheckSphere(targetPos, 0.45f, playerLayer))
            {
                Vector3Int voxelPos = new Vector3Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), Mathf.RoundToInt(targetPos.z));
                if (voxels[currentVoxel].breakSound != null)
                {
                    GameObject soundObj = Instantiate(blockAudioObject, voxelPos, Quaternion.identity);
                    soundObj.GetComponent<AudioSource>().clip = voxels[currentVoxel].breakSound;
                }
                map.SetVoxel(voxelPos, voxels[currentVoxel]);
            }
        }
    }
}

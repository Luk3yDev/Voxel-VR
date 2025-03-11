using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ManipulateTerrain : MonoBehaviour
{
    public InputActionProperty pinchAction;
    public InputActionProperty gripAction;
    public InputActionProperty LGripAction;
    public InputActionProperty buttonAction;

    [SerializeField] MapBuilder world;
    NetworkWorld netWorld;

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

    bool justBroke;
    bool justSwitched;

    private void Awake()
    {
        netWorld = world.GetComponent<NetworkWorld>();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, terrainLayer))
        {
            Vector3 targetPos = hitInfo.point - (hitInfo.normal * 0.5f);
            Vector3Int voxelPos = new Vector3Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), Mathf.RoundToInt(targetPos.z));
            indicator.SetActive(true);
            indicator.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
            indicator.transform.position = voxelPos;
        }
        else
            indicator.SetActive(false);

        float triggerValue = pinchAction.action.ReadValue<float>();
        if (triggerValue > 0.5f && justBroke == false)
        {
            DestroyVoxel();
            justBroke = true;
        }
        if (triggerValue < 0.5f) justBroke = false;

        float gripValue = gripAction.action.ReadValue<float>();
        float LGripValue = LGripAction.action.ReadValue<float>();
        if (gripValue > 0.5f && justSwitched == false)
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
            
            justSwitched = true;
        }
        if (gripValue < 0.5f) justSwitched = false;

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

            Voxel voxelTB = world.GetVoxel(voxelPos);
            if (voxelTB.breakSound != null)
            {
                GameObject soundObj = Instantiate(blockAudioObject, voxelPos, Quaternion.identity);
                soundObj.GetComponent<AudioSource>().clip = voxelTB.breakSound;
            }

            world.SetVoxel(voxelPos, voxels[0]);
            netWorld.NetworkSetVoxel(voxelPos, voxels[0]);
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

                world.SetVoxel(voxelPos, voxels[currentVoxel]);
                netWorld.NetworkSetVoxel(voxelPos, voxels[currentVoxel]);
            }
        }
    }
}

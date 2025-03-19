using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] int currentVoxel;

    [Header("Visuals and Effects")]
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject currentVoxelIndicator;
    [SerializeField] GameObject blockAudioObject;
    [SerializeField] GameObject blockParticleObject;
    [SerializeField] Material atlasMaterial;

    bool justBroke;
    bool justSwitched;

    private void Awake()
    {
        netWorld = world.GetComponent<NetworkWorld>();

        if (currentVoxel == 0) currentVoxelIndicator.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
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
            if (currentVoxel < VoxelIndexer.length - 1)
            {
                currentVoxel++;
            }
            else
            {
                currentVoxel = 0;
            }

            Voxel voxelTP = VoxelIndexer.IndexToVoxel(currentVoxel);
            Texture2D tex = (Texture2D)atlasMaterial.GetTexture("_BaseMap");
            Color[] data = tex.GetPixels(Mathf.FloorToInt(voxelTP.uvCoordinate.x) * 16, Mathf.FloorToInt(voxelTP.uvCoordinate.y) * 16, 16, 16);
            tex = new Texture2D(16, 16);
            tex.SetPixels(0, 0, 16, 16, data);
            tex.Apply();
            tex.filterMode = FilterMode.Point;

            currentVoxelIndicator.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", tex);
            currentVoxelIndicator.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);

            if (currentVoxel == 0) currentVoxelIndicator.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);

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
            
            Material particleMaterial = new Material(atlasMaterial.shader);           
            GameObject particleObj = Instantiate(blockParticleObject, voxelPos, Quaternion.identity);

            Texture2D tex = (Texture2D)atlasMaterial.GetTexture("_BaseMap");
            Color[] data = tex.GetPixels(Mathf.FloorToInt(voxelTB.uvCoordinate.x) * 16, Mathf.FloorToInt(voxelTB.uvCoordinate.y) * 16, 16, 16);
            tex = new Texture2D(16, 16);
            tex.SetPixels(0, 0, 16, 16, data);
            tex.Apply();
            tex.filterMode = FilterMode.Point;

            ParticleSystemRenderer psr = particleObj.GetComponent<ParticleSystemRenderer>();
            psr.material = particleMaterial;
            psr.material.EnableKeyword("_BASEMAP");
            psr.material.SetTexture("_BaseMap", tex);

            world.SetVoxel(voxelPos, VoxelIndexer.IndexToVoxel(0));
            netWorld.NetworkSetVoxel(voxelPos, VoxelIndexer.IndexToVoxel(0));
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
                if (VoxelIndexer.IndexToVoxel(currentVoxel).breakSound != null)
                {
                    GameObject soundObj = Instantiate(blockAudioObject, voxelPos, Quaternion.identity);
                    soundObj.GetComponent<AudioSource>().clip = VoxelIndexer.IndexToVoxel(currentVoxel).breakSound;
                }

                world.SetVoxel(voxelPos, VoxelIndexer.IndexToVoxel(currentVoxel));
                netWorld.NetworkSetVoxel(voxelPos, VoxelIndexer.IndexToVoxel(currentVoxel));
            }
        }
    }
}

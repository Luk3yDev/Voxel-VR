using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoiseGenerator : MonoBehaviour
{
    [Header("Voxels")]
    [SerializeField] Voxel airVoxel;
    [SerializeField] Voxel grassVoxel;
    [SerializeField] Voxel dirtVoxel;
    [SerializeField] Voxel stoneVoxel;

    [Header("Noise Params")]
    [SerializeField] float horizontalScale;
    [SerializeField] float verticalScale;
    int seed;

    private void Awake()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Second);
        seed = UnityEngine.Random.Range(-1000, 1000);
    }

    public Voxel GetVoxelAtPos(Vector3Int pos)
    {
        float noiseVal = noise.snoise(new float3((float)pos.x + seed, 0, (float)pos.z + seed) / horizontalScale);
        if (((noiseVal + 1) * verticalScale) > pos.y)
        {
            return stoneVoxel;
        }
        if (((noiseVal + 1) * verticalScale) > pos.y-1)
        {
            return dirtVoxel;
        }
        if (((noiseVal + 1) * verticalScale) > pos.y-2)
        {
            return grassVoxel;
        }
        return airVoxel;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] Voxel grassVoxel;
    [SerializeField] Voxel dirtVoxel;
    [SerializeField] Voxel airVoxel;
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
            return dirtVoxel;
        }
        if (((noiseVal + 1) * verticalScale) > pos.y-1)
        {
            return grassVoxel;
        }
        return airVoxel;
    }
}
